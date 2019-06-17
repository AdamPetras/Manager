using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;
using Manager.Annotations;
using Manager.Extensions;
using Manager.Mappers;
using Manager.Model;
using Manager.Model.Enums;
using Manager.Model.Interfaces;
using Manager.Resources;
using Manager.SaveManagement;
using Manager.Views;
using Xamarin.Forms;

namespace Manager.ViewModels
{
    public class TableUcVm:INotifyPropertyChanged
    {
        
        public event PropertyChangedEventHandler PropertyChanged;
        public ObservableCollection<TableItemUcVm> RecordList { get; private set; } = new ObservableCollection<TableItemUcVm>();
        public static ObservableCollection<TableItemUcVm> SavedRecordList { get; set; } = new ObservableCollection<TableItemUcVm>();
        private TableItemUcVm _selectedItem;
        private uint _datesFounded;
        private WorkTime _workHours;
        private double _priceTogether;
        private uint _piecesTogether;
        private string _searchDate;
        private double _bonusTogether;
        private uint _selectedPeriod;
        private uint _vacationDays;
        private double _averagePricePerHour;
        private double _totalPriceForHourType;
        private IXmlManager _saveAndLoad;
        private double _totalOvertimePrice;
        private WorkTime _totalOvertimeHours;

        public uint SelectedPeriod
        {
            get => _selectedPeriod;
            set
            {
                _selectedPeriod = value;
                OnPropertyChanged(nameof(SelectedPeriod));
            }
        }

        public TableItemUcVm SelectedItem
        {
            get => _selectedItem;
            set
            {
                _selectedItem = value;
                OnPropertyChanged(nameof(SelectedItem));
            }
        }

        public uint DatesFounded
        {
            get => _datesFounded;
            set
            {
                _datesFounded = value; 
                OnPropertyChanged(nameof(DatesFounded));
            }
        }

        public string HoursFounded => _workHours.ToString();

        public WorkTime WorkHours
        {
            get => _workHours;
            set
            {
                _workHours = value;
                OnPropertyChanged(nameof(HoursFounded));
            }
        }

        public uint PiecesTogether
        {
            get => _piecesTogether;
            set
            {
                _piecesTogether = value;
                OnPropertyChanged(nameof(PiecesTogether));
            }
        }

        public double BonusTogether
        {
            get => Math.Round(_bonusTogether, 1);
            set
            {
                _bonusTogether = value;
                OnPropertyChanged(nameof(BonusTogether));
            }
        }

        public double PriceTogether
        {
            get => Math.Round(_priceTogether,1);
            set
            {
                _priceTogether = value;
                OnPropertyChanged(nameof(PriceTogether));
            }
        }
        public string SearchDate
        {
            get => _searchDate;
            set
            {
                _searchDate = value;
                OnPropertyChanged(nameof(SearchDate));
            }
        }

        public ICommand ShowStatisticsCommand { get; }

        public ICommand FindByDateCommand { get; }

        public static List<string> WorkingTermList = new List<string>(EnumMapper.MapEnumToStringArray<ESelectedStage>());

        public TableUcVm()
        {
            ShowStatisticsCommand = new Command(ShowStatistics);
            FindByDateCommand = new Command(FindByDate);
            _saveAndLoad = new XmlManager(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), "recordData.xml"));
            LoadRecordsAsync();
            MessagingCenter.Subscribe<TableItemUcVm>(this, "Add", PersistAdd);
            MessagingCenter.Subscribe<TableItemUcVm, TableItemUcVm>(this, "Modify", PersistModify);
            MessagingCenter.Subscribe<TableItemUcVm>(this, "Remove", item => { PersistRemove(item);});
            MessagingCenter.Subscribe<IBaseRecord, EDeleteAction>(this, "ClearRecords", Clear);
            SearchDate = "";
        }

        public void FindByDate()
        {
            ClearUp();
            RecordList.Clear();
            foreach (TableItemUcVm tableItem in SavedRecordList)
            {
                if (tableItem.Record.DateString.Contains(SearchDate))
                {
                    CalcAndSet(tableItem);
                }
            }
            CalculateAveragePrice();
        }

        private void Clear(IBaseRecord rec, EDeleteAction action)
        {
            switch (action)
            {
                case EDeleteAction.All:
                    ClearAll();
                    break;
                case EDeleteAction.Vacations:
                    ClearVacations();
                    break;
                case EDeleteAction.LastYear:
                    ClearLastYear();
                    break;
                case EDeleteAction.LastYears:
                    ClearLastYears();
                    break;
                case EDeleteAction.ThisYear:
                    ClearThisYear();
                    break;
                case EDeleteAction.ThisMonth:
                    ClearThisMonth();
                    break;
                case EDeleteAction.LastMonth:
                    ClearLastMonth();
                    break;
                case EDeleteAction.VacationsThisYear:
                    ClearVacationsThisYear();
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(action), action, null);
            }
        }

        private void ClearVacationsThisYear()
        {
            PersistRemove(new List<TableItemUcVm>(SavedRecordList.Where(s => s.Record.Date.Year == DateTime.Today.Year && s.Record.Type == ERecordType.Vacation)));
        }

        private void ClearLastMonth()
        {
            int month = 12;
            int year = DateTime.Today.Year - 1;
            if (DateTime.Today.Month != 1)
            {
                month = DateTime.Today.Month - 1;
                year = DateTime.Today.Year;
            }
            PersistRemove(new List<TableItemUcVm>(SavedRecordList.Where(s => s.Record.Date.Year == year && s.Record.Date.Month == month)));
        }

        private void ClearThisMonth()
        {
            PersistRemove(new List<TableItemUcVm>(SavedRecordList.Where(s => s.Record.Date.Year == DateTime.Today.Year && s.Record.Date.Month == DateTime.Today.Month)));
        }

        private void ClearThisYear()
        {
            PersistRemove(new List<TableItemUcVm>(SavedRecordList.Where(s => s.Record.Date.Year == DateTime.Today.Year)));
        }

        private void ClearAll()
        {
            _saveAndLoad.CreateNewXmlFile();
            SavedRecordList.Clear();
            RecordList.Clear();
        }

        private void ClearVacations()
        {
            PersistRemove(new List<TableItemUcVm>(SavedRecordList.Where(s => s.Record.Type == ERecordType.Vacation)));
        }

        private void ClearLastYear()
        {
            PersistRemove(new List<TableItemUcVm>(SavedRecordList.Where(s => s.Record.Date.Year == DateTime.Today.Year-1)));
        }

        private void ClearLastYears()
        {
            PersistRemove(new List<TableItemUcVm>(SavedRecordList.Where(s => s.Record.Date.Year != DateTime.Today.Year)));
        }

        private async void LoadRecordsAsync()
        {
            await Task.Factory.StartNew(() =>
            {
                foreach (IBaseRecord tmp in _saveAndLoad.LoadXmlFile())
                {
                    SelectedPeriod = 0;
                    SavedRecordList.Add(new TableItemUcVm(tmp));
                }
                ClearAndWriteStatistics();
            });
        }

        private void PersistModify(TableItemUcVm find, TableItemUcVm modify)
        {
            SelectedPeriod = 0;
            for (int i = 0; i < SavedRecordList.Count; i++)
            {
                if (SavedRecordList[i] == find)
                {
                    _saveAndLoad.EditXmlRecord(SavedRecordList[i].Record, modify.Record);
                    SavedRecordList[i] = modify;
                }
            }
            ((MasterDetailPage)Application.Current.MainPage).Detail = MainPage.TableTab;
            ClearAndWriteStatistics();
        }

        private void PersistRemove(TableItemUcVm item, bool isUpdated = false)
        {
            SelectedPeriod = 0;
            SavedRecordList.Remove(item);
            _saveAndLoad.RemoveXmlRecord(item.Record);
            if (!isUpdated)
                LoadRecordsAsync();
        }

        private void PersistRemove(TableItemUcVm[] items)
        {
            foreach (TableItemUcVm item in items)
            {
                PersistRemove(item);
            }
            LoadRecordsAsync();
        }

        private void PersistRemove(List<TableItemUcVm> itemList)
        {
            PersistRemove(itemList.ToArray());
        }

        private void PersistAdd(TableItemUcVm item)
        {
            SelectedPeriod = 0;
            SavedRecordList.Add(item);
            _saveAndLoad.AppendXmlFile(item.Record);
            ClearAndWriteStatistics();
        }

        private void ShowStatistics()
        {
            Application.Current.MainPage.DisplayAlert(AppResource.Statistics, AppResource.AveragePricePerHour+": "+ _averagePricePerHour + 
                                                                              "\n"+AppResource.Bonuses+": "+BonusTogether+
                                                                              "\n"+AppResource.VacationDays+": "+_vacationDays+
                                                                              "\n"+AppResource.TotalOvertimePrice+": "+_totalOvertimePrice+
                                                                              "\n"+AppResource.TotalOvertimeHours+": "+_totalOvertimeHours, AppResource.CancelButton);
        }

        private void ClearUp()
        {
            DatesFounded = 0;
            WorkHours = new WorkTime();
            PiecesTogether = 0;
            _vacationDays = 0;
            BonusTogether = 0;
            _averagePricePerHour = 0;
            _totalPriceForHourType = 0;
            PriceTogether = 0;
            _totalOvertimePrice = 0;
            _totalOvertimeHours = new WorkTime();
        }

        public void ClearAndWriteStatistics()
        {
            ClearUp();
            RecordList.Clear();
            SearchDate = "";
            switch (SelectedPeriod)
            {
                case (int)ESelectedStage.All:
                    foreach (TableItemUcVm tmp in SavedRecordList)
                    {
                        CalcAndSet(tmp);
                    }
                    break;
                case (int)ESelectedStage.LastWeek:
                    Statistics.Week(DateTime.Today.AddDays(-7), SavedRecordList,CalcAndSet);
                    break;
                case (int)ESelectedStage.LastMonth:
                    Statistics.Month(SavedRecordList, DateTime.Today.Month - 1, CalcAndSet);
                    break;
                case (int)ESelectedStage.Week:
                    Statistics.Week(DateTime.Today, SavedRecordList, CalcAndSet);
                    break;
                case (int)ESelectedStage.Month:
                    Statistics.Month(SavedRecordList, DateTime.Today.Month, CalcAndSet);
                    break;
                case (int)ESelectedStage.Year:
                    Statistics.Year(SavedRecordList, CalcAndSet);
                    break;
                case (int)ESelectedStage.VacationAll:
                    foreach (TableItemUcVm tmp in SelectVacations(SavedRecordList))
                    {
                        CalcAndSet(tmp);
                    }
                    break;
                case (int)ESelectedStage.VacationYear:
                    Statistics.Year(SelectVacations(SavedRecordList), CalcAndSet);
                    break;
            }
            CalculateAveragePrice();
        }

        private void CalculateAveragePrice()
        {
            if ((WorkHours.Hours + WorkHours.Minutes / 60.0) != 0)
                _averagePricePerHour = Math.Round(_totalPriceForHourType / (WorkHours.Hours + WorkHours.Minutes / 60.0), 1);
        }

        private ObservableCollection<TableItemUcVm> SelectVacations(IReadOnlyCollection<TableItemUcVm> records)
        {
            return new ObservableCollection<TableItemUcVm>(records.Where(s => s.Record.Type == ERecordType.Vacation).ToList());
        }

        private void CalcAndSet(TableItemUcVm rec)
        {
            DatesFounded++;
            RecordList.Add(rec);
            switch (rec.Record.Type)
            {
                case ERecordType.Hours:
                    SetHoursStats((IHoursRecord)rec.Record);
                    break;
                case ERecordType.Pieces:
                    PiecesTogether += ((IPiecesRecord)rec.Record).Pieces;
                    break;
            }
            if (rec.Record.Type != ERecordType.Vacation)
            {
                BonusTogether += ((IRecord) rec.Record).Bonus;
                double.TryParse(rec.Record.TotalPrice, NumberStyles.Any, CultureInfo.InvariantCulture,
                    out double price);
                if (rec.Record.Type == ERecordType.Hours)
                {
                    _totalPriceForHourType += price;
                }
                PriceTogether += price;
            }
            else
            {
                _vacationDays++;
            }
            
        }

        private void SetHoursStats(IHoursRecord rec)
        {
            WorkHours += rec.Time + rec.OverTime;
            _totalOvertimeHours += rec.OverTime;
            _totalOvertimePrice += rec.OverTime.Hours * rec.Price+(rec.OverTime.Minutes/60.0)*rec.Price;
        }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
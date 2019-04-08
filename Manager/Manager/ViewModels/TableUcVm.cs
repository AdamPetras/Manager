using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using Manager.Annotations;
using Manager.Model;
using Manager.Model.Enums;
using Manager.Model.Interfaces;
using Manager.SaveManagement;
using Xamarin.Forms;

namespace Manager.ViewModels
{
    public class TableUcVm:INotifyPropertyChanged
    {
        
        public event PropertyChangedEventHandler PropertyChanged;
        public ObservableCollection<TableItemUcVm> RecordList { get; private set; } = new ObservableCollection<TableItemUcVm>();
        private ObservableCollection<TableItemUcVm> SavedRecordList { get; set; }
        private TableItemUcVm _selectedItem;
        private uint _datesFounded;
        private WorkTime _workHours;
        private double _priceTogether;
        private uint _piecesTogether;
        private double _bonusTogether;
        private uint _selectedPeriod;
        private IXmlManager _saveAndLoad;

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

        public TableUcVm()
        {
            SavedRecordList = new ObservableCollection<TableItemUcVm>();
            InitializeTableItems();
            _saveAndLoad = new XmlManager(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), "recordData.xml"));
            foreach (IBaseRecord tmp in _saveAndLoad.LoadXmlFile())
            {
                SelectedPeriod = 0;
                SavedRecordList.Add(new TableItemUcVm(tmp));
                ShowStatistics();
            }
            MessagingCenter.Subscribe<TableItemUcVm>(this, "Add", (item) =>
            {
                SelectedPeriod = 0;
                SavedRecordList.Add(item);
                _saveAndLoad.AppendXmlFile(item.Record);
                ShowStatistics();
            });

            MessagingCenter.Subscribe<TableItemUcVm>(this, "Remove", (item) =>
            {
                SelectedPeriod = 0;
                SavedRecordList.Remove(item);
                _saveAndLoad.RemoveXmlRecord(item.Record);
                ShowStatistics();
            });
            MessagingCenter.Subscribe<TableItemUcVm, TableItemUcVm>(this, "Modify", (find,modify) =>
            {
                SelectedPeriod = 0;
                for (int i = 0; i< SavedRecordList.Count;i++)
                {
                    if (SavedRecordList[i] == find)
                    {
                        SavedRecordList[i] = modify;
                    }
                }
                ShowStatistics();
            });
        }

        private void ClearUp()
        {
            DatesFounded = 0;
            WorkHours = new WorkTime();
            PiecesTogether = 0;
            BonusTogether = 0;
            PriceTogether = 0;
        }

        public void ShowStatistics()
        {
            ClearUp();
            RecordList.Clear();
            switch (SelectedPeriod)
            {
                case (int)ESelectedStage.All:
                    foreach (TableItemUcVm tmp in SavedRecordList)
                    {
                        CalcAndSet(tmp);
                    }
                    break;
                case (int)ESelectedStage.LastWeek:
                    Week(DateTime.Now.AddDays(-7), SavedRecordList);
                    break;
                case (int)ESelectedStage.LastMonth:
                    Month(SavedRecordList, DateTime.Now.Month - 1);
                    break;
                case (int)ESelectedStage.Week:
                    Week(DateTime.Now, SavedRecordList);
                    break;
                case (int)ESelectedStage.Month:
                    Month(SavedRecordList, DateTime.Now.Month);
                    break;
                case (int)ESelectedStage.Year:
                    Year(SavedRecordList);
                    break;
                case (int)ESelectedStage.VacationAll:
                    foreach (TableItemUcVm tmp in SelectVacations(SavedRecordList))
                    {
                        CalcAndSet(tmp);
                    }
                    break;
                case (int)ESelectedStage.VacationYear:
                    Year(SelectVacations(SavedRecordList));
                    break;
            }
        }

        private ObservableCollection<TableItemUcVm> SelectVacations(IReadOnlyCollection<TableItemUcVm> records)
        {
            return new ObservableCollection<TableItemUcVm>(records.Where(s => s.Record.Type == ERecordType.Vacation).ToList());
        }

        private Tuple<DateTime, DateTime> GetStartAndEndOfTheWeek(DateTime now)
        {
            while (now.DayOfWeek != DayOfWeek.Monday)  //iterace dokud nenarazím na pondìlí
            {
                now = now.AddDays(-1);
            }
            if (now.DayOfWeek == DayOfWeek.Monday)
            {
                now = now.AddDays(-1);
            }
            DateTime endDate = now.AddDays(7);
            return new Tuple<DateTime, DateTime>(now, endDate);
        }

        private void Week(DateTime startDate, IReadOnlyCollection<TableItemUcVm> records)
        {
            Tuple<DateTime, DateTime> dates = GetStartAndEndOfTheWeek(startDate);
            foreach (TableItemUcVm rec in records)
            {
                if ((rec.Record.Date > dates.Item1) && (rec.Record.Date < dates.Item2))
                {
                    CalcAndSet(rec);
                }
            }
        }

        private void Month(IReadOnlyCollection<TableItemUcVm> records, int month)
        {
            foreach (TableItemUcVm rec in records)
            {
                if (rec.Record.Date.Month == month && rec.Record.Date.Year == DateTime.Now.Year)
                {
                    CalcAndSet(rec);
                }
            }
        }
        private void Year(IReadOnlyCollection<TableItemUcVm> records)
        {
            foreach (TableItemUcVm rec in records)
            {
                if (rec.Record.Date.Year == DateTime.Now.Year)
                {
                    CalcAndSet(rec);
                }
            }
        }

        private void CalcAndSet(TableItemUcVm rec)
        {
            DatesFounded++;
            RecordList.Add(rec);
            switch (rec.Record.Type)
            {
                case ERecordType.Hours:
                    WorkHours += ((IHoursRecord) rec.Record).Time;
                    break;
                case ERecordType.Pieces:
                    PiecesTogether += ((IPiecesRecord)rec.Record).Pieces;
                    break;
            }
            if (rec.Record.Type != ERecordType.Vacation)
            {
                BonusTogether += ((IRecord)rec.Record).Bonus;
                Double.TryParse(rec.Record.TotalPrice, out double price);
                PriceTogether += price;
            }
        }

        private void InitializeTableItems()
        {
            SavedRecordList.Add(new TableItemUcVm(new PiecesRecord(DateTime.Now, 10, 10, 10,"Description 1",false)));
            SavedRecordList.Add(new TableItemUcVm(new HoursRecord(DateTime.Now, 10, 10, 10, 10,"Description 2",true)));
            SavedRecordList.Add(new TableItemUcVm(new VacationRecord(DateTime.Now, "Description 3Description 3Description 3Description 3Description 3Description 3Description 3Description 3")));
        }

        

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
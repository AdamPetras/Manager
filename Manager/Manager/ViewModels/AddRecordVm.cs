using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
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
using Xamarin.Forms;


namespace Manager.ViewModels
{
    public class AddRecordVm : INotifyPropertyChanged
    {
        private DateTime _date;
        private DateTime _dateTo;
        private string _bonus;
        private string _price;
        private uint _pieces;
        private int _selectedPicker;
        private string _buttonText;
        private string _description;
        private TableItemUcVm _modifying;
        private bool _isCancelModifyVisible;
        private int _buttonAddColumnSpan;
        private TimeSpan _workTimeFrom;
        private TimeSpan _workTimeTo;
        private TimeSpan _overTime;
        private TimeSpan _breakTime;
        public ICommand CancelModify { get; }
        public ICommand ButtonAdd { get; }
        public event PropertyChangedEventHandler PropertyChanged;

        public string Description
        {
            get => _description;
            set
            {
                _description = value;
                OnPropertyChanged(nameof(Description));
            }
        }

        public string ButtonText
        {
            get => _buttonText;
            set
            {
                _buttonText = value;
                OnPropertyChanged(nameof(ButtonText));
            }
        }

        public int SelectedPicker
        {
            get => _selectedPicker;
            set
            {
                _selectedPicker = value;
                OnPropertyChanged(nameof(SelectedPicker));
            }
        }

        public DateTime Date
        {
            get => _date;
            set
            {
                if (DateTo < value)
                {
                    DateTo = value;
                }
                _date = value;
                OnPropertyChanged(nameof(Date));
            }
        }

        public DateTime DateTo
        {
            get => _dateTo;
            set
            {
                DateTime resetDate = DateTo;
                if (value < Date)
                {
                    Application.Current.MainPage.DisplayAlert(AppResource.WrongDateTitle, AppResource.WrongDateMessage,
                        "Ok");
                    _dateTo = resetDate;
                }
                else
                {
                    _dateTo = value;
                }
                OnPropertyChanged(nameof(DateTo));
            }
        }

        public string Bonus
        {
            get => _bonus;
            set
            {
                _bonus = value.IsDigitsOnly()? value.Replace('.', ',') : value.Remove(value.Length - 1);
                OnPropertyChanged(nameof(Bonus));
            }
        }


        public string Price
        {
            get => _price;
            set
            {
                _price = value.IsDigitsOnly() ? value.Replace('.', ',') : value.Remove(value.Length-1);
                OnPropertyChanged(nameof(Price));
            }
        }

        public uint Pieces
        {
            get => _pieces;
            set
            {
                _pieces = value;
                OnPropertyChanged(nameof(Pieces));
            }
        }

        public ObservableCollection<string> PickerRecordTypes { get; } =
            new ObservableCollection<string>(EnumMapper.MapEnumToStringArray<ERecordType>().Where(s=>s.Length>0));

        public bool IsCancelModifyVisible
        {
            get => _isCancelModifyVisible;
            set
            {
                _isCancelModifyVisible = value;
                OnPropertyChanged(nameof(IsCancelModifyVisible));
            }
        }


        public int ButtonAddColumnSpan
        {
            get => _buttonAddColumnSpan;
            set
            {
                _buttonAddColumnSpan = value;
                OnPropertyChanged(nameof(ButtonAddColumnSpan));
            }
        }

        public TimeSpan WorkTimeFrom
        {
            get => _workTimeFrom;
            set
            {
                if (value > WorkTimeTo)
                    WorkTimeTo = value;
                _workTimeFrom = value;
                OnPropertyChanged(nameof(WorkTimeFrom));
            }
        }

        public TimeSpan WorkTimeTo
        {
            get => _workTimeTo;
            set
            {
                _workTimeTo = value;
                OnPropertyChanged(nameof(WorkTimeTo));
            }
        }

        public TimeSpan OverTime
        {
            get => _overTime;
            set
            {
                _overTime = value;
                OnPropertyChanged(nameof(OverTime));
            }
        }

        public TimeSpan BreakTime
        {
            get => _breakTime;
            set
            {
                _breakTime = value;
                OnPropertyChanged(nameof(BreakTime));
            }
        }

        public AddRecordVm()
        {
            ButtonText = AppResource.AddButton;
            _date = DateTime.Today;
            _dateTo = DateTime.Today;
            ButtonAdd = new Command(AddButtonCommand);
            CancelModify = new Command(ClearModifyProperty);
            SetButtonCancelModifyVisible(false);
            MessagingCenter.Subscribe<TableItemUcVm>(this, "ModifyItem", SetupModifyAction);
            MessagingCenter.Subscribe<DateTimeAsReference>(this, "SetupDate", SetupDate);
            ClearValues();
        }

        private void SetupModifyAction(TableItemUcVm modifer)
        {
            _modifying = modifer;
            if (modifer != null)
            {
                if (modifer.Record.Type != ERecordType.None)
                {
                    Date = modifer.Record.Date;
                    ButtonText = AppResource.ModifyButton;
                    SetButtonCancelModifyVisible();
                    switch (modifer.Record.Type)
                    {
                        case ERecordType.Vacation:
                            SelectedPicker = (int) ERecordType.Vacation;
                            return;
                        case ERecordType.Hours:
                            _workTimeTo = new TimeSpan(((IHoursRecord) modifer.Record).WorkTimeTo.Hours,((IHoursRecord)modifer.Record).WorkTimeTo.Minutes,0);
                            _workTimeFrom = new TimeSpan(((IHoursRecord) modifer.Record).WorkTimeFrom.Hours,((IHoursRecord)modifer.Record).WorkTimeFrom.Minutes,0);
                            _overTime = new TimeSpan(((IHoursRecord)modifer.Record).OverTime.Hours, ((IHoursRecord)modifer.Record).OverTime.Minutes,0);
                            SelectedPicker = (int) ERecordType.Hours;
                            break;
                        case ERecordType.Pieces:
                            Pieces = ((IPiecesRecord) modifer.Record).Pieces;
                            SelectedPicker = (int) ERecordType.Pieces;
                            break;
                    }
                    Price = ((IRecord) modifer.Record).Price.ToString(CultureInfo.InvariantCulture);
                    Bonus = ((IRecord) modifer.Record).Bonus.ToString(CultureInfo.InvariantCulture);
                }
                else
                {
                    SetButtonCancelModifyVisible(false);
                    _modifying = null;
                    ClearValues();
                    ButtonText = AppResource.AddButton;
                }
            }
        }

        

        private void ClearValues()
        {
            SelectedPicker = 0;
            Date = DateTime.Today;
            Description = "";
            ReloadConfigValues();
            DateTo = DateTime.Today;
            Bonus = "0";
            OverTime = TimeSpan.Zero;
        }

        public void ReloadConfigValues()
        {
            WorkTimeFrom = SaveStaticVariables.DefaultTimeFrom;
            WorkTimeTo = SaveStaticVariables.DefaultTimeTo;
            Pieces = SaveStaticVariables.DefaultPieces;
            Price = SaveStaticVariables.DefaultPrice.ToString(CultureInfo.InvariantCulture);
            BreakTime = new TimeSpan(0,30,0);
        }

        private void SetButtonCancelModifyVisible(bool visible = true)
        {
            IsCancelModifyVisible = visible;
            ButtonAddColumnSpan = SetTermPickerColumnSpan(visible);
        }

        private int SetTermPickerColumnSpan(bool isButtonVisible)
        {
            return isButtonVisible ? 3 : 5;
        }

        private void ClearModifyProperty()
        {
            SetupModifyAction(new TableItemUcVm(new NoneRecord()));
        }

        private async void AddButtonCommand()
        {
            
            if (await ExistingDate())
            {
                NightShift();
                await Application.Current.MainPage.DisplayAlert("Info",
                    AppResource.AddMessage + " " + AddOrModifyRecord() + ".", "OK");
                ClearValues();
            }
        }

        private async Task<bool> ExistingDate()
        {
            bool answer = true;
            if (TableUcVm.SavedRecordList.Any(s => s.Record.Date.Year == Date.Year && s.Record.Date.Month == Date.Month && s.Record.Date.Day == Date.Day))
                answer = await Application.Current.MainPage.DisplayAlert(AppResource.ExistDateDialogTitle,
                    AppResource.ExistingDateDialogMessage, AppResource.Yes, AppResource.No);
            return answer;
        }

        private void NightShift()
        {
            if (WorkTimeFrom > WorkTimeTo)
            {
                WorkTimeTo = new TimeSpan(1, WorkTimeTo.Hours, WorkTimeTo.Minutes, WorkTimeTo.Seconds);
            }
        }

        private string AddOrModifyRecord()
        {
            if (_modifying == null)
            {
                Add();
                return AppResource.Added.ToLower();
            }
            Modify();
            return AppResource.Modified.ToLower();
        }

        private void Add()
        {
            IBaseRecord[] records = CreateRecords();
            foreach (IBaseRecord rec in records)
            {
                MessagingCenter.Send(new TableItemUcVm(rec), "Add");
            }
        }

        private void Modify()
        {
            IBaseRecord[] records = CreateRecords();
            if (records.Length == 1)
                MessagingCenter.Send(_modifying, "Modify", new TableItemUcVm(records[0]));
            _modifying = null;
            ButtonText = AppResource.AddButton;
            SetButtonCancelModifyVisible(false);
        }

        private IBaseRecord[] CreateRecords()
        {
            double.TryParse(Price, out double price);
            double.TryParse(Bonus, out double bonus);
            switch (SelectedPicker)
            {
                case (int)ERecordType.Hours:
                    return new IBaseRecord[] {new HoursRecord(Date, WorkTimeFrom , WorkTimeTo, price, bonus, Description, OverTime.Hours,OverTime.Minutes, new WorkTime(BreakTime.Hours,BreakTime.Minutes))};
                case (int)ERecordType.Pieces:
                    return new IBaseRecord[] {new PiecesRecord(Date, Pieces, price, bonus, Description)};
                default:
                    return AddVacationRecords();
            }
        }


        private void SetupDate(DateTimeAsReference cal)
        {
            Date = cal.Date;
        }



        private IBaseRecord[] AddVacationRecords()
        {
            DateTime tmpDate = Date;
            Debug.WriteLine((int) (DateTo-tmpDate).TotalDays);
            IBaseRecord[] vacationRecords = new IBaseRecord[(int)(DateTo - tmpDate).TotalDays + 1];
            for (int i =0;tmpDate <= DateTo;i++)
            {
                vacationRecords[i] = new VacationRecord(tmpDate, Description);
                tmpDate = tmpDate.AddDays(1);
            }
            return vacationRecords;
        }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
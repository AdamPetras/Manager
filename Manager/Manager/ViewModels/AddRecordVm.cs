using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Manager.Annotations;
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
        private double _bonus;
        private uint _hours;
        private uint _minutes;
        private double _price;
        private uint _pieces;
        private int _selectedPicker;
        private string _buttonText;
        private string _description;
        private TableItemUcVm _modifying;
        private bool _isCancelModifyVisible;
        private int _buttonAddColumnSpan;
        private uint _overTimeHours;
        private uint _overTimeMinutes;
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
                DateTime tmp = _dateTo;
                if (value < Date)
                {
                    Application.Current.MainPage.DisplayAlert(AppResource.WrongDateTitle, AppResource.WrongDateMessage,
                        "Ok");
                    DateTo = tmp;
                }
                else
                {
                    _dateTo = value;
                    OnPropertyChanged(nameof(DateTo));
                }
            }
        }

        public double Bonus
        {
            get => _bonus;
            set
            {
                _bonus = value;
                OnPropertyChanged(nameof(Bonus));
            }
        }

        public uint Hours
        {
            get => _hours;
            set
            {
                if (value > 23)
                    value = 23;
                _hours = value;
                OnPropertyChanged(nameof(Hours));
            }
        }

        public uint Minutes
        {
            get => _minutes;
            set
            {
                if (value > 59)
                    value = 59;
                _minutes = value;
                OnPropertyChanged(nameof(Minutes));
            }
        }


        public double Price
        {
            get => _price;
            set
            {
                _price = value;
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

        public Command CancelModify { get; }

        public int ButtonAddColumnSpan
        {
            get => _buttonAddColumnSpan;
            set
            {
                _buttonAddColumnSpan = value;
                OnPropertyChanged(nameof(ButtonAddColumnSpan));
            }
        }

        public uint OverTimeHours
        {
            get => _overTimeHours;
            set
            {
                if (value > 23)
                    value = 23;
                _overTimeHours = value;
                OnPropertyChanged(nameof(OverTimeHours));
            }
        }

        public uint OverTimeMinutes
        {
            get => _overTimeMinutes;
            set
            {
                if (value > 59)
                    value = 59;
                _overTimeMinutes = value;
                OnPropertyChanged(nameof(OverTimeMinutes));
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
                            Hours = ((IHoursRecord) modifer.Record).Time.Hours;
                            Minutes = ((IHoursRecord) modifer.Record).Time.Minutes;
                            SelectedPicker = (int) ERecordType.Hours;
                            break;
                        case ERecordType.Pieces:
                            Pieces = ((IPiecesRecord) modifer.Record).Pieces;
                            SelectedPicker = (int) ERecordType.Pieces;
                            break;
                    }
                    Price = ((IRecord) modifer.Record).Price;
                    Bonus = ((IRecord) modifer.Record).Bonus;
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
            Bonus = 0;
            OverTimeHours = 0;
            OverTimeMinutes = 0;
        }

        public void ReloadConfigValues()
        {
            Hours = SaveStaticVariables.DefaultHours;
            Minutes = SaveStaticVariables.DefaultMinutes;
            Pieces = SaveStaticVariables.DefaultPieces;
            Price = SaveStaticVariables.DefaultPrice;
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

        private void AddButtonCommand()
        {
            Application.Current.MainPage.DisplayAlert("Info", AppResource.AddMessage +" "+ AddOrModifyRecord() + ".", "OK");
            ClearValues();
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
            switch (SelectedPicker)
            {
                case (int)ERecordType.Hours:
                    return new IBaseRecord[] {new HoursRecord(Date, Hours, Minutes, Price, Bonus, Description, OverTimeHours,OverTimeMinutes)};
                case (int)ERecordType.Pieces:
                    return new IBaseRecord[] {new PiecesRecord(Date, Pieces, Price, Bonus, Description)};
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
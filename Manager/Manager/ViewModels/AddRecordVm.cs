using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Manager.Annotations;
using Manager.Model;
using Manager.Model.Enums;
using Manager.Model.Interfaces;
using Xamarin.Forms;

namespace Manager.ViewModels
{
    public class AddRecordVm :INotifyPropertyChanged
    {
        private DateTime _date;
        private DateTime _dateTo;
        private uint _bonus;
        private uint _hours;
        private uint _minutes;
        private uint _price;
        private uint _pieces;
        private int _selectedPicker;
        private TableItemUcVm _modifying;
        public ICommand ButtonAdd { get; }
        public event PropertyChangedEventHandler PropertyChanged;

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
                    Application.Current.MainPage.DisplayAlert("Wrong date", "Date to is earlier than the date from.",
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

        public uint Bonus
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

        
        public uint Price
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

        public ObservableCollection<string> PickerRecordTypes { get; } = new ObservableCollection<string>(Enum.GetNames(typeof(ERecordType)));


        public AddRecordVm()
        {
            _date = DateTime.Now;
            _dateTo = DateTime.Now;
            ButtonAdd = new Command(AddButtonCommand);
            MessagingCenter.Subscribe<TableItemUcVm>(this, "ModifyItem", (modifer) =>
            {
                _modifying = modifer;
            });
        }

        private void AddButtonCommand()
        {
            if (_modifying == null)
            {
                AddRecords();
            }
            else
            {
                MessagingCenter.Send(_modifying, "Modify",
                    new TableItemUcVm(new HoursRecord(Date, Hours, Minutes, Price, Bonus)));
                _modifying = null;
            }
        }

        private void AddRecords()
        {
            switch (SelectedPicker)
            {
                case (int)ERecordType.Hours:
                    CallAddMethod(new HoursRecord(Date, Hours, Minutes, Price, Bonus));
                    break;
                case (int)ERecordType.Pieces:
                    CallAddMethod(new PiecesRecord(Date,Pieces,Price,Bonus));
                    break;
                default:
                    AddVacationRecord();
                    break;
            }
        }

        private void CallAddMethod(IBaseRecord rec)
        {
            MessagingCenter.Send(new TableItemUcVm(rec), "Add");
        }

        private void AddVacationRecord()
        {
            DateTime tmpDate = Date;
            while (tmpDate <= DateTo)
            {
                CallAddMethod(new VacationRecord(tmpDate));
                tmpDate = tmpDate.AddDays(1);
            }
        }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
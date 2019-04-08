using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Manager.Annotations;
using Manager.Model;
using Manager.Model.Enums;
using Manager.Model.Interfaces;
using Manager.SaveManagement;
using Xamarin.Forms;

namespace Manager.ViewModels
{
    public class AddRecordVm :INotifyPropertyChanged
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
        private bool _isOverTime;
        public ICommand ButtonAdd { get; }
        public event PropertyChangedEventHandler PropertyChanged;

        public bool IsOverTime
        {
            get => _isOverTime;
            set
            {
                _isOverTime = value;
                OnPropertyChanged(nameof(IsOverTime));
            }
        }

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

        public ObservableCollection<string> PickerRecordTypes { get; } = new ObservableCollection<string>(Enum.GetNames(typeof(ERecordType)).Where(s=>s != "None"));


        public AddRecordVm()
        {
            ButtonText = "Add";
            _date = DateTime.Now;
            _dateTo = DateTime.Now;
            ButtonAdd = new Command(AddButtonCommand);

            MessagingCenter.Subscribe<TableItemUcVm>(this, "ModifyItem", (modifer) =>
            {
                _modifying = modifer;
                if (modifer != null)
                {
                    if (modifer.Record.Type != ERecordType.None)
                    {
                        Date = modifer.Record.Date;
                        if (modifer.Record.Type == ERecordType.Vacation)
                        {
                            SelectedPicker = (int) ERecordType.Vacation;
                            return;
                        }

                        if (modifer.Record.Type == ERecordType.Hours)
                        {
                            Hours = ((IHoursRecord) modifer.Record).Time.Hours;
                            Minutes = ((IHoursRecord) modifer.Record).Time.Minutes;
                            SelectedPicker = (int) ERecordType.Hours;
                        }
                        else if (modifer.Record.Type == ERecordType.Pieces)
                        {
                            Pieces = ((IPiecesRecord) modifer.Record).Pieces;
                            SelectedPicker = (int) ERecordType.Pieces;
                        }

                        Price = ((IRecord) modifer.Record).Price;
                        Bonus = ((IRecord) modifer.Record).Bonus;
                        ButtonText = "Modify";
                    }
                    else
                        ButtonText = "Add";
                }
            });
        }

        private void AddButtonCommand()
        {
            string str = "";
            if (_modifying == null)
            {
                str = "added";
                AddRecords();
            }
            else
            {
                str = "modified";
                MessagingCenter.Send(_modifying, "Modify",
                    new TableItemUcVm(new HoursRecord(Date, Hours, Minutes, Price, Bonus, Description, IsOverTime)));
                _modifying = null;
            }
            Application.Current.MainPage.DisplayAlert("Info", "Record has been "+str+".", "OK");
        }


        private void AddRecords()
        {
            IBaseRecord rec;
            switch (SelectedPicker)
            {
                case (int)ERecordType.Hours:
                    rec = new HoursRecord(Date, Hours, Minutes, Price, Bonus, Description, IsOverTime);
                    CallAddMethod(rec);
                    break;
                case (int)ERecordType.Pieces:
                    rec = new PiecesRecord(Date, Pieces, Price, Bonus, Description, IsOverTime);
                    CallAddMethod(rec);
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
                IBaseRecord rec = new VacationRecord(tmpDate, Description);
                CallAddMethod(rec);
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
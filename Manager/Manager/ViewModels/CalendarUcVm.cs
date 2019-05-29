using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using Manager.Annotations;
using Manager.Model;
using Manager.Resources;

namespace Manager.ViewModels
{
    public class CalendarUcVm:INotifyPropertyChanged
    {
        private int _year;
        private double _totalPrice;
        private uint _totalDays;
        private uint _totalPieces;
        private WorkTime _totalTime;
        private double _totalBonus;
        private uint _vacationDays;
        private int _month;
        public List<string> MonthSelect { get; }
        public int SelectedMonth
        {
            get => _month;
            set
            {
                _month = value;
                OnPropertyChanged(nameof(SelectedMonth));
            }
        }

        public int Year
        {
            get => _year;
            set
            {
                _year = value;
                OnPropertyChanged(nameof(Year));
            }
        }

        public uint TotalDays
        {
            get => _totalDays;
            set
            {
                _totalDays = value;
                OnPropertyChanged(nameof(TotalDays));
            }
        }

        public double TotalPrice
        {
            get => _totalPrice;
            set
            {
                _totalPrice = value;
                OnPropertyChanged(nameof(TotalPrice));
            }
        }

        public uint TotalPieces
        {
            get => _totalPieces;
            set
            {
                _totalPieces = value;
                OnPropertyChanged(nameof(TotalPieces));
            }
        }

        public WorkTime TotalTime
        {
            get => _totalTime;
            set
            {
                _totalTime = value;
                OnPropertyChanged(nameof(TimeToString));
            }
        }

        public double TotalBonus
        {
            get => _totalBonus;
            set
            {
                _totalBonus = value;
                OnPropertyChanged(nameof(TotalBonus));
            }
        }

        public uint VacationDays
        {
            get => _vacationDays;
            set
            {
                _vacationDays = value;
                OnPropertyChanged(nameof(VacationDays));
            }
        }

        public string TimeToString => _totalTime.ToString();

        public CalendarUcVm()
        {
            MonthSelect = new List<string>()
            {
                AppResource.January,AppResource.February,
                AppResource.March,AppResource.April,AppResource.May,
                AppResource.June,AppResource.July,AppResource.August,
                AppResource.September,AppResource.October,AppResource.November,
                AppResource.December
            };
            SelectedMonth = DateTime.Now.Month - 1;
            Year = DateTime.Now.Year;
            ClearStats();
        }

        public void ClearStats()
        {
            TotalDays = 0;
            TotalPrice = 0;
            TotalPieces = 0;
            TotalTime = new WorkTime(0,0);
            TotalBonus = 0;
            TotalDays = 0;
            VacationDays = 0;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
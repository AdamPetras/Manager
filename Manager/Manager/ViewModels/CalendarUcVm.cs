using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using Manager.Annotations;
using Manager.Resources;

namespace Manager.ViewModels
{
    public class CalendarUcVm:INotifyPropertyChanged
    {
        private int year;
        public List<string> MonthSelect { get; }
        public int SelectedIndex { get; }

        public int Year
        {
            get => year;
            set
            {
                year = value;
                OnPropertyChanged(nameof(Year));
            }
        }

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
            SelectedIndex = DateTime.Now.Month - 1;
            Year = DateTime.Now.Year;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
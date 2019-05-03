using System;
using System.Collections.Generic;
using System.Linq;
using Manager.Resources;

namespace Manager.ViewModels
{
    public class CalendarUcVm
    {
        public List<string> MonthSelect { get; }
        public int SelectedIndex { get; }

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
        }

    }
}
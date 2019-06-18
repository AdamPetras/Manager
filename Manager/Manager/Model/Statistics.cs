using System;
using System.Collections.Generic;
using Manager.ViewModels;

namespace Manager.Model
{
    public class Statistics
    {
        private static Tuple<DateTime, DateTime> GetStartAndEndOfTheWeek(DateTime now)
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

        public static void Week(DateTime startDate, IReadOnlyCollection<TableItemUcVm> records, Action<TableItemUcVm> action)
        {   
            Tuple<DateTime, DateTime> dates = GetStartAndEndOfTheWeek(startDate);
            foreach (TableItemUcVm rec in records)
            {
                if ((rec.Record.Date > dates.Item1) && (rec.Record.Date < dates.Item2))
                {
                    action(rec);
                }
            }
        }

        public static void Month(IReadOnlyCollection<TableItemUcVm> records, int month,Action<TableItemUcVm> action)
        {
            foreach (TableItemUcVm rec in records)
            {
                if (rec.Record.Date.Month == month && rec.Record.Date.Year == DateTime.Today.Year)
                {
                    action(rec);
                }
            }
        }
        public static void Year(IReadOnlyCollection<TableItemUcVm> records, Action<TableItemUcVm> action)
        {
            foreach (TableItemUcVm rec in records)
            {
                if (rec.Record.Date.Year == DateTime.Today.Year)
                {
                    action(rec);
                }
            }
        }
    }
}
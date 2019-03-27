using System;
using Manager.Extensions;
using Manager.Model.Enums;
using Manager.Model.Interfaces;

namespace Manager.Model
{
    public class VacationRecord : IBaseRecord
    {
        public DateTime Date { get; set; }
        public ERecordType Type { get; set; }
        public string DateString => Date.GetDateWithoutTime();
        public string TotalPrice => "VACATION";

        public VacationRecord(DateTime date)
        {
            Date = date;
            Type = ERecordType.Vacation;
        }
    }
}
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
        public string Description { get; set; }

        public VacationRecord(DateTime date, string description)
        {
            Date = date;
            Description = description;
            Type = ERecordType.Vacation;
        }
    }
}
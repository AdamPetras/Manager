using System;
using Manager.Model.Enums;
using Manager.Model.Interfaces;

namespace Manager.Model
{
    public class NoneRecord:IBaseRecord
    {
        public Guid Id { get; set; }
        public DateTime Date { get; set; }
        public string DateString { get; }
        public ERecordType Type { get; set; }
        public string TotalPrice { get; }
        public string Description { get; set; }
        public string Value { get; }
        public string GetRecordType { get; }

        public NoneRecord()
        {
            DateString = "";
            TotalPrice = "";
            Value = "";
            Type = ERecordType.None;
            GetRecordType = "None";
        }
    }
}
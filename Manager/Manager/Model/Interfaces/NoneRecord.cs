using System;
using Manager.Model.Enums;

namespace Manager.Model.Interfaces
{
    public class NoneRecord:IBaseRecord
    {
        public Guid Id { get; set; }
        public DateTime Date { get; set; }
        public string DateString { get; }
        public ERecordType Type { get; set; }
        public string TotalPrice { get; }
        public string Description { get; set; }

        public NoneRecord()
        {
            Type = ERecordType.None;
        }
    }
}
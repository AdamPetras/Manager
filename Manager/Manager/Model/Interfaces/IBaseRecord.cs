using System;
using Manager.Model.Enums;

namespace Manager.Model.Interfaces
{
    public interface IBaseRecord
    {
        DateTime Date { get; set; }
        string DateString { get; }
        ERecordType Type { get; set; }
        string TotalPrice { get; }

    }
}

using System;
using Manager.Model.Enums;

namespace Manager.Model.Interfaces
{
    public interface IBaseRecord
    {
        Guid Id { get; set; }
        DateTime Date { get; set; }
        string DateString { get; }
        ERecordType Type { get; set; }
        string TotalPrice { get; }
        string Description { get; set; }
        string GetRecordType { get; }
    }
}

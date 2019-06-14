using System;

namespace Manager.Model.Interfaces
{
    public interface IHoursRecord: IRecord
    {
        TimeSpan WorkTimeFrom { get; set; }
        TimeSpan WorkTimeTo { get; set; }
        WorkTime Time { get; set; }
        WorkTime OverTime { get; set; }
    }
}
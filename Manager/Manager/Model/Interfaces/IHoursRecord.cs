namespace Manager.Model.Interfaces
{
    public interface IHoursRecord: IRecord
    {
        WorkTime Time { get; set; }
        WorkTime OverTime { get; set; }
    }
}
namespace Manager.Model.Interfaces
{
    public interface IRecord: IBaseRecord
    {
        double Bonus { get; set; }
        double Price { get; set; }
        bool IsOverTime { get; set; }
    }
}
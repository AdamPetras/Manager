namespace Manager.Model
{
    public class WorkTime
    {
        public uint Hours { get; set; }
        public uint Minutes { get; set; }

        public WorkTime()
        {
            Hours = 0;
            Minutes = 0;
        }

        public WorkTime(uint hours, uint minutes)
        {
            Hours = hours;
            Minutes = minutes;
        }

        public static WorkTime operator +(WorkTime b, WorkTime c)
        {
            return new WorkTime
            {
                Minutes = (b.Minutes + c.Minutes) % 60,
                Hours = b.Hours + c.Hours + (b.Minutes + c.Minutes) / 60
            };
        }

        public override string ToString()
        {
            return (Hours <= 9 ? "0" + Hours : Hours.ToString()) + ":" + (Minutes <= 9 ? "0" + Minutes : Minutes.ToString());
        }
    }
}
using System;

namespace Manager.Model
{
    public class WorkTime
    {
        public int Days { get; set; }
        public int Hours { get; set; }
        public int Minutes { get; set; }

        public WorkTime()
        {
            Hours = 0;
            Minutes = 0;
            Days = 0;
        }

        public WorkTime(int hours, int minutes,int days = 0)
        {
            Hours = hours;
            Minutes = minutes;
            Days = days;
        }

        public static WorkTime operator -(WorkTime b, WorkTime c)
        {
            int minutes = b.Minutes - c.Minutes;
            int hours = b.Hours - c.Hours;
            if (minutes < 0)
            {
                minutes += 60;
                hours--;
            }
            if (hours < 0)
            {
                hours = 0;
            }
            return new WorkTime(hours,minutes);
        }

        public static WorkTime operator +(WorkTime b, WorkTime c)
        {
            return new WorkTime
            {
                Minutes = (b.Minutes + c.Minutes) % 60,
                Hours = b.Hours + c.Hours + (b.Minutes + c.Minutes) / 60
            };
        }

        public static bool operator ==(WorkTime a, WorkTime b)
        {
            if (ReferenceEquals(a, b))
            {
                return true;
            }
            if (ReferenceEquals(a, null))
            {
                return false;
            }
            if (ReferenceEquals(b, null))
            {
                return false;
            }
            return a.Hours == b.Hours && a.Minutes == b.Minutes;
        }

        public static bool operator !=(WorkTime a, WorkTime b)
        {
            return !(a == b);
        }


        public override string ToString()
        {
            return (Hours <= 9 ? "0" + Hours : Hours.ToString()) + ":" + (Minutes <= 9 ? "0" + Minutes : Minutes.ToString());
        }
        protected bool Equals(WorkTime other)
        {
            return Hours == other.Hours && Minutes == other.Minutes;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((WorkTime)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (Hours * 397) ^ Minutes;
            }
        }

        public TimeSpan ToTimeSpan()
        { 
            return new TimeSpan(Days,Hours, Minutes,0);
        }
    }
}
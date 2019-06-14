using System;

namespace Manager.Model
{
    public class WorkTime
    {
        public int Hours { get; set; }
        public int Minutes { get; set; }

        public WorkTime()
        {
            Hours = 0;
            Minutes = 0;
        }

        public WorkTime(int hours, int minutes)
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
            if (obj.GetType() != this.GetType()) return false;
            return Equals((WorkTime)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((int)Hours * 397) ^ (int)Minutes;
            }
        }

        public TimeSpan ToTimeSpan()
        { 
            return new TimeSpan(Hours,Minutes,0);
        }
    }
}
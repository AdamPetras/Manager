using System;
using System.Globalization;
using Manager.Extensions;
using Manager.Model.Enums;
using Manager.Model.Interfaces;
using Manager.Resources;

namespace Manager.Model
{
    public class HoursRecord : IHoursRecord
    { 
        public Guid Id { get; set; }
        public DateTime Date { get; set; }
        public double Bonus { get; set; }
        public double Price { get; set; }
        public string DateString => Date.GetDateWithoutTime();
        public ERecordType Type { get; set; }
        public string TotalPrice => Math.Round((Time.Hours * Price + Time.Minutes / 60.0 * Price) + (OverTime.Hours * Price + OverTime.Minutes / 60.0 * Price)+Bonus,1).ToString(CultureInfo.InvariantCulture);
        public string Description { get; set; }
        public string Value { get; }
        public string GetRecordType { get; }
        public TimeSpan WorkTimeFrom { get; set; }
        public TimeSpan WorkTimeTo { get; set; }
        public WorkTime Time { get; set; }
        public WorkTime BreakTime { get; set; }
        public WorkTime OverTime { get; set; }


        public HoursRecord(DateTime date, TimeSpan from, TimeSpan to, double price, double bonus, string description,int overTimeHours,int overTimeMinutes, WorkTime breakTime)
        {
            Id = Guid.NewGuid();
            Date = date;
            WorkTimeFrom = from;
            WorkTimeTo = to;
            (int hours,int minutes) = CalculateHoursAndMinutes();
            BreakTime = breakTime;
            Time = new WorkTime(hours, minutes);
            Time -= BreakTime;
            Price = price;
            Bonus = bonus;
            Description = description ?? "";
            OverTime = new WorkTime(overTimeHours,overTimeMinutes);
            Type = ERecordType.Hours;
            GetRecordType = AppResource.HoursType;
            Value = (Time + OverTime).ToString();
        }

        public HoursRecord(DateTime date, TimeSpan from, TimeSpan to, double price, double bonus, string description, WorkTime overTime, WorkTime breakTime) :this(date,from,to,price,bonus,description,overTime.Hours,overTime.Minutes,breakTime)
        {
        }

        private Tuple<int, int> CalculateHoursAndMinutes()
        {
            TimeSpan span = WorkTimeTo - WorkTimeFrom;
            return new Tuple<int, int>(span.Hours, span.Minutes);
        }

        public static bool operator ==(HoursRecord obj1 , HoursRecord obj2)
        {
            if (ReferenceEquals(obj1, obj2))
            {
                return true;
            }

            if (ReferenceEquals(obj1, null))
            {
                return false;
            }
            if (ReferenceEquals(obj2, null))
            {
                return false;
            }
            return obj1.Type == obj2.Type && obj1.Time == obj2.Time
                                          && IsDateEqual(obj1,obj2)
                                            && obj1.Description == obj2.Description
                                          && obj1.TotalPrice == obj2.TotalPrice;
        }

        private static bool IsDateEqual(HoursRecord obj1, HoursRecord obj2)
        {
            return obj1.Date.Day == obj2.Date.Day && obj1.Date.Month == obj2.Date.Month &&
                   obj1.Date.Year == obj2.Date.Year;
        }

        public static bool operator !=(HoursRecord a, HoursRecord b)
        {
            return !(a == b);
        }
        protected bool Equals(HoursRecord other)
        {
            return Id.Equals(other.Id) && Date.Equals(other.Date) && Bonus.Equals(other.Bonus) && Price.Equals(other.Price) && Type == other.Type && string.Equals(Description, other.Description) && string.Equals(Value, other.Value) && string.Equals(GetRecordType, other.GetRecordType) && WorkTimeFrom.Equals(other.WorkTimeFrom) && WorkTimeTo.Equals(other.WorkTimeTo) && Equals(Time, other.Time) && Equals(BreakTime, other.BreakTime) && Equals(OverTime, other.OverTime);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((HoursRecord)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = Id.GetHashCode();
                hashCode = (hashCode * 397) ^ Date.GetHashCode();
                hashCode = (hashCode * 397) ^ Bonus.GetHashCode();
                hashCode = (hashCode * 397) ^ Price.GetHashCode();
                hashCode = (hashCode * 397) ^ (int)Type;
                hashCode = (hashCode * 397) ^ (Description != null ? Description.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Value != null ? Value.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (GetRecordType != null ? GetRecordType.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ WorkTimeFrom.GetHashCode();
                hashCode = (hashCode * 397) ^ WorkTimeTo.GetHashCode();
                hashCode = (hashCode * 397) ^ (Time != null ? Time.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (BreakTime != null ? BreakTime.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (OverTime != null ? OverTime.GetHashCode() : 0);
                return hashCode;
            }
        }
    }
}
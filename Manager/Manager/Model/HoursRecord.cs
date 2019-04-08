using System;
using System.Globalization;
using Manager.Annotations;
using Manager.Extensions;
using Manager.Model.Enums;
using Manager.Model.Interfaces;

namespace Manager.Model
{
    public class HoursRecord : IHoursRecord
    {
        public Guid Id { get; set; }
        public DateTime Date { get; set; }
        public double Bonus { get; set; }
        public double Price { get; set; }
        public bool IsOverTime { get; set; }
        public string DateString => Date.GetDateWithoutTime();
        public ERecordType Type { get; set; }
        public string TotalPrice => Math.Round(Time.Hours * Price + Time.Minutes / 60.0 * Price + Bonus,1).ToString(CultureInfo.InvariantCulture);
        public string Description { get; set; }
        public WorkTime Time { get; set; }



        public HoursRecord(DateTime date, uint hours,uint minutes, double price, double bonus, string description,bool isOverTime)
        {
            Id = Guid.NewGuid();
            Date = date;
            Time = new WorkTime(hours, minutes);
            Price = price;
            Bonus = bonus;
            Description = description;
            IsOverTime = isOverTime;
            Type = ERecordType.Hours;
        }
        public HoursRecord(DateTime date, WorkTime time, double price, double bonus, string description, bool isOverTime) :this(date,time.Hours,time.Minutes,price,bonus, description,isOverTime)
        {
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
                                          && obj1.Date == obj2.Date && obj1.Description == obj2.Description
                                          && obj1.TotalPrice == obj2.TotalPrice;
        }

        public static bool operator !=(HoursRecord a, HoursRecord b)
        {
            return !(a == b);
        }
        protected bool Equals(HoursRecord other)
        {
            return Id.Equals(other.Id) && Date.Equals(other.Date) && Bonus.Equals(other.Bonus) && Price.Equals(other.Price) && IsOverTime == other.IsOverTime && Type == other.Type && string.Equals(Description, other.Description) && Equals(Time, other.Time);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
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
                hashCode = (hashCode * 397) ^ IsOverTime.GetHashCode();
                hashCode = (hashCode * 397) ^ (int)Type;
                hashCode = (hashCode * 397) ^ (Description != null ? Description.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Time != null ? Time.GetHashCode() : 0);
                return hashCode;
            }
        }
    }
}
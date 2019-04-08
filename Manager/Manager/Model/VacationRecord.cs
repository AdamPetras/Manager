using System;
using Manager.Extensions;
using Manager.Model.Enums;
using Manager.Model.Interfaces;

namespace Manager.Model
{
    public class VacationRecord : IBaseRecord
    {
        public Guid Id { get; set; }
        public DateTime Date { get; set; }
        public ERecordType Type { get; set; }
        public string DateString => Date.GetDateWithoutTime();
        public string TotalPrice => "VACATION";
        public string Description { get; set; }

        public VacationRecord(DateTime date, string description)
        {
            Id = Guid.NewGuid();
            Date = date;
            Description = description;
            Type = ERecordType.Vacation;
        }

        public static bool operator ==(VacationRecord obj1, VacationRecord obj2)
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
            return obj1.Type == obj2.Type && obj1.Date == obj2.Date 
                                          && obj1.Description == obj2.Description
                                          && obj1.TotalPrice == obj2.TotalPrice;
        }

        public static bool operator !=(VacationRecord a, VacationRecord b)
        {
            return !(a == b);
        }
        protected bool Equals(VacationRecord other)
        {
            return Id.Equals(other.Id) && Date.Equals(other.Date) && Type == other.Type && string.Equals(Description, other.Description);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((VacationRecord)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = Id.GetHashCode();
                hashCode = (hashCode * 397) ^ Date.GetHashCode();
                hashCode = (hashCode * 397) ^ (int)Type;
                hashCode = (hashCode * 397) ^ (Description != null ? Description.GetHashCode() : 0);
                return hashCode;
            }
        }
    }
}
using System;
using System.Globalization;
using Manager.Extensions;
using Manager.Model.Enums;
using Manager.Model.Interfaces;
using Manager.Resources;

namespace Manager.Model
{
    public class PiecesRecord:IPiecesRecord
    {
        public Guid Id { get; set; }
        public DateTime Date { get; set; }
        public double Bonus { get; set; }
        public double Price { get; set; }
        public string DateString => Date.GetDateWithoutTime();
        public ERecordType Type { get; set; }
        public string TotalPrice => Math.Round(Pieces * Price + Bonus,1).ToString(CultureInfo.InvariantCulture);
        public string Description { get; set; }
        public string Value { get; }
        public string GetRecordType { get; }

        public uint Pieces { get; set; }

        public PiecesRecord(DateTime date,uint pieces, double price, double bonus,string description)
        {
            Id = Guid.NewGuid();
            Date = date;
            Pieces = pieces;
            Price = price;
            Bonus = bonus;
            Description = description ?? "";
            Type = ERecordType.Pieces;
            GetRecordType = AppResource.PiecesType;
            Value = Pieces.ToString();
        }

        public static bool operator ==(PiecesRecord obj1, PiecesRecord obj2)
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
            return obj1.Type == obj2.Type && obj1.Pieces == obj2.Pieces
                                          && obj1.Date == obj2.Date && obj1.Description == obj2.Description
                                          && obj1.TotalPrice == obj2.TotalPrice;
        }

        public static bool operator !=(PiecesRecord a, PiecesRecord b)
        {
            return !(a == b);
        }
        protected bool Equals(PiecesRecord other)
        {
            return Id.Equals(other.Id) && Date.Equals(other.Date) && Bonus.Equals(other.Bonus) && Price.Equals(other.Price) && Type == other.Type && string.Equals(Description, other.Description) && Pieces == other.Pieces;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((PiecesRecord)obj);
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
                hashCode = (hashCode * 397) ^ (int)Pieces;
                return hashCode;
            }
        }
    }
}
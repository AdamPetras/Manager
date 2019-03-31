using System;
using System.Globalization;
using Manager.Extensions;
using Manager.Model.Enums;
using Manager.Model.Interfaces;

namespace Manager.Model
{
    public class PiecesRecord:IPiecesRecord
    {
        public DateTime Date { get; set; }
        public double Bonus { get; set; }
        public double Price { get; set; }
        public bool IsOverTime { get; set; }
        public string DateString => Date.GetDateWithoutTime();
        public ERecordType Type { get; set; }
        public string TotalPrice => Math.Round(Pieces * Price + Bonus,1).ToString(CultureInfo.InvariantCulture);
        public string Description { get; set; }

        public uint Pieces { get; set; }

        public PiecesRecord(DateTime date,uint pieces, double price, double bonus,string description,bool isOverTime)
        {
            Date = date;
            Pieces = pieces;
            Price = price;
            Bonus = bonus;
            Description = description;
            IsOverTime = isOverTime;
            Type = ERecordType.Pieces;
        }
    }
}
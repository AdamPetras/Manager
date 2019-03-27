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
        public string DateString => Date.GetDateWithoutTime();
        public ERecordType Type { get; set; }
        public string TotalPrice => Math.Round(Pieces * Price + Bonus,1).ToString(CultureInfo.InvariantCulture);

        public uint Pieces { get; set; }

        public PiecesRecord(DateTime date,uint pieces, double price, double bonus)
        {
            Date = date;
            Pieces = pieces;
            Price = price;
            Bonus = bonus;
            Type = ERecordType.Pieces;
        }
    }
}
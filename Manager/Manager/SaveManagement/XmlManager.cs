using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using Manager.Model;
using Manager.Model.Enums;
using Manager.Model.Interfaces;

namespace Manager.SaveManagement
{
    public class XmlManager : XmlBase, IXmlManager
    {
        private readonly XmlDocument _document;
        private readonly string _path;

        public XmlManager(string path)
        {
            _document = new XmlDocument();
            _path = path;
        }

        public void CreateNewXmlFile(IReadOnlyCollection<IBaseRecord> recordList)
        {
            if (File.Exists(_path))
                File.Delete(_path);
            using (XmlWriter writer = XmlWriter.Create(_path))
            {
                CreateRootElement(writer);
            }
            _document.Load(_path);
            foreach (IBaseRecord tmp in recordList)
            {
                _document.DocumentElement?.AppendChild(CreateRecordElement(tmp));
            }
            try
            {
                _document.Save(_path);
            }
            catch (XmlException e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public void CreateNewXmlFile(params IBaseRecord[] records)
        {
            CreateNewXmlFile(records.ToList());
        }

        public void RemoveXmlRecord(IBaseRecord rec)
        {
            XElement xElement = XElement.Load(_path);
            FindElementByRecord(xElement.Elements(), rec)?.Remove();
            xElement.Save(_path);
        }

        public void AppendXmlFile(IBaseRecord rec)
        {
            try
            {
                _document.Load(_path);
            }
            catch (XmlException)
            {
                //TODO DIALOG
            }
            catch (FileNotFoundException)
            {
                using (XmlWriter writer = XmlWriter.Create(_path))
                {
                    CreateRootElement(writer);
                }
                _document.Load(_path);
            }
            catch (IOException)
            {
                return;
                //TODO DIALOG
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
            _document.DocumentElement?.AppendChild(CreateRecordElement(rec));
            try
            {
                _document.Save(_path);
            }
            catch (XmlException e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public void EditXmlRecord(IBaseRecord oldRec, IBaseRecord newRecord)
        {
            XElement root = XElement.Load(_path);
            XElement el = FindElementByRecord(root.Elements(), oldRec);
            if (el == null)
                return;
            XElement newEl = XElement.Parse(CreateRecordElement(newRecord).OuterXml);
            el.ReplaceWith(
                newEl
            );
            root.Save(_path);
        }

        private bool IfFoundedElementIsEquals(IBaseRecord rec, IBaseRecord foundRec)
        {
            switch (rec.Type)
            {
                case ERecordType.Hours:
                    return (HoursRecord)foundRec == (HoursRecord)rec;
                case ERecordType.Pieces:
                    return (PiecesRecord)foundRec == (PiecesRecord)rec;
                case ERecordType.Vacation:
                    return (VacationRecord)foundRec == (VacationRecord)rec;
            }
            return false;
        }

        

        private XElement FindElementByRecord(IEnumerable<XElement> records, IBaseRecord rec)
        {
            foreach (XElement recordElement in records)
            {
                IBaseRecord foundRec = ParsePiecesAndTimeRecord(recordElement);
                if (rec.Type == foundRec.Type)
                {
                    if (IfFoundedElementIsEquals(rec, foundRec))
                        return recordElement;
                }
            }
            return null;
        }

        private void CreateRootElement(XmlWriter writer)
        {
            writer.WriteStartDocument();
            writer.WriteStartElement("RecordList");
            writer.WriteEndElement();
            writer.WriteEndDocument();
        }

        private XmlElement CreateRecordElement(IBaseRecord rec)
        {
            XmlElement record = _document.CreateElement("Record");
            record.SetAttribute("Type", rec.Type.ToString());
            XmlElement data = _document.CreateElement("Data");
            record.AppendChild(data);
            data.SetAttribute("Date", rec.Date.ToString(CultureInfo.InvariantCulture));
            if (rec.Description == null)
                rec.Description = "";
            data.SetAttribute("Description", rec.Description);
            switch (rec.Type)
            {
                case ERecordType.Hours:
                    data.SetAttribute("WorkTimeFrom", ((IHoursRecord) rec).WorkTimeFrom.ToString());
                    data.SetAttribute("WorkTimeTo", ((IHoursRecord)rec).WorkTimeTo.ToString());
                    data.SetAttribute("OverTime", ((IHoursRecord)rec).OverTime.ToString());
                    break;
                case ERecordType.Pieces:
                    data.SetAttribute("Variable", ((IPiecesRecord)rec).Pieces.ToString());
                    break;
            }
            if (rec.Type != ERecordType.Vacation && rec.Type != ERecordType.None)
            {
                IRecord baseRec = (IRecord)rec;
                data.SetAttribute("Price", baseRec.Price.ToString(CultureInfo.InvariantCulture));
                data.SetAttribute("Bonus", baseRec.Bonus.ToString(CultureInfo.InvariantCulture));
            }
            return record;
        }

        public List<IBaseRecord> LoadXmlFile()
        {
            List<IBaseRecord> recList = new List<IBaseRecord>();
            if (!File.Exists(_path))
            {
                return recList;
            }
            IEnumerable<XElement> records = XElement.Load(_path).Elements();
            foreach (XElement record in records)
            {
                recList.Add(ParsePiecesAndTimeRecord(record));
            }
            return recList;
        }

        private IBaseRecord ParsePiecesAndTimeRecord(XElement recordElement)
        {
            IBaseRecord rec = null;
            Enum.TryParse(recordElement.Attribute("Type")?.Value, out ERecordType recType);
            XElement dataElement = recordElement.Element("Data");
            if (dataElement != null)
            {
                DateTime date = ParseDate(dataElement.Attribute("Date")?.Value);
                string description = dataElement.Attribute("Description")?.Value;
                var (price, bonus) = ParseIRecord(recType, dataElement.Attribute("Price")?.Value, dataElement.Attribute("Bonus")?.Value);
                switch (recType)
                {
                    case ERecordType.Hours:
                        TimeSpan workTimeFrom = ParseTimeRecord(dataElement.Attribute("WorkTimeFrom")?.Value.Split(':')).ToTimeSpan();
                        TimeSpan workTimeTo = ParseTimeRecord(dataElement.Attribute("WorkTimeTo")?.Value.Split(':')).ToTimeSpan();
                        WorkTime overTime = ParseTimeRecord(dataElement.Attribute("OverTime")?.Value.Split(':'));
                        rec = new HoursRecord(date, workTimeFrom, workTimeTo, price,bonus, description, overTime);
                        break;
                    case ERecordType.Pieces:
                        uint.TryParse(dataElement.Attribute("Variable")?.Value, out uint pieces);
                        rec = new PiecesRecord(date, pieces, price, bonus, description);
                        break;
                    case ERecordType.Vacation:
                        rec = new VacationRecord(date, description);
                        break;
                }
            }
            return rec;
        }


        private Tuple<double, double> ParseIRecord(ERecordType recType, string priceString, string bonusString)
        {
            if (recType != ERecordType.Vacation && recType != ERecordType.None)
            {
                double.TryParse(priceString.Replace(',', '.'), NumberStyles.Any, CultureInfo.InvariantCulture, out double price);
                double.TryParse(bonusString.Replace(',', '.'), NumberStyles.Any, CultureInfo.InvariantCulture, out double bonus);
                return new Tuple<double, double>(price, bonus);
            }
            return new Tuple<double, double>(0, 0);
        }

        private DateTime ParseDate(string dateString)
        {
            DateTime date = DateTime.Today;
            try
            {
                date = DateTime.Parse(dateString, new CultureInfo("en"));
            }
            catch (FormatException)
            {

            }
            return date;
        }

        private WorkTime ParseTimeRecord(string[] timeString)
        {
            int.TryParse(timeString[0], out int hours);
            int.TryParse(timeString[1], out int minutes);
            return new WorkTime(hours, minutes);
        }
    }
}
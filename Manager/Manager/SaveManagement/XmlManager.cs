using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Xml;
using System.Xml.Linq;
using Manager.Model;
using Manager.Model.Enums;
using Manager.Model.Interfaces;

namespace Manager.SaveManagement
{
    public class XmlManager : IXmlManager
    {
        private XmlDocument _document;
        private string path;

        public XmlManager(string path)
        {
            _document = new XmlDocument();
            this.path = path;
        }

        public void CreateNewXmlFile(IReadOnlyCollection<IBaseRecord> recordList)
        {
            if (File.Exists(path))
                File.Delete(path);
            using (XmlWriter writer = XmlWriter.Create(path))
            {
                CreateRootElement(writer);
            }
            foreach (IBaseRecord tmp in recordList)
            {
                _document.DocumentElement?.AppendChild(CreateRecordElement(tmp));
            }
            try
            {
                _document.Save(path);
            }
            catch (XmlException e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public void AppendXmlFile(IBaseRecord rec)
        {
            try
            {
                _document.Load(path);
            }
            catch (XmlException)
            {
                //TODO DIALOG
            }
            catch (FileNotFoundException)
            {
                using (XmlWriter writer = XmlWriter.Create(path))
                {
                    CreateRootElement(writer);
                }
                _document.Load(path);
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
                _document.Save(path);
            }
            catch (XmlException e)
            {
                Console.WriteLine(e);
                throw;
            }
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
            record.SetAttribute("Date", rec.Date.ToString(CultureInfo.InvariantCulture));
            record.SetAttribute("Description", rec.Description);
            switch (rec.Type)
            {
                case ERecordType.Hours:
                    record.SetAttribute("Variable", ((IHoursRecord)rec).Time.ToString());
                    break;
                case ERecordType.Pieces:
                    record.SetAttribute("Variable", ((IPiecesRecord)rec).Pieces.ToString());
                    break;
            }
            if (rec.Type != ERecordType.Vacation && rec.Type != ERecordType.None)
            {
                IRecord baseRec = (IRecord)rec;
                record.SetAttribute("Price", baseRec.Price.ToString(CultureInfo.InvariantCulture));
                record.SetAttribute("Bonus", baseRec.Bonus.ToString(CultureInfo.InvariantCulture));
                record.SetAttribute("IsOverTime", baseRec.IsOverTime.ToString());
            }
            return record;
        }

        public List<IBaseRecord> LoadXmlFile()
        {
            List<IBaseRecord> recList = new List<IBaseRecord>();
            if (!File.Exists(path))
            {
                return recList;
            }
            IEnumerable<XElement> records = XElement.Load(path).Elements();
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
                var (price, bonus, isOverTime) = ParseIRecord(recType, dataElement.Attribute("Price")?.Value, dataElement.Attribute("Bonus")?.Value,dataElement.Attribute("IsOverTime")?.Value);
                switch (recType)
                {
                    case ERecordType.Hours:
                        WorkTime workTime = ParseTimeRecord(dataElement.Attribute("Variable")?.Value.Split(':'));
                        rec = new HoursRecord(date,workTime,price,bonus, description,isOverTime);
                        break;
                    case ERecordType.Pieces:
                        uint.TryParse(dataElement.Attribute("Variable")?.Value, out uint pieces);
                        rec = new PiecesRecord(date, pieces, price, bonus, description, isOverTime);
                        break;
                    case ERecordType.Vacation:
                        rec = new VacationRecord(date, description);
                        break;
                }
            }
            return rec;
        }


        private Tuple<double, double, bool> ParseIRecord(ERecordType recType, string priceString, string bonusString, string isOverTimeString)
        {
            if (recType != ERecordType.Vacation && recType != ERecordType.None)
            {
                double.TryParse(priceString, out double price);
                double.TryParse(bonusString, out double bonus);
                bool.TryParse(isOverTimeString, out bool isOverTime);
                return new Tuple<double, double, bool>(price, bonus, isOverTime);
            }
            return new Tuple<double, double, bool>(0, 0, false);
        }

        private DateTime ParseDate(string dateString)
        {
            DateTime date = DateTime.Now;
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
            uint.TryParse(timeString[0], out uint hours);
            uint.TryParse(timeString[1], out uint minutes);
            return new WorkTime(hours, minutes);
        }

        public void WriteXmlToConsole()
        {
            using (StreamReader reader = new StreamReader(path))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    Console.WriteLine(line);
                }
            }
        }
    }
}
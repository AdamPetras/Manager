using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Linq;
using Manager.Model.Interfaces;

namespace Manager.SaveManagement
{
    public class XmlSave: XmlBase,IXmlSave
    {
        private XmlDocument _document;
        private string _path;
        public XmlSave(string path)
        {
            _document = new XmlDocument();
            _path = path;
        }

        public void CreateXmlFile(IReadOnlyCollection<SaveOption> saveList)
        {
            if (File.Exists(_path))
                File.Delete(_path);
            using (XmlWriter writer = XmlWriter.Create(_path))
            {
                CreateRootElement(writer);
            }
            _document.Load(_path);
            foreach (SaveOption tmp in saveList)
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

        private XmlElement CreateRecordElement(SaveOption rec)
        {
            XmlElement record = _document.CreateElement("Save");
            record.SetAttribute("Title", rec.Title);
            record.SetAttribute("Value", rec.Value);
            return record;
        }



        private void CreateRootElement(XmlWriter writer)
        {
            writer.WriteStartDocument();
            writer.WriteStartElement("Settings");
            writer.WriteEndElement();
            writer.WriteEndDocument();
        }

        public void EditXml(SaveOption oldOpt, SaveOption newOpt)
        {
            XElement root = XElement.Load(_path);
            XElement el = FindElement(root.Elements(), oldOpt);
            if (el == null)
                return;
            XElement newEl = XElement.Parse(CreateRecordElement(newOpt).OuterXml);
            el.ReplaceWith(
                newEl
            );
            root.Save(_path);
        }

        private XElement FindElement(IEnumerable<XElement> records, SaveOption rec)
        {
            foreach (XElement element in records)
            {
                SaveOption founded = ParseSave(element);
                if (rec.Title == founded.Title)
                {
                    return element;
                }
            }
            return null;
        }

        public List<SaveOption> LoadXmlFile()
        {
            List<SaveOption> lst = new List<SaveOption>();
            if (!File.Exists(_path))
            {
                return lst;
            }
            IEnumerable<XElement> records = XElement.Load(_path).Elements();
            foreach (XElement record in records)
            {
                lst.Add(ParseSave(record));
            }
            return lst;
        }

        private SaveOption ParseSave(XElement recordElement)
        {
            return new SaveOption(recordElement.Attribute("Title")?.Value, recordElement.Attribute("Value")?.Value);
        }
    }
}
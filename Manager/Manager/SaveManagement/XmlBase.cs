using System;
using System.IO;
using System.Xml;

namespace Manager.SaveManagement
{
    public class XmlBase:IXmlBase
    {
        public virtual void WriteXmlToConsole(string path)
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

        public virtual string XmlToString(string path)
        {
            string tmp = "";
            using (StreamReader reader = new StreamReader(path))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    tmp += line;
                }
            }
            return tmp;
        }

        public void StringToXml(string path, string content)
        {
            if (File.Exists(path))
                File.Delete(path);
            using (XmlWriter.Create(path))
            {
            }
            XmlDocument document = new XmlDocument();
            try
            {
                document.LoadXml(content);

            }
            catch (XmlException e)
            {
                Console.WriteLine(e);
                throw;
            }
            try
            {
                document.Save(path);
            }
            catch (XmlException e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
    }
}
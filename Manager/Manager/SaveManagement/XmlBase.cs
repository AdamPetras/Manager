using System;
using System.IO;

namespace Manager.SaveManagement
{
    public abstract class XmlBase:IXmlBase
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
    }
}
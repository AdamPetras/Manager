using System.Collections.Generic;

namespace Manager.SaveManagement
{
    public class XmlSave: XmlBase,IXmlSave
    {
        public XmlSave()
        {
            IXmlManager manager = new XmlManager("");
        }

        public void CreateXmlFile()
        {
        }

        public void EditXmlRecord()
        {
        }

        public List<SaveOption> LoadXmlFile()
        {
            return new List<SaveOption>();
        }
    }
}
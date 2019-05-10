using System.Collections.Generic;

namespace Manager.SaveManagement
{
    public interface IXmlSave
    {
        void CreateXmlFile();
        void EditXmlRecord();
        List<SaveOption> LoadXmlFile();
    }
}
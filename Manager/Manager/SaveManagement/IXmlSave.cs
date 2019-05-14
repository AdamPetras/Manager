using System.Collections.Generic;

namespace Manager.SaveManagement
{
    public interface IXmlSave
    {
        void CreateXmlFile(IReadOnlyCollection<SaveOption> saveList);
        void EditXml(SaveOption oldOpt, SaveOption newOpt);
        List<SaveOption> LoadXmlFile();
    }
}
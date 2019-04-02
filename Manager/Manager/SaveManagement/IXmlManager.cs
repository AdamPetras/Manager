using System.Collections.Generic;
using Manager.Model.Interfaces;

namespace Manager.SaveManagement
{
    public interface IXmlManager
    {
        void CreateNewXmlFile(IReadOnlyCollection<IBaseRecord> recordList);
        void AppendXmlFile(IBaseRecord rec);
        List<IBaseRecord> LoadXmlFile();
        void WriteXmlToConsole();
    }
}
namespace Manager.SaveManagement
{
    public interface IXmlBase
    {
        void WriteXmlToConsole(string path);
        string XmlToString(string path);
        void StringToXml(string path, string content);
    }
}
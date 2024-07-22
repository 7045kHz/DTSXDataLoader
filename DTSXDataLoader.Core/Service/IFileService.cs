using System.Xml;

namespace DTSXDataLoader.Core.Service
{
    public interface IFileService
    {
        void AddOrUpdateAppSetting<T>(string file, string key, T value);
        void AddOrUpdateAppSetting<T>(string key, T value);
        XmlNode LoadFile(string FileName);
        IEnumerable<string> GetAllFilesInDirectory(string path, string extension);
    }
}
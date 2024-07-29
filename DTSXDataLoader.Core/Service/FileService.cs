using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.VisualBasic.FileIO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.XPath;

namespace DTSXDataLoader.Core.Service
{
    public class FileService : IFileService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger _logger;

        public FileService(IConfiguration configuration, ILogger logger)
        {
            _configuration = configuration;
            _logger = logger;
        }
        public XmlNode LoadFile(string FileName)
        {
            try
            {
                XmlDocument? doc = new XmlDocument();

                doc.Load(FileName);
                XmlNode? root = doc.DocumentElement;

                XPathNavigator? nav = root?.CreateNavigator();
                return root = doc.DocumentElement;
            }
            catch (Exception e)
            {
                _logger.LogCritical($@"Cannot load XML file {e}");
                throw;
            }


        }
        public void AddOrUpdateAppSetting<T>(string file, string key, T value)
        {
            try
            {
                var filePath = Path.Combine(AppContext.BaseDirectory, file);
                string appsettingsJson = File.ReadAllText(filePath);
                if (!string.IsNullOrEmpty(appsettingsJson))
                {
                    dynamic? appsettingsJsonObject = Newtonsoft.Json.JsonConvert.DeserializeObject(appsettingsJson);
                    if (appsettingsJsonObject != null)
                    {
                        var sectionPath = key.Split(":")[0];

                        if (!string.IsNullOrEmpty(sectionPath))
                        {
                            var keyPath = key.Split(":")[1];
                            appsettingsJsonObject[sectionPath][keyPath] = value;
                        }
                        else
                        {
                            appsettingsJsonObject[sectionPath] = value; // if no sectionpath just set the value
                        }

                        string output = Newtonsoft.Json.JsonConvert.SerializeObject(appsettingsJsonObject, Newtonsoft.Json.Formatting.Indented);
                        File.WriteAllText(filePath, output);
                    }
                }

            }
            catch (Exception e)
            {
                _logger.LogCritical($@"Cannot write to file {file}  {e}");
                throw;
            }
        }
        public void AddOrUpdateAppSetting<T>(string key, T value)
        {
            try
            {
                var filePath = Path.Combine(AppContext.BaseDirectory, "appsettings.json");
                string appsettingsJson = File.ReadAllText(filePath);
                if (!string.IsNullOrEmpty(appsettingsJson))
                {
                    dynamic? appsettingsJsonObject = Newtonsoft.Json.JsonConvert.DeserializeObject(appsettingsJson);
                    if (appsettingsJsonObject != null)
                    {
                        var sectionPath = key.Split(":")[0];

                        if (!string.IsNullOrEmpty(sectionPath))
                        {
                            var keyPath = key.Split(":")[1];
                            appsettingsJsonObject[sectionPath][keyPath] = value;
                        }
                        else
                        {
                            appsettingsJsonObject[sectionPath] = value; // if no sectionpath just set the value
                        }

                        string output = Newtonsoft.Json.JsonConvert.SerializeObject(appsettingsJsonObject, Newtonsoft.Json.Formatting.Indented);
                        File.WriteAllText(filePath, output);
                    }
                }

            }
            catch (Exception e)
            {
                _logger.LogCritical($@"Cannot write to appsettings.json file {e}");
                throw;
            }
        }
        public IEnumerable<string> GetAllFilesInDirectory(string path, string extension)
        {
            IEnumerable<string> returnList = new List<string>();
            if (!string.IsNullOrEmpty(path) && !string.IsNullOrEmpty(extension))
            {
                var directoryInfo = new DirectoryInfo(path);
                return directoryInfo.GetFiles(extension, System.IO.SearchOption.TopDirectoryOnly).Select(i => i.FullName);
            }
            return returnList;
        }
         
    }
}

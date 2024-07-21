using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.XPath;

namespace DTSXDataLoader.Core.Service
{
    public class FileService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger _logger;

        public FileService(IConfiguration configuration, ILogger logger)
        {
            _configuration = configuration;
            _logger = logger;
        }
        public XmlNode  LoadFile(string FileName)
        {
            XmlDocument? doc = new XmlDocument();

            doc.Load(FileName);
            XmlNode? root = doc.DocumentElement;

            XPathNavigator? nav = root?.CreateNavigator();
            return root = doc.DocumentElement;

        }
 
    }
}

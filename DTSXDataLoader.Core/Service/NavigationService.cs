using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using DTSXDataLoaderCore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.XPath;

namespace DTSXDataLoaderCore.Service
{
    public class NavigationService : INavigationService
    {
        private readonly ILogger _logger;
       

        public NavigationService(ILogger logger)
        {
            _logger = logger;
        }
        public XmlNamespaceManager CreateNameSpaceManager(XmlNameTable NameTable)
        {
            XmlNamespaceManager xmlNamespaceManager = null ;
            if (NameTable!= null)
            {
                xmlNamespaceManager = new XmlNamespaceManager(NameTable);
                if (xmlNamespaceManager != null)
                {
                    // Add DTS Specific NameSpaces -- move this selection to appsettings
                    xmlNamespaceManager.AddNamespace("DTS", @"www.microsoft.com/SqlServer/Dts");
                    xmlNamespaceManager.AddNamespace("SQLTask", @"www.microsoft.com/sqlserver/dts/tasks/sqltask");
                }
            } else
            {
                _logger.LogCritical("CreateNameSpaceManager Failure with TableName is null ");
            }
            return xmlNamespaceManager;
        }
 
        public  XPathNavigator CreateNavigator(XmlDocument doc)
        {
            XPathNavigator? navigator = null;
            if (doc != null && doc.DocumentElement != null)
            {
                XmlNode root = doc.DocumentElement;
                if (root != null)
                {
                    navigator = root.CreateNavigator();

                }
                if(navigator != null)
                {
                    navigator.MoveToRoot();
                }

            }
            else
            {
                _logger.LogCritical("CreateNavigator Failure with XmlDocument or DocumentElement is null ");
            }
            
            return navigator;
        }
        public XmlDocument NewXmlDocument( string FileName )
        {
            XmlDocument? doc = new XmlDocument();
            if (!string.IsNullOrEmpty(FileName) && Path.Exists(FileName) )
            {
                doc.Load(FileName);
            }
            else
            {
                _logger.LogCritical($@"{FileName} is undefined or does not exist");
                
            }
            
            return doc;

        }
    }
}

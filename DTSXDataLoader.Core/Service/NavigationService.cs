using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using DTSXDataLoader.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.XPath;

namespace DTSXDataLoader.Core.Service
{
    
public class NavigationService : INavigationService
    {
        private readonly ILogger _logger;
       

        public NavigationService(ILogger logger)
        {
            _logger = logger;
        }
        private   int GetLevel(  XPathNavigator node)
        {
            var n = node.Clone();
            if (n.MoveToParent() == false) return 0;
            return 1 + n.GetLevel();
        }
        /*
                    'R',    // Root
                    'E',    // Element
                    'A',    // Attribute
                    'N',    // Namespace
                    'T',    // Text
                    'S',    // SignificantWhitespace
                    'W',    // Whitespace
                    'P',    // ProcessingInstruction
                    'C',    // Comment
        */

        public   string NewGUID(XPathNavigator node)
        {
            var n1 = node.Clone();
            char prefixGUID;

            switch (n1.NodeType)
            {
                case XPathNodeType.Namespace:
                    prefixGUID = 'N';
                    break;
                case XPathNodeType.Attribute:
                    prefixGUID = 'A';
                    break;
                case XPathNodeType.Element:
                    prefixGUID = 'E';
                    break;
                case XPathNodeType.Root:
                    prefixGUID = 'R';
                    break;
                case XPathNodeType.Text:
                    prefixGUID = 'T';
                    break;
                case XPathNodeType.SignificantWhitespace:
                    prefixGUID = 'S';
                    break;
                case XPathNodeType.Whitespace:
                    prefixGUID = 'W';
                    break;
                case XPathNodeType.ProcessingInstruction:
                    prefixGUID = 'P';
                    break;
                case XPathNodeType.Comment:
                    prefixGUID = 'C';
                    break;

                default:
                    prefixGUID = 'X';
                    break;
            }
            byte[]? ba = null;
            ba = Encoding.Default.GetBytes(GetPath(n1));
            var hexString = BitConverter.ToString(ba);
            return prefixGUID.ToString() + ':' + hexString.Replace("-", "");


        }
        public   string NewGUID(XPathNavigator node, string label)
        {
            var n1 = node.Clone();
            char prefixGUID;

            switch (n1.NodeType)
            {
                case XPathNodeType.Namespace:
                    prefixGUID = 'N';
                    break;
                case XPathNodeType.Attribute:
                    prefixGUID = 'A';
                    break;
                case XPathNodeType.Element:
                    prefixGUID = 'E';
                    break;
                case XPathNodeType.Root:
                    prefixGUID = 'R';
                    break;
                case XPathNodeType.Text:
                    prefixGUID = 'T';
                    break;
                case XPathNodeType.SignificantWhitespace:
                    prefixGUID = 'S';
                    break;
                case XPathNodeType.Whitespace:
                    prefixGUID = 'W';
                    break;
                case XPathNodeType.ProcessingInstruction:
                    prefixGUID = 'P';
                    break;
                case XPathNodeType.Comment:
                    prefixGUID = 'C';
                    break;
                default:
                    prefixGUID = 'X';
                    break;
            }
            byte[]? ba = null;
            ba = Encoding.Default.GetBytes(GetPath(n1, label));
            var hexString = BitConverter.ToString(ba);
            return prefixGUID.ToString() + ':' + hexString.Replace("-", "");


        }

        /// <summary>
        /// GetPath(node) method attempts to derive the full node path to the current node. 
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public   string GetPath(  XPathNavigator node)
        {
            var n = node.Clone();
            string str = null;
            List<string> names = new List<string>();
            // Add potential object names
            names.Add("ObjectName");
            names.Add("Name");
            names.Add("name");
            names.Add("cachedName");
            names.Add("SqlStatementSource");

            foreach (var name in names)
            {
                if (!string.IsNullOrEmpty(node.GetAttribute(name, node.NamespaceURI)))
                {
                    str = $@"{node.Name}[@{name}={node.GetAttribute(name, node.NamespaceURI)}]";
                    break;
                }
            }

            if (string.IsNullOrEmpty(str))
            {
                str = $@"{node.Name}";
            }

            if (n.MoveToParent() == false)
            {

                return $@"ROOT";
            }
            else
            {
                return n.GetPath() + "/" + str;
            }
        }
        public string GetPath(XPathNavigator node, string label)
        {
            var n = node.Clone();
            string str = null;

            if (!string.IsNullOrEmpty(node.GetAttribute(label, node.NamespaceURI)))
            {
                str = $@"{node.Name}[@{label}={node.GetAttribute(label, node.NamespaceURI)}]";
            }

            if (string.IsNullOrEmpty(str))
            {
                str = $@"{node.Name}";
            }

            if (n.MoveToParent() == false)
            {

                return $@"ROOT";
            }
            else
            {
                return n.GetPath() + "/" + str;
            }
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

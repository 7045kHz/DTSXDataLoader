using System.Xml;
using System.Xml.XPath;

namespace DTSXDataLoader.Core.Service
{
    public interface INavigationService
    {
        public XmlDocument NewXmlDocument(string FileName);
        public XPathNavigator CreateNavigator(XmlDocument doc);
        public XmlNamespaceManager CreateNameSpaceManager(XmlNameTable NameTable);
        public string GetPath(XPathNavigator node);
        public string GetPath(XPathNavigator node, string label);
        public string GetUniqueId(XPathNavigator node);
    }
}
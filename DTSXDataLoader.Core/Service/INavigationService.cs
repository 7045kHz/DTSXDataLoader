using System.Xml;
using System.Xml.XPath;

namespace DTSXDataLoaderCore.Service
{
    public interface INavigationService
    {
        public XmlDocument NewXmlDocument(string FileName);
        public XPathNavigator CreateNavigator(XmlDocument doc);
        public XmlNamespaceManager CreateNameSpaceManager(XmlNameTable NameTable);
    }
}
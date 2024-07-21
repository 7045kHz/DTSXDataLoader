using System.Xml;
using System.Xml.XPath;

namespace DTSXDataLoader.Core.Models
{
    public interface IXConfig
    {
        XPathNodeIterator? Children { get; set; }
        string FileName { get; set; }
        string? nodeName { get; set; }
        string? nodeRefid { get; set; }
        XmlNamespaceManager? nsmgr { get; set; }
        string? PackageName();
    }
}
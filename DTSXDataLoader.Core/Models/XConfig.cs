using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.XPath;
using System.Xml;

namespace DTSXDataLoaderCore.Models
{
    public class XConfig : IXConfig
    {
        
        public XPathNodeIterator? Children { get; set; }
        public XmlNamespaceManager? nsmgr { get; set; }
        public string? nodeRefid { get; set; }
        public string? nodeName { get; set; }
        public string? PackageName() {
            
        return this.FileName.Split(Path.DirectorySeparatorChar).Last().ToString();
        }
        public string FileName { get; set; }
    }
}

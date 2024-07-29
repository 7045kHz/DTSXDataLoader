using DTSXDataLoader.Core.Models;
using System.Xml.XPath;
using DTSXDataLoader.Service;
namespace DTSXDataLoader.Service
{
    public   interface IDocumentProcessingService
    {
        List<DtsMapper> GetFlowDataMapper(XConfig config);
        List<DtsMapper> GetSqlExeMapper(XConfig config);
         List<DtsAttribute> GetAttributes(XPathNavigator node,XConfig config);
        List<DtsElement> GetElements(XConfig config);
        DtsVariable GetVariable(XPathNavigator node, XConfig config);
        List<DtsVariable> GetVariables(XConfig config);
        List<DtsAttribute> GetAttributes(XPathNavigator node, DtsElement element);
        List<DtsAttribute> GetAttributeListFromElements(List<DtsElement> elements);
    }
}
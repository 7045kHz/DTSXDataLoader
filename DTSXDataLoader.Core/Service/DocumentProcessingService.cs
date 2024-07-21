using System.Xml.XPath;
using DTSXDataLoader.Core.Models;
using System.Text;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using DTSXDataLoader.Core.Service;

namespace DTSXDataLoader.Service
{
    public class DocumentProcessingService : IDocumentProcessingService
    {

        private readonly IConfiguration _configuration;
        private readonly ILogger _logger;
        private readonly INavigationService _navigationService;

        public DocumentProcessingService(IConfiguration configuration, ILogger logger, INavigationService navigationService)
        {
            _configuration = configuration;
            _logger = logger;
            _navigationService = navigationService;
        }

        public DtsVariable GetVariable(XPathNavigator node, XConfig config)
        {
            try
            {
                DtsVariable variable = new DtsVariable();
                List<DtsVariable>? variableList = new List<DtsVariable>();

                var vAttributes = GetAttributes(node, config);
                XPathNavigator? NodeFirstClone = node.Clone();
                XPathNavigator? NodeParentClone = node.Clone();
                XPathNavigator? NodeVariableValueClone = node.Clone();
                if (NodeFirstClone != null && NodeParentClone != null )
                {

                    NodeParentClone.MoveToParent();
                    NodeVariableValueClone.MoveToChild("VariableValue", NodeFirstClone.NamespaceURI);

                    variable.ParentNodeType = NodeParentClone.NodeType.ToString();
                    variable.ParentRefId = NodeParentClone.GetAttribute("refId", NodeParentClone.NamespaceURI)?.ToString();
                    variable.ParentNodeDtsId = NodeParentClone.GetAttribute("DTSID", NodeParentClone.NamespaceURI);
                    variable.GUID = _navigationService.NewGUID(NodeFirstClone);
                    variable.ParentGUID = _navigationService.NewGUID(NodeParentClone);
                    variable.ParentNodeName = NodeParentClone.Name;
                    variable.Filename = config.FileName;
                    variable.Package = config.PackageName();
                    variable.VariableValue = NodeVariableValueClone.Value;
                    variable.XPath = _navigationService.GetPath(NodeFirstClone);
                    variable.Description = NodeFirstClone.GetAttribute("Description", NodeFirstClone.NamespaceURI)?.ToString();
                    variable.CreationName = NodeFirstClone.GetAttribute("CreationName", NodeFirstClone.NamespaceURI)?.ToString();

                    variable.VariableDataType = NodeVariableValueClone.GetAttribute("DataType", NodeVariableValueClone.NamespaceURI);

                    if (vAttributes.Any())
                    {

                        foreach (DtsAttribute attr in vAttributes)
                        {
                            if (!string.IsNullOrEmpty(attr.AttributeName))
                            {
                                switch (attr.AttributeName)
                                {
                                    case "DTS:ObjectName":
                                        variable.VariableName = attr.AttributeValue;
                                        break;
                                    case "DTS:Namespace":
                                        variable.VariableNameSpace = attr.AttributeValue;
                                        break;
                                    case "DTS:DTSID":
                                        variable.VariableDtsxId = attr.AttributeValue;
                                        break;
                                    case "DTS:IncludeInDebugDump":
                                        variable.IncludeInDebugDump = attr.AttributeValue;
                                        break;
                                    case "DTS:Expression":
                                        variable.VariableExpression = attr.AttributeValue;
                                        break;
                                    case "DTS:EvaluateAsExpression":
                                        variable.EvaluateAsExpression = attr.AttributeValue;
                                        break;
                                }
                            }
                        }

                    }
                }
                return variable;
            }
            catch (Exception e)
            {
                _logger.LogInformation($@"Program Error = {e}");
                throw;
            }

        }

        public List<DtsVariable> GetVariables(XConfig config)
        {
            try
            {
                List<DtsVariable>? variableList = new List<DtsVariable>();

                if (config.Children != null)
                {
                    foreach (XPathNavigator n in config.Children)
                    {
                        if (n != null)
                        {

                            var cloneN = n.Clone();

                            var variable = GetVariable(cloneN, config);
                            variableList.Add(variable);
                        }
                    }
                }
                return variableList;
            }
            catch (Exception e)
            {
                _logger.LogInformation($@"Program Error = {e}");
                throw;
            }


        }


        public DtsElement GetPackageRootElement(XPathNavigator n, XConfig config)
        {
            try
            {
                StringBuilder path = new StringBuilder();
                DtsElement element = new DtsElement();
                if (n != null)
                {

                    if (n != null)
                    {
                        var FirstNodeClone = n.Clone();



                        var cloneX = FirstNodeClone.Clone();
                        if (!n.IsEmptyElement)
                        {
                            element.ParentNodeType = Convert.ToString(FirstNodeClone.NodeType);
                            element.ParentRefId = FirstNodeClone.GetAttribute("refId", n.NamespaceURI)?.ToString();
                            element.ParentNodeDtsId = FirstNodeClone.GetAttribute("DTSID", n.NamespaceURI);
                            element.ParentGUID = "ROOT";
                            element.XmlType = Convert.ToString(FirstNodeClone.XmlType);
                            element.ParentNodeName = Convert.ToString(FirstNodeClone.Name);



                            element.XPath = _navigationService.GetPath(cloneX).ToString();
                            element.DtsId = FirstNodeClone.GetAttribute("DTSID", FirstNodeClone.NamespaceURI).ToString();
                            element.Description = FirstNodeClone.GetAttribute("Description", FirstNodeClone.NamespaceURI).ToString();
                            element.CreationName = FirstNodeClone.GetAttribute("CreationName", FirstNodeClone.NamespaceURI).ToString();
                            element.Name = FirstNodeClone.Name;
                            element.NodeType = Convert.ToString(FirstNodeClone.NodeType);

                            element.Filename = config.FileName.ToString();
                            element.Package = config.PackageName();
                            var vAttributes = GetAttributes(FirstNodeClone, element);
                            element.Attributes = vAttributes;

                            element.Value = FirstNodeClone.Value;
                        }
                    }

                }
                return element;
            }
            catch (Exception e)
            {
                _logger.LogInformation($@"Program Error = {e}");
                throw;
            }


        }

        /// <summary>
        /// GetElements(config) returns a List<DtsElement> of all Elements provided in Config.Children
        /// </summary>
        /// <param name="config"></param>
        /// <returns>List<DtsElement></returns>
        public List<DtsElement> GetElements(XConfig config)
        {
            try
            {
                List<DtsElement>? elementList = new List<DtsElement>();

                var TchildrenClone = config?.Children?.Clone();
                if (TchildrenClone != null)
                {

                    while (TchildrenClone.MoveNext())
                    {

                        XPathNavigator n = TchildrenClone.Current;
                        if (n != null)
                        {
                            var FirstNodeClone = n.Clone();
                            var ParentNodeClone = n.Clone();

                            // @$"DTS:VariableValue"
                            ParentNodeClone.MoveToParent(); // Becomes NodeType = Root, no name
                            DtsElement element = new DtsElement();
                            var cloneX = FirstNodeClone.Clone();
                            var parentNodeTypeIs = Convert.ToString(ParentNodeClone.NodeType);
                            var currentNodeTypeIs = Convert.ToString(FirstNodeClone.NodeType);

                            if (!string.IsNullOrEmpty(parentNodeTypeIs) && !string.IsNullOrEmpty(currentNodeTypeIs))
                            {
                                element.GUID = _navigationService.NewGUID(FirstNodeClone);
                                element.XPath = _navigationService.GetPath(cloneX).ToString();
                                element.DtsId = FirstNodeClone.GetAttribute("DTSID", FirstNodeClone.NamespaceURI).ToString();
                                element.Description = FirstNodeClone.GetAttribute("Description", FirstNodeClone.NamespaceURI).ToString();
                                element.CreationName = FirstNodeClone.GetAttribute("CreationName", FirstNodeClone.NamespaceURI).ToString();
                                element.Name = FirstNodeClone.Name;
                                element.NodeType = Convert.ToString(FirstNodeClone.NodeType);
                                element.Filename = config?.FileName.ToString();
                                element.Package = config?.PackageName();

                                if (FirstNodeClone.Name == "SQLTask:SqlTaskData")
                                {

                                    var t = FirstNodeClone.GetAttribute("SqlStatementSource", FirstNodeClone.NamespaceURI)?.ToString();
                                    var c = FirstNodeClone.GetAttribute("SQLTask:Connection", FirstNodeClone.NamespaceURI)?.ToString();
                                    element.Value = $@"[{c}] {t}";
                                }
                                else
                                {
                                    element.Value = FirstNodeClone.Value;
                                }


                                if (currentNodeTypeIs.Equals("Root"))
                                {
                                    element.ParentGUID = element.GUID;
                                    element.ParentRefId = FirstNodeClone.GetAttribute("refId", FirstNodeClone.NamespaceURI)?.ToString();
                                    element.ParentNodeDtsId = FirstNodeClone.GetAttribute("DTSID", FirstNodeClone.NamespaceURI);
                                    element.ParentNodeType = Convert.ToString(FirstNodeClone.NodeType);
                                    element.ParentNodeName = Convert.ToString(FirstNodeClone.Name);

                                }
                                else
                                {
                                    element.ParentGUID = _navigationService.NewGUID(ParentNodeClone);
                                    element.ParentRefId = ParentNodeClone.GetAttribute("refId", ParentNodeClone.NamespaceURI)?.ToString();
                                    element.ParentNodeDtsId = ParentNodeClone.GetAttribute("DTSID", ParentNodeClone.NamespaceURI);
                                    element.ParentNodeType = Convert.ToString(ParentNodeClone.NodeType);
                                    element.ParentNodeName = Convert.ToString(ParentNodeClone.Name);

                                }


                            }


                            var vAttributes = GetAttributes(FirstNodeClone, element);
                            element.Attributes = vAttributes;
                            elementList.Add(element);
                        }
                    }

                }
                    return elementList;
            }
            catch (Exception e)
            {
                _logger.LogInformation($@"Program Error = {e}");
                throw;
            }


        }
        public List<DtsAttribute> GetAttributeListFromElements(List<DtsElement> elements)
        {
            List<DtsAttribute> attributesList = new List<DtsAttribute>();
            try
            {
                if (elements != null && elements.Count > 0)
                {
                    foreach (DtsElement element in elements)
                    {
                        List<DtsAttribute> attributes = element.Attributes;
                        if (attributes?.Count > 0)
                        {
                            attributesList.AddRange(attributes);
                        }

                    }
                }
            }
            catch (Exception e)
            {

                _logger.LogInformation($@"GetAttributeListFromElements Error = {e}");
                throw;
            }
            return attributesList;
        }
        public List<DtsAttribute> GetAttributes(XPathNavigator node, DtsElement element)
        {
            try
            {

                XPathNavigator? NodeFirstClone = node.Clone();
                XPathNavigator? NodeElementPathClone = node.Clone();
                List<DtsAttribute> attributes = new List<DtsAttribute>();
                XPathNavigator? NodeParentClone = node.Clone();

                var eventCloneCount = NodeFirstClone.Evaluate("count(@*)");
                if (element != null)
                {
                    if (string.IsNullOrEmpty(element.RefId))
                    {
                        element.RefId = NodeFirstClone.GetAttribute("refId", NodeFirstClone.NamespaceURI);
                    }
                    if (string.IsNullOrEmpty(element.ParentRefId))
                    {
                        if (!NodeParentClone.IsNode)
                        {
                            NodeParentClone.MoveToParent();
                        }
                        element.ParentRefId = NodeParentClone.GetAttribute("refId", NodeParentClone.NamespaceURI);
                    }

                    NodeFirstClone.MoveToFirstAttribute();

                    if ((double)eventCloneCount > 0)
                    {
                        var attribute = new DtsAttribute()
                        {

                            Filename = element.Filename,
                            Package = element.Package,
                            ParentRefId = element.ParentRefId,
                            ParentGUID = _navigationService.NewGUID(NodeElementPathClone),
                            ParentNodeType = element.ParentNodeType,
                            ParentNodeName = element.ParentNodeName,
                            ParentNodeDtsId = element.ParentNodeDtsId,
                            XPath = _navigationService.GetPath(NodeFirstClone),
                            RefId = element.RefId,
                            AttributeName = NodeFirstClone.Name,
                            AttributeValue = NodeFirstClone.Value,
                            AttributeType = Convert.ToString(NodeFirstClone.ValueType),
                            GUID = _navigationService.NewGUID(NodeFirstClone),
                            ElementXPath = _navigationService.GetPath(NodeElementPathClone)?.ToString()
                        };
                        attributes.Add(attribute);
                    }


                    if ((double)eventCloneCount > 1)
                    {

                        while (NodeFirstClone.MoveToNextAttribute())
                        {
                            var attribute = new DtsAttribute()
                            {

                                Filename = element.Filename,
                                Package = element.Package,
                                ParentRefId = element.ParentRefId,
                                ParentGUID = _navigationService.NewGUID(NodeElementPathClone),
                                ParentNodeType = element.ParentNodeType,
                                ParentNodeName = element.ParentNodeName,
                                ParentNodeDtsId = element.ParentNodeDtsId,
                                XPath = _navigationService.GetPath(NodeFirstClone),
                                RefId = element.RefId,
                                AttributeName = NodeFirstClone.Name,
                                AttributeValue = NodeFirstClone.Value,
                                AttributeType = Convert.ToString(NodeFirstClone.ValueType),
                                GUID = _navigationService.NewGUID(NodeFirstClone),
                                ElementXPath = _navigationService.GetPath(NodeElementPathClone)?.ToString()
                            };
                            attributes.Add(attribute);
                        }
                    }
                }
                return attributes;




            }
            catch (Exception e)
            {
                _logger.LogInformation($@"GetAttributes Error = {e}");
                throw;
            }

        }

        public List<DtsAttribute> GetAttributes(XPathNavigator node, XConfig config)
        {
            try
            {
                XPathNavigator? NodeFirstClone = node.Clone();
                XPathNavigator? NodeParentClone = node.Clone();
                XPathNavigator? NodeElementPathClone = node.Clone();

                List<DtsAttribute> attributes = new List<DtsAttribute>();
                var nodeRefId = node.GetAttribute("refId", node.NamespaceURI);

                NodeParentClone.MoveToParent();
                var eventCloneCount = NodeFirstClone.Evaluate("count(@*)");
                NodeFirstClone.MoveToFirstAttribute();

                if ((double)eventCloneCount > 0)
                {
                    var attribute = new DtsAttribute()
                    {

                        Filename = config.FileName.ToString(),
                        Package = config.PackageName(),
                        ParentRefId = NodeParentClone.GetAttribute("refId", NodeParentClone.NamespaceURI),
                        GUID = _navigationService.NewGUID(NodeFirstClone),
                        ParentGUID = _navigationService.NewGUID(NodeParentClone),
                        ParentNodeType = Convert.ToString(NodeParentClone.NodeType),
                        XPath = _navigationService.GetPath(NodeFirstClone),
                        RefId = nodeRefId,
                        AttributeName = NodeFirstClone.Name,
                        AttributeValue = NodeFirstClone.Value,
                        AttributeType = Convert.ToString(NodeFirstClone.ValueType),
                        ElementXPath = _navigationService.GetPath(NodeElementPathClone)?.ToString()
                    };
                    attributes.Add(attribute);
                }


                if ((double)eventCloneCount > 1)
                {

                    while (NodeFirstClone.MoveToNextAttribute())
                    {
                        var attribute = new DtsAttribute()
                        {
                            ParentRefId = NodeParentClone.GetAttribute("refId", NodeParentClone.NamespaceURI),
                            ParentNodeName = NodeParentClone.Name,
                            GUID = _navigationService.NewGUID(NodeFirstClone),
                            ParentGUID = _navigationService.NewGUID(NodeParentClone),
                            ParentNodeType = Convert.ToString(NodeParentClone.NodeType),
                            XPath = _navigationService.GetPath(NodeFirstClone),
                            RefId = nodeRefId,
                            AttributeName = NodeFirstClone.Name,
                            AttributeValue = Convert.ToString(NodeFirstClone.Value),
                            AttributeType = Convert.ToString(NodeFirstClone.ValueType)
                        };
                        attributes.Add(attribute);
                    }
                }




                return attributes;
            }
            catch (Exception e)
            {
                _logger.LogInformation($@"Program Error = {e}");
                throw;
            }

        }

    }
}

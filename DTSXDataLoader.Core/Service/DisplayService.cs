using DTSXDataLoaderCore.Models;
using DTSXDataLoader.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
namespace DTSXDataLoaderCore.Service
{
    public class DisplayService : IDisplayService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger _logger;

        public DisplayService(IConfiguration configuration, ILogger logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        public   void DisplayVariables(List<DtsVariable> variables)
        {
            try
            {
                Console.WriteLine("##################################################################################################");
                Console.WriteLine(@$"Package Variables");
                Console.WriteLine("##################################################################################################");
                foreach (var variable in variables)
                {
                    if (variable != null)
                    {
                        string? value = null;
                        if (!string.IsNullOrEmpty(variable.EvaluateAsExpression) && !string.IsNullOrEmpty(variable.VariableExpression))
                        {
                            value = variable.VariableExpression?.ToString();
                        }
                        else
                        {
                            value = variable.VariableValue?.ToString();
                        }
                        Console.WriteLine($@"{variable.XPath} -> [{variable.CreationName}]:[{variable.VariableNameSpace}::{variable.VariableName}] ({variable.VariableDataType}) = '{value.ToString()}'  ");
                    }
                }
            }
            catch (Exception e)
            {
                _logger.LogInformation($@"Program Error = {e}");
                throw;
            }
            
        }
        public   void InsertVariables(List<DtsVariable> variables)
        {
            try
            {
                Console.WriteLine("/*");
                Console.WriteLine(@$"Package Variables");
                Console.WriteLine("*/");
                foreach (var variable in variables)
                {
                    if (variable != null)
                    {
                        string? value = null;
                        if (!string.IsNullOrEmpty(variable.EvaluateAsExpression) && !string.IsNullOrEmpty(variable.VariableExpression))
                        {
                            value = variable.VariableExpression?.ToString();
                        }
                        else
                        {
                            value = variable.VariableValue?.ToString();
                        }
                        var table = _configuration.GetSection("ApplicationTables").GetValue<string>("DtsxAttributes");
                        var sql = @$"insert into {table} (ParentNodeName,ParentNodeXmlType,ParentNodeType,ParentNodeDtsId,Package,Filename,RefId,XPath,CreationName,VariableDtsxId,IncludeInDebugDump";
                        sql = sql + $@",VariableName,VariableNameSpace,VariableValue,VariableDataType,VariableExpression,EvaluateAsExpression) VALUE (";
                        sql = sql + $@"{variable.ParentNodeName},{variable.ParentGUID},{variable.ParentNodeType},{variable.ParentNodeDtsId},{variable.Package},{variable.Filename},{variable.RefId},{variable.XPath}";
                        sql = sql + $@",{variable.CreationName},{variable.VariableDtsxId},{variable.IncludeInDebugDump}";
                        sql = sql + $@",{variable.VariableName},{variable.VariableNameSpace},{variable.VariableValue},{variable.VariableDataType},{variable.VariableExpression},{variable.EvaluateAsExpression}";
                        sql = sql + ");";

                        Console.WriteLine($@"{sql}");
                    }
                }
            }
            catch (Exception e)
            {
                _logger.LogInformation($@"Program Error = {e}");
                throw;
            }
            
        }
        public   void DisplayElements(List<DtsElement> elements)
        {
            try
            {
                Console.WriteLine("##################################################################################################");
                Console.WriteLine(@$"Package Elements");
                Console.WriteLine("##################################################################################################");
                foreach (var element in elements)
                {
                    if (element != null)
                    {
                        string? value = null;

                        Console.WriteLine($@"{element.XPath} -> [{element.CreationName}]:[{element.Name}] ({element.NodeType}) = '{element.Value.ToString()}'  ");
                    }
                }
            }
            catch (Exception e)
            {
                _logger.LogInformation($@"Program Error = {e}");

                throw;
            }
            
        }
        public void InsertElements(List<DtsElement> elements)
        {
            try
            {
                Console.WriteLine("/*");
                Console.WriteLine(@$"Package Elements");
                Console.WriteLine("*/");
                var table = _configuration.GetSection("ApplicationTables").GetValue<string>("DtsxElements");
                foreach (var element in elements)
                {
                    if (element != null)
                    {
                        var sql = @$"insert into {table} ([CreationName], [Description], [Filename], [Package], [ParentNodeDtsId], [ParentNodeName], [ParentNodeType], [ParentNodeXmlType]
        , [ParentRefId], [RefId], [XPath], [DtsId], [Name], [NodeType], [Value], [XmlType], [LoadDate]) VALUE (";
                        sql = sql + $@"{element.CreationName},{element.Description},{element.Filename},{element.Package},{element.ParentNodeDtsId},{element.ParentNodeName},{element.RefId},{element.ParentNodeType}";
                        sql = sql + $@",{element.ParentGUID},{element.ParentRefId},{element.RefId}";
                        sql = sql + $@",{element.RefId},{element.XPath},{element.DtsId},{element.Name},{element.NodeType},{element.Value},{element.XmlType},{DateTime.UtcNow.ToString()}";
                        sql = sql + ");";

                        Console.WriteLine($@"{sql}");
                       if( element.Attributes.Count > 0)
                        {
                            InsertElementAttributes(element);
                        }
                    }
                }
 
            }
            catch (Exception e)
            {
                _logger.LogInformation($@"Program Error = {e}");

                throw;
            }

        }
        public void InsertElementAttributes(DtsElement element)
        {
            try
            {
                Console.WriteLine("/*");
                Console.WriteLine(@$" Attributes");
                Console.WriteLine("*/");
                var table = _configuration.GetSection("ApplicationTables").GetValue<string>("DtsxAttributes");
                if(element != null)
                {
                    if (element != null && element.Attributes.Count > 0)
                    {
                        foreach (var attribute in element.Attributes)
                        {
                            var sql = @$"insert into {table} ([CreationName],[Description],[Filename],[Package],[ParentNodeDtsId],[ParentNodeName],[ParentNodeType],[ParentNodeXmlType],[ParentRefId]
      ,[RefId],[XPath],[ElementXPath],[AttributeName],[AttributeType],[AttributeValue],[LoadDate]) VALUE (";
                            sql = sql + $@"  {element.CreationName},{element.Description},{element.Filename},{element.Package},{element.ParentNodeDtsId},{element.ParentNodeName},{element.ParentNodeType},{element.ParentGUID}
,{element.ParentRefId},{element.RefId},{attribute.XPath},{attribute.ElementXPath},{attribute.AttributeName},{attribute.AttributeType},{attribute.AttributeValue},{DateTime.UtcNow.ToString()} );";

                            Console.WriteLine($@"{sql}");
                        }

                       
                    }
                }

            }
            catch (Exception e)
            {
                _logger.LogInformation($@"Program Error = {e}");

                throw;
            }

        }
        public   void DisplayPackage(List<DtsAttribute> attributes)
        {
            try
            {
                Console.WriteLine("##################################################################################################");
                Console.WriteLine(@$"Package Details / Attributes");
                Console.WriteLine("##################################################################################################");
                foreach (var attr in attributes)
                {
                    if (attr != null)
                    {
                        Console.WriteLine($@"{attr.XPath} -> [{attr.CreationName}]:[{attr.AttributeName}] ({attr.AttributeType}) = {attr.AttributeValue.ToString()}");
                    }
                }
            }
            catch (Exception e)
            {
                _logger.LogInformation($@"Program Error = {e}");

                throw;
            }
            
        }
    }

}

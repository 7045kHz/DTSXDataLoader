using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTSXDataLoader.Core.Models
{

    /*
     *         public string? NodeRefId { get; set; }
        public string? NodeName { get; set; }
        public object? NodeType { get; set; }
     */
    public class DtsVariable : DtsBase
    {
        public string? VariableDtsxId { get; set; } = string.Empty;
        public string? IncludeInDebugDump { get; set; } = string.Empty;
        public string? VariableName { get; set; } = string.Empty;
        public string? VariableNameSpace { get; set; } = string.Empty;
        public string? VariableValue { get; set; } = string.Empty;
        public string? VariableDataType { get; set; } = string.Empty;
        public string? VariableExpression { get; set; } = string.Empty;
        public string? EvaluateAsExpression { get; set; } = string.Empty;
    }
}

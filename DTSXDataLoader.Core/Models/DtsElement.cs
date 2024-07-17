using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTSXDataLoaderCore.Models
{
    public class DtsElement :  DtsBase
    {
        public string? Name { get; set; } = string.Empty;
        public string? Value { get; set; } = string.Empty;
        public string? NodeType { get; set; } = string.Empty;
        public string DtsId { get; set; } = string.Empty;
        public string? XmlType { get; set; } = string.Empty;
        public List<DtsAttribute>? Attributes { get; set; } = new List<DtsAttribute>();
    }
}

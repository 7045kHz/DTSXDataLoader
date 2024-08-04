using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTSXDataLoader.Core.Models
{
    public class DtsBase : IDtsBase
    {
        public int Id { get; set; }
        public string? ParentRefId { get; set; } = string.Empty;
        public string? ParentNodeName { get; set; } = string.Empty;
        public string? ParentUniqueId { get; set; } = string.Empty;
        public string? ParentNodeType { get; set; } = string.Empty;
        public string? ParentNodeDtsId { get; set; } = string.Empty;
        public string? Package { get; set; } = string.Empty;
        public string? Filename { get; set; } = string.Empty;
        public string? RefId { get; set; } = string.Empty;
        public string? UniqueId { get; set; } = string.Empty;
        public string? XPath { get; set; } = string.Empty;
        public string? CreationName { get; set; } = string.Empty;
        public string? Description { get; set; } = string.Empty;
        
    }

}

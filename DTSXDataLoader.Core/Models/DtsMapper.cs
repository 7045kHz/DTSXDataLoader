using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTSXDataLoader.Core.Models
{
    public class DtsMapper : IDtsMapper
    {
        public string? Package { get; set; } = string.Empty;
        public string? SqlStatement { get; set; } = string.Empty;
        public string? ConnectionString { get; set; } = string.Empty;
        public string? ConnectionName { get; set; } = string.Empty;
        public string ConnectionDtsId { get; set; } = string.Empty;
        public string? ConnectionType { get; set; } = string.Empty;
        public string? ConnectionRefId { get; set; } = string.Empty;
        public string? Name { get; set; } = string.Empty;
        public string? Description { get; set; } = string.Empty;
        public string? RefId { get; set; } = string.Empty;
        public string? ComponentType { get; set; } = string.Empty;
    }
}

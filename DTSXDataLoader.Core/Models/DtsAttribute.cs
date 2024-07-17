using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTSXDataLoaderCore.Models
{
    public class DtsAttribute : DtsBase
    {
        public string? AttributeName { get; set; } = string.Empty;
        public string? AttributeValue { get; set; } = string.Empty;
        public string? AttributeType { get; set; } = string.Empty;
        public string? ElementXPath { get; set; } = string.Empty;

    }
}

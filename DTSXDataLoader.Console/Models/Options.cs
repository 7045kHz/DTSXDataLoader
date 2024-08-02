using CommandLine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTSXDataLoader.Models
{
    public class Options : IOptions
    {
        [Option('v', "verbose", Required = false, HelpText = "Set output to verbose messages.")]
        public bool IsVerbose { get; set; } = false;

        [Option('p', "path", Required = false, HelpText = "Path to individual package or directory of packages")]
        public string? Path { get; set; }

        [Option('s', "sql", Required = false, HelpText = "SQL")]
        public bool IsSql { get; set; } = false;
        [Option('l', "lite", Required = false, HelpText = "Mapper and Variables Only")]
        public bool IsLite { get; set; } = false;

        [Option('x', "extension", Required = false, HelpText = "non-dtsx extension")]
        public string? Extension { get; set; } 
        [Option('o', "Output", Required = false, HelpText = "Output Directory")]
        public string? OutputDirectory { get; set; }
    }
}

using CommandLine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTSXDataLoaderCore.Models
{
    public class Options : IOptions
    {
        [Option('v', "verbose", Required = false, HelpText = "Set output to verbose messages.")]
        public bool IsVerbose { get; set; }

        [Option('i', "ifile", Required = false, HelpText = "Path to individual package")]
        public string? File { get; set; }

        [Option('s', "sql", Required = false, HelpText = "SQL")]
        public bool IsSql { get; set; }
        [Option('d', "InputDirectory", Required = false, HelpText = "Package Directory")]
        public string? InputDirectory { get; set; }

        [Option('o', "Output", Required = false, HelpText = "Output Directory")]
        public string? OutputDirectory { get; set; }
    }
}

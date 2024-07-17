using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommandLine;
using DTSXDataLoaderCore.Models;
using DTSXDataLoader.Service;
namespace DTSXDataLoader.Service
{
    public class CommandLineService : ICommandLineService
    {
        public CommandLineService() { }
        public void CheckCommandArguments(string[] args)
        {
            var options = new Dictionary<string,object> ();
            Parser.Default.ParseArguments<Options>(args)
                   .WithParsed<Options>(o =>
                   {
                       if (!string.IsNullOrEmpty(o.InputDirectory))
                       {
                           Console.WriteLine($"Directory input enabled. Current Arguments: -d {o.InputDirectory}");
                       }
                       if (!string.IsNullOrEmpty(o.File))
                       {
                           Console.WriteLine($"File input enabled. Current Arguments: -f {o.File}");
                       }
                       if (!string.IsNullOrEmpty(o.OutputDirectory))
                       {
                           Console.WriteLine($"Directory output enabled. Current Arguments: -o {o.OutputDirectory}");
                       }
                   });
        }

    }

}


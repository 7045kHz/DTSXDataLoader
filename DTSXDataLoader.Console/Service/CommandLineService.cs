using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommandLine;
using DTSXDataLoader.Models;
using DTSXDataLoader.Service;
using CommandLine;
using Microsoft.Extensions.Logging;
using System.IO;
namespace DTSXDataLoader.Service
{
    public class CommandLineService : ICommandLineService
    {
        private readonly ILogger _logger;

        public CommandLineService(ILogger logger)
        {
            _logger = logger;
        }

        public IOptions CheckCommandArguments(string[] args)
        {
           var returnOptions = new Options();

           Parser.Default.ParseArguments<Options>(args)
                   .WithParsed<Options>(o =>
                   {

                        
                       if (!string.IsNullOrEmpty(o.Path))
                       {
                           Console.WriteLine($"File input enabled. Current Arguments: -p {o.Path}");
                           returnOptions.Path = o.Path;
                       }
                       if (!string.IsNullOrEmpty(o.OutputDirectory))
                       {
                           Console.WriteLine($"Directory output enabled. Current Arguments: -o {o.OutputDirectory}");
                           returnOptions.OutputDirectory = o.OutputDirectory;
                       }
                       if (o.IsVerbose)
                       {
                           Console.WriteLine($"Directory output enabled. Current Arguments: -v {o.IsVerbose}");
                           returnOptions.IsVerbose = true;
                       } 
                   });
            return returnOptions;
        }
        private IEnumerable<string> GetAllFilesInDirectory(string path, string extension)
        {
            IEnumerable<string> returnList = new List<string>();
            if (!string.IsNullOrEmpty(path) && !string.IsNullOrEmpty(extension))
            {
                var directoryInfo = new DirectoryInfo(path);
               return directoryInfo.GetFiles(extension, SearchOption.AllDirectories).Select(i => i.FullName); 
            }
            
            return returnList;
        }

public IEnumerable<string> GetArrayOfFiles(IOptions options) {
            IEnumerable<string> returnList = new List<string>();

            if (options != null && !string.IsNullOrEmpty(options.Path))
            {
                FileAttributes attr = File.GetAttributes(options.Path);
                if (attr.HasFlag(FileAttributes.Directory))
                {
                    Console.WriteLine("Its a directory");
                    returnList = GetAllFilesInDirectory(options.Path,"*.dtsx");
                    return returnList;
                }
                else
                {
                    Console.WriteLine("Its a file");
                    if (!string.IsNullOrEmpty(options.Path))
                    {
                            return returnList.Append(options.Path);
                    }

                }
                return returnList;
            }
            else
            {
                _logger.LogCritical("Configuration file problems");
                return returnList;
            }
           

        }

    }

}


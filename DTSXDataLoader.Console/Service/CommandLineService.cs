using CommandLine;
using DTSXDataLoader.Models;
using Microsoft.Extensions.Logging;
using DTSXDataLoader.Core.Service;
namespace DTSXDataLoader.Service
{
    public class CommandLineService : ICommandLineService
    {
        private readonly ILogger _logger;
        private readonly IFileService _fileService;

        public CommandLineService(ILogger logger, IFileService fileService)
        {
            _logger = logger;
            _fileService = fileService;
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
                        if (o.IsLite)
                        {
                            Console.WriteLine($"Directory output enabled. Current Arguments: -l {o.IsLite}");
                            returnOptions.IsLite = true;
                        }
                        if (o.IsTruncate)
                        {
                            Console.WriteLine($"Directory output enabled. Current Arguments: -t {o.IsTruncate}");
                            returnOptions.IsTruncate = true;
                        }
                    });
            return returnOptions;
        }
 

        public IEnumerable<string> GetArrayOfFilesFromOptions(IOptions options)
        {
            IEnumerable<string> returnList = new List<string>();

            if (options != null && !string.IsNullOrEmpty(options.Path))
            {
                FileAttributes attr = File.GetAttributes(options.Path);
                if (attr.HasFlag(FileAttributes.Directory))
                {
                    Console.WriteLine("Its a directory");
                    if (!string.IsNullOrEmpty(options.Extension))
                    {
                        returnList = _fileService.GetAllFilesInDirectory(options.Path, options.Extension);
                    }
                    else
                    {
                        returnList = _fileService.GetAllFilesInDirectory(options.Path, "*.dtsx");
                    }
                    
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


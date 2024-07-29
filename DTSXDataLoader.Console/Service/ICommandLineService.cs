using DTSXDataLoader.Models;

namespace DTSXDataLoader.Service
{
    public interface ICommandLineService
    {
         IOptions CheckCommandArguments(string[] args);
          IEnumerable<string> GetArrayOfFilesFromOptions(IOptions options);
    }
}
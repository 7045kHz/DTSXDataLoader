using DTSXDataLoader.Models;

namespace DTSXDataLoader.Service
{
    public interface ICommandLineService
    {
         IOptions CheckCommandArguments(string[] args);
          IEnumerable<string> GetArrayOfFiles(IOptions options);
    }
}
using DTSXDataLoader.Models;

namespace DTSXDataLoader.Service
{
    public interface ICommandLineService
    {
       public IOptions CheckCommandArguments(string[] args);
        public IEnumerable<string> GetArrayOfFiles(IOptions options);
    }
}
using CommandLine;

namespace DTSXDataLoader.Models
{
    public interface IOptions
    {
        string? Path
        {
            get; set;
        }
        bool IsSql
        {
            get; set;
        }
        bool IsVerbose
        {
            get; set;
        }
        string? OutputDirectory
        {
            get; set;
        }
        string? Extension
        {
            get; set;
        }
        bool IsLite
        {
            get; set;
        }
    }
}
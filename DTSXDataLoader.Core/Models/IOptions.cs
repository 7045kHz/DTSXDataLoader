namespace DTSXDataLoaderCore.Models
{
    public interface IOptions
    {
        string? File { get; set; }
        string? InputDirectory { get; set; }
        bool IsSql { get; set; }
        bool IsVerbose { get; set; }
        string OutputDirectory { get; set; }
    }
}
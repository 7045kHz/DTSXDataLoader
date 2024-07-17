namespace DTSXDataLoaderCore.Models
{
    public interface IDtsAttribute: IDtsBase
    {
        string? AttributeName { get; set; }
        string? ElementXPath { get; set; }
        string? AttributeType { get; set; }
        string? AttributeValue { get; set; }
    }
}
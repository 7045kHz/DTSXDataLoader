namespace DTSXDataLoaderCore.Models
{
    public interface IDtsBase
    {
        string? CreationName { get; set; }
        string? Description { get; set; }
        string? Filename { get; set; }
        string? Package { get; set; }
        string? ParentNodeDtsId { get; set; }
        string? ParentNodeName { get; set; }
        string? ParentNodeType { get; set; }
        string? ParentGUID { get; set; }
        string? ParentRefId { get; set; }
        string? GUID { get; set; }
        string? RefId { get; set; }
        string? XPath { get; set; }
    }
}
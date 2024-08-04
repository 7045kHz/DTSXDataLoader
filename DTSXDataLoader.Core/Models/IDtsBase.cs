namespace DTSXDataLoader.Core.Models
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
        string? ParentUniqueId { get; set; }
        string? ParentRefId { get; set; }
        string? UniqueId { get; set; }
        string? RefId { get; set; }
        string? XPath { get; set; }
    }
}
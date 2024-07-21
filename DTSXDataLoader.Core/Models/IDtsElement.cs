
namespace DTSXDataLoader.Core.Models
{
    public interface IDtsElement  : IDtsBase
    {
        List<DtsAttribute>? Attributes { get; set; }
        string DtsId { get; set; }
        string? Name { get; set; }
        string? NodeType { get; set; }
        string? Value { get; set; }
        string? XmlType { get; set; }
    }
}
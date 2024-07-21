namespace DTSXDataLoader.Core.Models
{
    public interface IDtsVariable  : IDtsBase
    {
        string? EvaluateAsExpression { get; set; }
        string? IncludeInDebugDump { get; set; }
        string? VariableDataType { get; set; }
        string? VariableDtsxId { get; set; }
        string? VariableExpression { get; set; }
        string? VariableName { get; set; }
        string? VariableNameSpace { get; set; }
        string? VariableValue { get; set; }
    }
}
namespace DTSXDataLoader.Core.Models;

public interface IDtsMapper
{
    string ConnectionDtsId
    {
        get;
        set;
    }
    string? ConnectionName
    {
        get;
        set;
    }
    string? ConnectionType
    {
        get; set;
    }
    string? ConnectionRefId
    {
        get;
        set;
    }
    string? ConnectionString
    {
        get;
        set;
    }
    string? Description
    {
        get;
        set;
    }
    string? Name
    {
        get;
        set;
    }
    string? Package
    {
        get;
        set;
    }
    string? RefId
    {
        get;
        set;
    }
    string? SqlStatement
    {
        get;
        set;
    }
    string? ComponentType
    {
        get; 
        set;
    }

}
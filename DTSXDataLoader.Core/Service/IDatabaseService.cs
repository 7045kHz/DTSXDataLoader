
using DTSXDataLoaderCore.Models;

namespace DTSXDataLoaderCore.Service
{
    public interface  IDatabaseService
    {
        Task<int> CheckAttributesTable();
        Task<int> CheckElementsTable();
        Task<int> CheckVariablesTable();
        Task<bool> IsDbConnectionActive();
        Task  TruncateTable(string tableName);
        Task<int> InsertElementsAsync(IEnumerable<DtsElement> elements);
        Task<int> InsertAttributesAsync(IEnumerable<DtsAttribute> attributes);
        Task<int> InsertVariablesAsync(IEnumerable<DtsVariable> variables);
        Task<IEnumerable<DtsElement>> GetAllElementsAsync();
        Task<IEnumerable<DtsAttribute>> GetAllAttributesAsync();
        Task<IEnumerable<DtsVariable>> GetAllVariablesAsync();
    }
}
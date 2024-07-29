
using DTSXDataLoader.Core.Models;

namespace DTSXDataLoader.Core.Service
{
    public interface  IEtlDatabaseService
    {
        Task<int> InsertEtlMapperAsync(IEnumerable<DtsMapper> mapper);
        Task<int> CheckEtlAttributesTableAsync();
        Task<int> CheckEtlMapperTableAsync();
        Task<int> CheckEtlElementsTableAsync();
        Task<int> CheckEtlVariablesTableAsync();
        Task<bool> IsDbConnectionActiveAsync();
        Task  TruncateEtlTableAsync(string tableName);
        Task<int> InsertEtlElementsAsync(IEnumerable<DtsElement> elements);
        Task<int> InsertEtlAttributesAsync(IEnumerable<DtsAttribute> attributes);
        Task<int> InsertEtlVariablesAsync(IEnumerable<DtsVariable> variables);
        Task<IEnumerable<DtsElement>> GetEtlElementsAllAsync();
        Task<IEnumerable<DtsAttribute>> GetEtlAttributesAllAsync();
        Task<IEnumerable<DtsVariable>> GetEtlVariablesAllAsync();
         Task TruncateEtlTablesAllAsync();
        Task SaveEtlToDatabaseAsync(List<DtsElement> packageElements, List<DtsAttribute> packageAttributes, List<DtsVariable> packageVariables, List<DtsMapper> mapper);

    }
}
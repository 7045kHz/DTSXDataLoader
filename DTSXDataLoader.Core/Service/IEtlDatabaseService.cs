
using DTSXDataLoader.Core.Models;

namespace DTSXDataLoader.Core.Service
{
    public interface  IEtlDatabaseService
    {
        Task<int> CheckEtlDependentTableAsync(string table);
        Task<int> InsertEtlAsync<T>(IEnumerable<T> list,   string sql) where T : class;
        Task<bool> IsDbConnectionActiveAsync();
        Task  TruncateEtlTableAsync(string tableName);
        Task<IEnumerable<DtsElement>> GetEtlElementsAllAsync();
        Task<IEnumerable<DtsAttribute>> GetEtlAttributesAllAsync();
        Task<IEnumerable<DtsVariable>> GetEtlVariablesAllAsync();
        Task SaveLiteEtlToDb(List<DtsVariable> packageVariables, List<DtsMapper> mapper, bool truncate);
        Task SaveAllEtlToDb(List<DtsElement> packageElements, List<DtsAttribute> packageAttributes, List<DtsVariable> packageVariables, List<DtsMapper> mapper, bool truncate);

    }
}
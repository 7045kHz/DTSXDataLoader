using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DTSXDataLoader.Core.Models;
using Dapper;
using System.Runtime.CompilerServices;
using System.ComponentModel.Design;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;
using Microsoft.Extensions.Configuration.Json;
using Microsoft.Extensions.Configuration.CommandLine;
using Microsoft.Extensions.Configuration.Binder;
using Microsoft.Extensions.Configuration.EnvironmentVariables;
using System.IO;
namespace DTSXDataLoader.Core.Service;

public class EtlDatabaseService : IEtlDatabaseService
{
    private readonly IConfiguration _configuration;
    private readonly ILogger _logger;
    private readonly string _connectionString;

    public EtlDatabaseService(IConfiguration configuration, ILogger logger)
    {
        _configuration = configuration;
        _logger = logger;
        _connectionString = _configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

    }
    public async Task SaveAllEtlToDb(List<DtsElement> packageElements, List<DtsAttribute> packageAttributes, List<DtsVariable> packageVariables, List<DtsMapper> mapper, bool truncate)
    {

        if (mapper != null)
        {
            _logger.LogInformation($@"Running InsertEtlAsync<DtsMapper>()");
            var tableName = _configuration.GetSection("ApplicationTables").GetValue<string>("DtsxMapper");
            if (truncate && !string.IsNullOrEmpty(tableName))
            {
                await TruncateEtlTableAsync(tableName);
            }
            var sql = @$"insert into {tableName} ([Description] ,[Package]  ,[RefId]  ,[SqlStatement]  ,[ConnectionString]  ,[ConnectionName]  ,[ConnectionDtsId]
      ,[ConnectionType]  ,[ConnectionRefId]  ,[Name]  ,[ComponentType]) VALUES (@Description ,@Package  ,@RefId  ,@SqlStatement  ,@ConnectionString  ,@ConnectionName  ,@ConnectionDtsId
      ,@ConnectionType  ,@ConnectionRefId  ,@Name  ,@ComponentType)";
            var returnCount = await InsertEtlAsync<DtsMapper>(mapper, sql);
            _logger.LogInformation($@" Writting {returnCount} Mapper");
        }
        if (packageAttributes != null)
        {
            _logger.LogInformation($@"Running InsertEtlAsync<DtsAttribute>()");
            var tableName = _configuration.GetSection("ApplicationTables").GetValue<string>("DtsxAttributes");
            if (truncate && !string.IsNullOrEmpty(tableName))
            {
                await TruncateEtlTableAsync(tableName);
            }
            var sql = @$"insert into {tableName}
([CreationName],[Description],[Filename],[Package],[ParentNodeDtsId],[ParentNodeName],[ParentNodeType],[ParentGUID], [GUID],[ParentRefId],[RefId],[XPath]
,[ElementXPath],[AttributeName],[AttributeType],[AttributeValue]) VALUES (@CreationName,@Description,@Filename,@Package,@ParentNodeDtsId,@ParentNodeName,@ParentNodeType,@ParentGUID,@GUID,@ParentRefId,@RefId,@XPath,@ElementXPath,@AttributeName,@AttributeType
,@AttributeValue)";
            var returnCount = await InsertEtlAsync<DtsAttribute>(packageAttributes, sql);
            _logger.LogInformation($@" Writting {returnCount} attributes");
        }
        if (packageVariables != null)
        {
            _logger.LogInformation($@"Running InsertEtlAsync<DtsVariable>()");
            var tableName = _configuration.GetSection("ApplicationTables").GetValue<string>("DtsxVariables");
            if (truncate && !string.IsNullOrEmpty(tableName))
            {
                await TruncateEtlTableAsync(tableName);
            }

            var sql = @$"insert into {tableName}
([CreationName],[Description],[Filename],[Package],[ParentNodeDtsId],[ParentNodeName],[ParentNodeType],[ParentGUID], [GUID],[ParentRefId],[RefId],[XPath],[EvaluateAsExpression],[IncludeInDebugDump],[VariableDataType]
,[VariableDtsxId],[VariableExpression],[VariableName],[VariableNameSpace],[VariableValue]) 
VALUES (@CreationName,@Description,@Filename,@Package,@ParentNodeDtsId,@ParentNodeName,@ParentNodeType,@ParentGUID,@GUID,@ParentRefId,@RefId,@XPath,@EvaluateAsExpression,@IncludeInDebugDump,@VariableDataType,@VariableDtsxId
,@VariableExpression,@VariableName,@VariableNameSpace,@VariableValue)";
            var returnCount = await InsertEtlAsync<DtsVariable>(packageVariables,sql);
            _logger.LogInformation($@"Writting {returnCount} Variables");
        }
        if (packageElements != null)
        {
            _logger.LogInformation($@"Running InsertEtlAsync<DtsElement>()");
            var tableName = _configuration.GetSection("ApplicationTables").GetValue<string>("DtsxElements");
            if (truncate && !string.IsNullOrEmpty(tableName))
            {
                await TruncateEtlTableAsync(tableName);
            }
            var sql = @$"insert into {tableName}
                    ([CreationName], [Description], [Filename], [Package], [ParentNodeDtsId], [ParentNodeName], [ParentNodeType], [ParentGUID], [GUID]
        , [ParentRefId], [RefId], [XPath], [DtsId], [Name], [NodeType], [Value], [XmlType]) VALUES (@CreationName, @Description, @Filename, @Package, @ParentNodeDtsId, @ParentNodeName, @ParentNodeType, @ParentGUID, @GUID
        , @ParentRefId, @RefId, @XPath, @DtsId, @Name, @NodeType, @Value, @XmlType)";
            var returnCount = await InsertEtlAsync<DtsElement>(packageElements,sql);
            _logger.LogInformation($@"Writting {returnCount} Elements");
        }

    }
    public async Task TruncateEtlTableAsync(string tableName)
    {
        try
        {

            var sqlString = $@"truncate table {tableName}";
            using var connection = new SqlConnection(_connectionString);
            await connection.ExecuteAsync(@$"{sqlString}");
        }
        catch (Exception e)
        {
            _logger.LogInformation($@"TruncateTable Error = {e}");

        }

    }

    public async Task<bool> IsDbConnectionActiveAsync()
    {
        try
        {
            int? IsCheckAttribute = await CheckEtlDependentTableAsync("DtsxAttributes");
            int? IsCheckElement = await CheckEtlDependentTableAsync("DtsxElements");
            int? IsCheckVariable = await CheckEtlDependentTableAsync("DtsxVariables");
            int? IsCheckMapper = await CheckEtlDependentTableAsync("DtsxMapper");
            if ((IsCheckAttribute + IsCheckElement + IsCheckVariable + IsCheckMapper) > 0)
            {
                return false;
            }
            else return true;
        }
        catch (Exception e)
        {
            _logger.LogInformation($@"Program Error = {e}");

            throw;
        }


    }

    public async Task<int> CheckEtlDependentTableAsync(string table)
    {
        try
        {

            var tableName = _configuration.GetSection("ApplicationTables").GetValue<string>(table);
            var sqlString = $@"IF OBJECT_ID(N'{tableName}', N'U') IS NOT NULL     PRINT 0	ELSE PRINT 1;";
            using var connection = new SqlConnection(_connectionString);
            return await connection.ExecuteAsync(@$"{sqlString}");
        }
        catch (Exception e)
        {
            _logger.LogInformation($@"{table} Error = {e}");
            return -1;
        }

    }


    

    public async Task<int> InsertEtlAsync<T>(IEnumerable<T> list, string sql) where T : class
    {
        try
        {

            using (var connection = new SqlConnection(_connectionString))
            {
                var rowsAffected = await connection.ExecuteAsync(sql, list);
                return rowsAffected;
            }
        }
        catch (Exception e)
        {
            _logger.LogInformation($@"InsertElementsAsync Error = {e}");
            throw;
        }

    }
    
    
    
     
    public async Task<IEnumerable<DtsVariable>> GetEtlVariablesAllAsync()
    {


        try
        {

            using (var connection = new SqlConnection(_connectionString))
            {
                IEnumerable<DtsVariable> variables = new List<DtsVariable>();
                var tableName = _configuration.GetSection("ApplicationTables").GetValue<string>("DtsxVariables");

                var sql = @$"SELECT 
[Id],[CreationName],[Description],[Filename],[Package],[ParentNodeDtsId],[ParentNodeName],[ParentNodeType],[ParentGUID], [GUID],[ParentRefId],[RefId],[XPath],[EvaluateAsExpression],[IncludeInDebugDump],[VariableDataType]
,[VariableDtsxId],[VariableExpression],[VariableName],[VariableNameSpace],[VariableValue],[LoadDate]) 
FROM {tableName} ";
                variables = await connection.QueryAsync<DtsVariable>(sql);
                return variables;
            }
        }
        catch (Exception e)
        {
            _logger.LogInformation($@"GetAllVariablesAsync Error = {e}");
            throw;
        }


    }
    public async Task<IEnumerable<DtsAttribute>> GetEtlAttributesAllAsync()
    {
        IEnumerable<DtsAttribute> attributes = new List<DtsAttribute>();

        try
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var tableName = _configuration.GetSection("ApplicationTables").GetValue<string>("DtsxVariables");

                var sql = @$"SELECT 
[Id],[CreationName],[Description],[Filename],[Package],[ParentNodeDtsId],[ParentNodeName],[ParentNodeType],[ParentGUID], [GUID],[ParentRefId],[RefId],[XPath]
,[ElementXPath],[AttributeName],[AttributeType],[AttributeValue],[LoadDate]) 
FROM {tableName} ";
                attributes = await connection.QueryAsync<DtsAttribute>(sql);
                return attributes;
            }
        }
        catch (Exception e)
        {
            _logger.LogInformation($@"GetAllAttributesAsync Error = {e}");
            throw;
        }

    }
    public async Task<IEnumerable<DtsElement>> GetEtlElementsAllAsync()
    {
        IEnumerable<DtsElement> elements = new List<DtsElement>();

        try
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var tableName = _configuration.GetSection("ApplicationTables").GetValue<string>("DtsxVariables");

                var sql = @$"SELECT 
[Id],[CreationName], [Description], [Filename], [Package], [ParentNodeDtsId], [ParentNodeName], [ParentNodeType], [ParentGUID], [GUID], [ParentRefId], [RefId], [XPath], [DtsId], [Name], [NodeType], [Value], [XmlType], [LoadDate]) 
FROM {tableName} ";
                elements = await connection.QueryAsync<DtsElement>(sql);
                return elements;
            }
        }
        catch (Exception e)
        {
            _logger.LogInformation($@"GetAllElementsAsync Error = {e}");
            throw;
        }

    }
}



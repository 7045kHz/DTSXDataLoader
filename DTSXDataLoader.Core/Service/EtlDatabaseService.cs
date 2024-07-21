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
    public async Task SaveEtlToDatabaseAsync( List<DtsElement> packageElements, List<DtsAttribute> packageAttributes, List<DtsVariable> packageVariables)
    {
       
            if (packageAttributes != null)
            {
                _logger.LogInformation($@"Running InsertAttributesAsync()");
                var returnCount = await InsertEtlAttributesAsync(packageAttributes);
                _logger.LogInformation($@" Writting {returnCount} attributes");
            }
            if (packageVariables != null)
            {
                _logger.LogInformation($@"Running InsertVariablesAsync()");
                var returnCount = await InsertEtlVariablesAsync(packageVariables);
                _logger.LogInformation($@"Writting {returnCount} Variables");
            }
            if (packageElements != null)
            {
                _logger.LogInformation($@"Running InsertElementsAsync()");
                var returnCount = await InsertEtlElementsAsync(packageElements);
                _logger.LogInformation($@"Writting {returnCount} Elements");
            }

    }
    public async Task  TruncateEtlTableAsync(string tableName)
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
            int IsCheckAttribute = await CheckEtlAttributesTableAsync();
            int IsCheckElement = await CheckEtlElementsTableAsync();
            int IsCheckVariable = await CheckEtlVariablesTableAsync();
            if ((IsCheckAttribute + IsCheckElement + IsCheckVariable) > 0)
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
    public async Task<int> CheckEtlAttributesTableAsync()
    {
        try
        {

            var tableName = _configuration.GetSection("ApplicationTables").GetValue<string>("DtsxAttributes");
            var sqlString = $@"IF OBJECT_ID(N'{tableName}', N'U') IS NOT NULL     PRINT 0	ELSE PRINT 1;";
            if (string.IsNullOrEmpty(_connectionString))
            {
                _logger.LogCritical($@"CheckAttributesTable Error with connection string");
             }
            using var connection = new SqlConnection(_connectionString);
            return await connection.ExecuteAsync(@$"{sqlString}");
        }
        catch (Exception e)
        {
            _logger.LogInformation($@"CheckAttributesTable Error = {e}");
            return -1;
        }

    }
    public async Task<int> CheckEtlElementsTableAsync()
    {
        try
        {

            var tableName = _configuration.GetSection("ApplicationTables").GetValue<string>("DtsxElements");
            var sqlString = $@"IF OBJECT_ID(N'{tableName}', N'U') IS NOT NULL     PRINT 0	ELSE PRINT 1;";
            using var connection = new SqlConnection(_connectionString);
            return await connection.ExecuteAsync(@$"{sqlString}");
        }
        catch (Exception e)
        {
            _logger.LogInformation($@"CheckElementsTable Error = {e}");
            return -1;
        }

    }
    public async Task<int> CheckEtlVariablesTableAsync()
    {
        try
        {

            var tableName = _configuration.GetSection("ApplicationTables").GetValue<string>("DtsxVariables");
            var sqlString = $@"IF OBJECT_ID(N'{tableName}', N'U') IS NOT NULL     PRINT 0	ELSE PRINT 1;";
            using var connection = new SqlConnection(_connectionString);
            return await connection.ExecuteAsync(@$"{sqlString}");
        }
        catch (Exception e)
        {
            _logger.LogInformation($@"CheckVariablesTable Error = {e}");
            return -1;
        }

    }
    public async Task  TruncateEtlTablesAllAsync()
    {
        try
        {

            var tableName = _configuration.GetSection("ApplicationTables").GetValue<string>("DtsxElements");
            if(tableName != null)
            {
                Console.WriteLine($@"Truncating table {tableName}  ");
                await TruncateEtlTableAsync(tableName);
                Console.WriteLine($@"Truncate table {tableName} done ");
            }

              tableName = _configuration.GetSection("ApplicationTables").GetValue<string>("DtsxAttributes");
            if (tableName != null)
            {
                Console.WriteLine($@"Truncating table {tableName}  ");
                await TruncateEtlTableAsync(tableName);
                Console.WriteLine($@"Truncate table {tableName} done ");
            }
              tableName = _configuration.GetSection("ApplicationTables").GetValue<string>("DtsxVariables");
            if (tableName != null)
            {
                Console.WriteLine($@"Truncating table {tableName}  ");
                await TruncateEtlTableAsync(tableName);
                Console.WriteLine($@"Truncate table {tableName} done ");
            }
        }
        catch (Exception e)
        {
            _logger.LogInformation($@"InsertElementsAsync Error = {e}");
            throw;
        }

    }
    public async Task<int> InsertEtlElementsAsync(IEnumerable<DtsElement> elements)
    {
        try
        {
            var tableName = _configuration.GetSection("ApplicationTables").GetValue<string>("DtsxElements");
          
            using (var connection = new SqlConnection(_connectionString))
            {

                var sql = @$"insert into {tableName}
                    ([CreationName], [Description], [Filename], [Package], [ParentNodeDtsId], [ParentNodeName], [ParentNodeType], [ParentGUID], [GUID]
        , [ParentRefId], [RefId], [XPath], [DtsId], [Name], [NodeType], [Value], [XmlType]) VALUES (@CreationName, @Description, @Filename, @Package, @ParentNodeDtsId, @ParentNodeName, @ParentNodeType, @ParentGUID, @GUID
        , @ParentRefId, @RefId, @XPath, @DtsId, @Name, @NodeType, @Value, @XmlType)";
                var rowsAffected = await connection.ExecuteAsync(sql, elements);
                return rowsAffected;
            }
        }
        catch (Exception e)
        {
            _logger.LogInformation($@"InsertElementsAsync Error = {e}");
            throw;
        }
    
    }
    public   async Task<int> InsertEtlAttributesAsync(IEnumerable<DtsAttribute> attributes)
    {
        try
        {
            var tableName = _configuration.GetSection("ApplicationTables").GetValue<string>("DtsxAttributes");

 
            using (var connection = new SqlConnection(_connectionString))
            {
                try
                {

                    var sql = @$"insert into {tableName}
([CreationName],[Description],[Filename],[Package],[ParentNodeDtsId],[ParentNodeName],[ParentNodeType],[ParentGUID], [GUID],[ParentRefId],[RefId],[XPath]
,[ElementXPath],[AttributeName],[AttributeType],[AttributeValue]) VALUES (@CreationName,@Description,@Filename,@Package,@ParentNodeDtsId,@ParentNodeName,@ParentNodeType,@ParentGUID,@GUID,@ParentRefId,@RefId,@XPath,@ElementXPath,@AttributeName,@AttributeType
,@AttributeValue)";
                    var rowsAffected =    await connection.ExecuteAsync(sql, attributes);
                    if(rowsAffected == 0)
                    {
                        Console.WriteLine(  "ZERO");
                    }
                    Console.WriteLine("pause");
                    return rowsAffected;
                }
                catch (SqlException ex)
                {
                    Console.WriteLine($"Database operation failed: {ex.Message}");

                    throw;
                }

            }
        }
        catch (Exception e)
        {
            _logger.LogInformation($@"InsertAttributesAsync Error = {e}");
            throw;
        }
    }
    public async Task<int> InsertEtlVariablesAsync(IEnumerable<DtsVariable> variables)
    {
        try
        {
            var tableName = _configuration.GetSection("ApplicationTables").GetValue<string>("DtsxVariables");
 
            using (var connection = new SqlConnection(_connectionString))
            {

                var sql = @$"insert into {tableName}
([CreationName],[Description],[Filename],[Package],[ParentNodeDtsId],[ParentNodeName],[ParentNodeType],[ParentGUID], [GUID],[ParentRefId],[RefId],[XPath],[EvaluateAsExpression],[IncludeInDebugDump],[VariableDataType]
,[VariableDtsxId],[VariableExpression],[VariableName],[VariableNameSpace],[VariableValue]) 
VALUES (@CreationName,@Description,@Filename,@Package,@ParentNodeDtsId,@ParentNodeName,@ParentNodeType,@ParentGUID,@GUID,@ParentRefId,@RefId,@XPath,@EvaluateAsExpression,@IncludeInDebugDump,@VariableDataType,@VariableDtsxId
,@VariableExpression,@VariableName,@VariableNameSpace,@VariableValue)";
                var rowsAffected = await connection.ExecuteAsync(sql, variables);
                return rowsAffected;
            }
        }
        catch (Exception e)
        {
            _logger.LogInformation($@"InsertVariablesAsync Error = {e}");
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



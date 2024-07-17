using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DTSXDataLoaderCore.Models;
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
namespace DTSXDataLoaderCore.Service;

public class DatabaseService : IDatabaseService
{
    private readonly IConfiguration _configuration;
    private readonly ILogger _logger;
    private readonly string _connectionString;

    public DatabaseService(IConfiguration configuration, ILogger logger)
    {
        _configuration = configuration;
        _logger = logger;
        _connectionString = _configuration.GetConnectionString("DefaultConnection");

    }
    public async Task  TruncateTable(string tableName)
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

    public async Task<bool> IsDbConnectionActive()
    {
        try
        {
            int IsCheckAttribute = await CheckAttributesTable();
            int IsCheckElement = await CheckElementsTable();
            int IsCheckVariable = await CheckVariablesTable();
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
    public async Task<int> CheckAttributesTable()
    {
        try
        {

            var tableName = _configuration.GetSection("ApplicationTables").GetValue<string>("DtsxAttributes");
            var sqlString = $@"IF OBJECT_ID(N'{tableName}', N'U') IS NOT NULL     PRINT 0	ELSE PRINT 1;";
            using var connection = new SqlConnection(_connectionString);
            return await connection.ExecuteAsync(@$"{sqlString}");
        }
        catch (Exception e)
        {
            _logger.LogInformation($@"CheckAttributesTable Error = {e}");
            return -1;
        }

    }
    public async Task<int> CheckElementsTable()
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
    public async Task<int> CheckVariablesTable()
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
    public async Task<int> InsertElementsAsync(IEnumerable<DtsElement> elements)
    {
        try
        {
            var tableName = _configuration.GetSection("ApplicationTables").GetValue<string>("DtsxElements");
             await TruncateTable(tableName);
            Console.WriteLine(  $@"Truncate table {tableName}  ");
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
    public   async Task<int> InsertAttributesAsync(IEnumerable<DtsAttribute> attributes)
    {
        try
        {
            var tableName = _configuration.GetSection("ApplicationTables").GetValue<string>("DtsxAttributes");

            await   TruncateTable(tableName);
            Console.WriteLine($@"Truncate table {tableName} ");
            using (var connection = new SqlConnection(_connectionString))
            {
                try
                {

                    var sql = @$"insert into {tableName}
([CreationName],[Description],[Filename],[Package],[ParentNodeDtsId],[ParentNodeName],[ParentNodeType],[ParentGUID], [GUID],[ParentRefId],[RefId],[XPath]
,[ElementXPath],[AttributeName],[AttributeType],[AttributeValue]) VALUES (@CreationName,@Description,@Filename,@Package,@ParentNodeDtsId,@ParentNodeName,@ParentNodeType,@ParentGUID,@GUID,@ParentRefId,@RefId,@XPath,@ElementXPath,@AttributeName,@AttributeType
,@AttributeValue)";
                    //await connection.OpenAsync();
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
    public async Task<int> InsertVariablesAsync(IEnumerable<DtsVariable> variables)
    {
        try
        {
            var tableName = _configuration.GetSection("ApplicationTables").GetValue<string>("DtsxVariables");
            await TruncateTable(tableName);
            Console.WriteLine($@"Truncate table {tableName}  ");
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
    public async Task<IEnumerable<DtsVariable>> GetAllVariablesAsync()
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
    public async Task<IEnumerable<DtsAttribute>> GetAllAttributesAsync()
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
    public async Task<IEnumerable<DtsElement>> GetAllElementsAsync()
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



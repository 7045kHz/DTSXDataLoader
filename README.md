# DTSX DataReader
Intent is to create a way of mapping DTSX files to SQL for easier mappinmg and C4 Model generation.
 
 **ATTENTION: ALPHA CODE NOT FOR PRODUCTION USAGE**

## Command Line Execution

### Example Execution 
```bash

.\DTSXDataLoader -p C:\\PATH\\DIRECTORY\\ -l -s -v -t

```

### Options
Not all options enabled.

```c-sharp
        [Option('v', "verbose", Required = false, HelpText = "Set output to verbose messages.")]
        public bool IsVerbose { get; set; } = false;

        [Option('p', "path", Required = false, HelpText = "Path to individual package or directory of packages")]
        public string? Path { get; set; }

        [Option('s', "sql", Required = false, HelpText = "SQL")]
        public bool IsSql { get; set; } = false;

        [Option('l', "lite", Required = false, HelpText = "Mapper and Variables Only")]
        public bool IsLite { get; set; } = false;

		[Option('t', "truncate", Required = false, HelpText = "Truncates all destination tables first")]
        public bool IsTruncate { get; set; } = false;

        [Option('x', "extension", Required = false, HelpText = "non-dtsx extension")]
        public string? Extension { get; set; } 

        [Option('o', "Output", Required = false, HelpText = "Output Directory")]
        public string? OutputDirectory { get; set; }
```
## Initial DB and App settings configuration

### Appsettings.json

```json

{
  "Settings": {
    "DefaultPackageFile": "C:\\SSIS\\PACKAGE_DIR\\"
  },
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=DBSERVER;Initial Catalog=DB;Trusted_Connection=True;Encrypt=False"
  },
  "ApplicationTables": {
    "DtsxAttributes": "ETL.DTSX_Attributes",
    "DtsxVariables": "ETL.DTSX_Variables",
    "DtsxElements": "ETL.DTSX_Elements",
    "DtsxMapper": "ETL.DTSX_Mapper"
  },
  "ScanElements": [
    "//*"
  ],

}


```

### SQL Table Dependencies

```sql

CREATE TABLE [SSIS].[DTSX_Attributes](
	[Id] [int] IDENTITY(1,1) NOT NULL PRIMARY KEY,
	[CreationName] [nvarchar](max) NULL,
	[Description] [nvarchar](max) NULL,
	[Filename] [nvarchar](max) NULL,
	[Package] [nvarchar](max) NULL,
	[ParentNodeDtsId] [nvarchar](max) NULL,
	[ParentNodeName] [nvarchar](max) NULL,
	[ParentNodeType] [nvarchar](max) NULL,
	[ParentGUID] [nvarchar](250) NULL,
	[GUID] [nvarchar](250) NULL,
	[ParentRefId] [nvarchar](max) NULL,
	[RefId] [nvarchar](max) NULL,
	[XPath] [nvarchar](max) NULL,
	[ElementXPath] [nvarchar](max) NULL,
	[AttributeName] [nvarchar](max) NULL,
	[AttributeType] [nvarchar](max) NULL,
	[AttributeValue] [nvarchar](max) NULL,
	[LoadDate] [datetime] NOT NULL DEFAULT CURRENT_TIMESTAMP,
);

 

CREATE TABLE [SSIS].[DTSX_Elements](
	[Id] [int] IDENTITY(1,1) NOT NULL PRIMARY KEY,
	[CreationName] [nvarchar](max) NULL,
	[Description] [nvarchar](max) NULL,
	[Filename] [nvarchar](max) NULL,
	[Package] [nvarchar](max) NULL,
	[ParentNodeDtsId] [nvarchar](max) NULL,
	[ParentNodeName] [nvarchar](max) NULL,
	[ParentNodeType] [nvarchar](max) NULL,
	[ParentGUID] [nvarchar](250) NULL,
	[GUID] [nvarchar](250) NULL,
	[ParentRefId] [nvarchar](max) NULL,
	[RefId] [nvarchar](max) NULL,
	[XPath] [nvarchar](max) NULL,
	[DtsId] [nvarchar](max) NULL,
	[Name] [nvarchar](max) NULL,
	[NodeType] [nvarchar](max) NULL,
	[Value] [nvarchar](max) NULL,
	[XmlType] [nvarchar](max) NULL,
	[LoadDate] [datetime] NOT NULL DEFAULT CURRENT_TIMESTAMP,
);

 

CREATE TABLE [SSIS].[DTSX_Mapper](
	[Id] [int] IDENTITY(1,1) NOT NULL PRIMARY KEY,
	[Description] [nvarchar](max) NULL,
	[Package] [nvarchar](max) NULL,
	[RefId] [nvarchar](max) NULL,
	[SqlStatement] [nvarchar](max) NULL,
	[ConnectionString] [nvarchar](max) NULL,
	[ConnectionName] [nvarchar](max) NULL,
	[ConnectionDtsId] [nvarchar](max) NULL,
	[ConnectionType] [nvarchar](max) NULL,
	[ConnectionRefId] [nvarchar](max) NULL,
	[Name] [nvarchar](max) NULL,
	[ComponentType] [nvarchar](max) NULL,
	[LoadDate] [datetime] NOT NULL DEFAULT CURRENT_TIMESTAMP,
);
 

CREATE TABLE [SSIS].[DTSX_Variables](
	[Id] [int] IDENTITY(1,1) NOT NULL PRIMARY KEY,
	[CreationName] [nvarchar](max) NULL,
	[Description] [nvarchar](max) NULL,
	[Filename] [nvarchar](max) NULL,
	[Package] [nvarchar](max) NULL,
	[ParentNodeDtsId] [nvarchar](max) NULL,
	[ParentNodeName] [nvarchar](max) NULL,
	[ParentNodeType] [nvarchar](max) NULL,
	[ParentGUID] [nvarchar](250) NULL,
	[GUID] [nvarchar](250) NULL,
	[ParentRefId] [nvarchar](max) NULL,
	[RefId] [nvarchar](max) NULL,
	[XPath] [nvarchar](max) NULL,
	[EvaluateAsExpression] [nvarchar](max) NULL,
	[IncludeInDebugDump] [nvarchar](max) NULL,
	[VariableDataType] [nvarchar](max) NULL,
	[VariableDtsxId] [nvarchar](max) NULL,
	[VariableExpression] [nvarchar](max) NULL,
	[VariableName] [nvarchar](max) NULL,
	[VariableNameSpace] [nvarchar](max) NULL,
	[VariableValue] [nvarchar](max) NULL,
	[LoadDate] [datetime] NOT NULL DEFAULT CURRENT_TIMESTAMP,
);


/*
  Listing Maps with Pipelines and resolved variables

*/
SELECT   m.[Id]
      ,m.[Description]
      ,m.[Package]
      ,m.[RefId]
	  ,'Pipeline' =   CASE
		  WHEN SUBSTRING(m.[RefId], 1, LEN(m.[RefId]) - CHARINDEX('\', REVERSE(m.[RefId]))) = 'Package' THEN m.[RefId]
		  ELSE SUBSTRING(m.[RefId], 1, LEN(m.[RefId]) - CHARINDEX('\', REVERSE(m.[RefId]))) 
		  END

      ,'SQL Source' = m.[SqlStatement]
	  ,'Resolved SQL' = CASE 
		  WHEN v.VariableValue  IS NULL THEN m.SqlStatement
		  ELSE v.VariableValue
		  END
      ,m.[ConnectionString]
      ,m.[ConnectionName]
      ,m.[ConnectionDtsId]
      ,m.[ConnectionType]
      ,m.[ConnectionRefId]
      ,m.[Name]
      ,m.[ComponentType]
      ,m.[LoadDate]
  FROM [PROTO].[SSIS].[DTSX_Mapper] m
  left join [PROTO].[SSIS].[DTSX_Variables] v on v.Package=m.Package and CONCAT(v.[VariableNameSpace],'::',v.[VariableName]) = m.SqlStatement


```
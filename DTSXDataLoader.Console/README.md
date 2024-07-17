# DTSX DataReader
Intent is to create a way of mapping DTSX files to SQL for easier mappinmg and C4 Model generation.

## Initial DB and App settings configuration

### Appsettings.json

```json

{
  "Settings": {
    "DefaultPackageFile": "Y:\\GitHub\\SSISDeploy\\dtsx_data_sync.dtsx"
  },
  "ConnectionStrings": {
    "DefaultConnection": "Server=P330; Database=PROTO;Authentication=Windows Authentication;"
  },
  "ApplicationTables": {
    "DtsxAttributes": "[PROTP].[SSIS].[DTSX_Attributes]",
    "DtsxVariables": "[PROTP].[SSIS].[DTSX_Elements]",
    "DtsxElements": "[PROTP].[SSIS].[DTSX_Variables]"
  }
}

```
### SQL Table Dependencies

```sql
create table [PROTO].[SSIS].[DTSX_Elements] (
	[Id] [int] IDENTITY(1,1) NOT NULL PRIMARY KEY
      ,  CreationName nvarchar(max)
        , Description nvarchar(max)
        , Filename nvarchar(max)
        , Package nvarchar(max)
        , ParentNodeDtsId nvarchar(max)
        , ParentNodeName nvarchar(max) NULL
        , ParentNodeType nvarchar(max) NULL -- object
        , ParentNodeXmlType nvarchar(max) NULL -- object
        , ParentRefId nvarchar(max) NULL
        , RefId nvarchar(max)
        , XPath nvarchar(max)
        , DtsId nvarchar(max)
        , Name nvarchar(max) NULL
        , NodeType nvarchar(max) -- object
        , Value nvarchar(max) -- object
        , XmlType nvarchar(max) -- object
		, LoadDate Datetime not null
);


create table [PROTO].[SSIS].[DTSX_Attributes] (
	[Id] [int] IDENTITY(1,1) NOT NULL PRIMARY KEY
	, [ElementId] int not null
        , CreationName nvarchar(max) NOT NULL
        , Description nvarchar(max)
        , Filename nvarchar(max) NOT NULL
        , Package nvarchar(max) NOT NULL
        , ParentNodeDtsId nvarchar(max)  NULL
        ,ParentNodeName nvarchar(max) NULL
        , ParentNodeType nvarchar(max) NULL -- object
        , ParentNodeXmlType nvarchar(max) NULL -- object
        , ParentRefId nvarchar(max) NULL
        , RefId nvarchar(max)
        , XPath nvarchar(max)
		,AttributeName nvarchar(max) NULL
        , AttributeType nvarchar(max) NULL -- object
        , AttributeValue nvarchar(max) NULL -- object
		, LoadDate Datetime not null DEFAULT CURRENT_TIMESTAMP

);
ALTER TABLE [PROTO].[SSIS].[DTSX_Attributes]
   ADD CONSTRAINT FK_DTSX_Attributes_ElementId FOREIGN KEY (ElementId)
      REFERENCES [PROTO].[SSIS].[DTSX_Elements]  (Id) ;


create table [PROTO].[SSIS].[DTSX_Variables] (
       	[Id] [int] IDENTITY(1,1) NOT NULL,

		CreationName nvarchar(max)
        , Description nvarchar(max)
        , Filename nvarchar(max)
        , Package nvarchar(max)
        , ParentNodeDtsId nvarchar(max)
        , ParentNodeName nvarchar(max) NULL
        , ParentNodeType nvarchar(max) NULL -- object
        , ParentNodeXmlType nvarchar(max) NULL -- object
        , ParentRefId nvarchar(max) NULL
        , RefId nvarchar(max)
        , XPath nvarchar(max)
		, EvaluateAsExpression nvarchar(max) NULL
        , IncludeInDebugDump nvarchar(max) NULL
        , VariableDataType nvarchar(max) NULL
        , VariableDtsxId nvarchar(max) NULL
        , VariableExpression nvarchar(max) NULL
        , VariableName nvarchar(max) NULL
        , VariableNameSpace nvarchar(max) NULL
        , VariableValue nvarchar(max) NULL -- object
		, LoadDate Datetime not null
);


```
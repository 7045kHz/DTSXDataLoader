﻿using System.Xml.XPath;
using System.Xml;
using DTSXDataLoader.Core.Models;
using DTSXDataLoader.Service;
using DTSXDataLoader.Core.Service;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using DTSXDataLoader.Models;
using System.Collections.Generic;
namespace DTSXDataLoader;

public static class Program
{



    static async Task Main(string[] args)
    {
        IConfiguration configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
            .AddEnvironmentVariables()
            .AddCommandLine(args)
            .Build();
        using var loggerFactory = LoggerFactory.Create(builder =>
        {
            builder
                .AddFilter("Microsoft", LogLevel.Information)
                .AddFilter("System", LogLevel.Information)
                .AddFilter("DataReader.Program", LogLevel.Debug)
                .AddConsole();
        });
        ILogger logger = loggerFactory.CreateLogger<ILoggerFactory>();
        logger.LogInformation("DTSX DataReader ");

        // Basic Setup
        ServiceProvider serviceProvider = new ServiceCollection()
            .AddLogging()
            //  .AddSingleton<IDisplayService, DisplayService>()
            .AddSingleton<IDocumentProcessingService, DocumentProcessingService>()
            .AddSingleton<ICommandLineService, CommandLineService>()
            .AddSingleton<IFileService, FileService>()
            .AddSingleton<IEtlDatabaseService, EtlDatabaseService>()
            .AddSingleton<INavigationService, NavigationService>()
            .AddScoped<IConfiguration>(_ => configuration)
            .AddScoped<ILogger>(_ => logger)
            .BuildServiceProvider();

        var processingService = serviceProvider.GetService<IDocumentProcessingService>();
        var commandLineService = serviceProvider.GetService<ICommandLineService>();
        var databaseService = serviceProvider.GetService<IEtlDatabaseService>();
        var navigationService = serviceProvider.GetService<INavigationService>();

        // Setup Lists for output storage
        List<DtsAttribute>? packageAttributes = new List<DtsAttribute>();
        List<DtsElement>? packageElements = new List<DtsElement>();
        List<DtsElement>? elements = new List<DtsElement>();
        List<DtsVariable>? packageVariables = new List<DtsVariable>();
        List<DtsMapper>? packageDataMapper = new List<DtsMapper>();



        IEnumerable<string> fileList = new List<string>();
        string FileName = string.Empty;
        string? nodeRefid = string.Empty;
        string? nodeName = string.Empty;
        string xpath = string.Empty;
        bool? IsDbActive = false;
        List<string>? xpaths;

        IOptions options = new Options();

        try
        {
            if (commandLineService != null)
            {
                options = commandLineService.CheckCommandArguments(args);
                if (!string.IsNullOrEmpty(options.Path))
                {
                    FileName = options.Path;
                    if (options.IsVerbose)
                    {
                        logger.LogInformation(@$"Scanning: {FileName}");
                    }
                }
            }
            else
            {
                logger.LogCritical($@"Error loading command lile options");
            }

        }
        catch (Exception e)
        {
            logger.LogInformation($@"Error getting file for scanning = {e}");

            throw;
        }
        // Confirm access to DB
        try
        {
            IsDbActive = false;
            IsDbActive = databaseService?.IsDbConnectionActiveAsync().Result;

            if (IsDbActive == true)
            {
                logger.LogInformation($@"Initiated SQL Database Connection = {IsDbActive}");

            }


        }
        catch (Exception e)
        {
            logger.LogInformation($@"SQL Database Connection Failure = {e}");
            throw;
        }

        try
        {

            // Define and load the XML Objects
            if (commandLineService != null && databaseService != null && processingService != null && navigationService != null)
            {
                if (!string.IsNullOrEmpty(options.Path))
                {

                    fileList = commandLineService.GetArrayOfFilesFromOptions(options);
                    var total = fileList.Count();
                    var currentCount = 1;
                    foreach (var file in fileList)
                    {

                        Console.WriteLine($@"Processing {total}/{currentCount} - {FileName}");


                        FileName = file;


                        XmlDocument doc = navigationService.NewXmlDocument(FileName);
                        XPathNavigator nav = navigationService.CreateNavigator(doc);
                        XmlNamespaceManager nsmgr = navigationService.CreateNameSpaceManager(nav.NameTable);


                        if (!options.IsLite && packageAttributes != null && packageAttributes.Count >= 1)
                        {

                            if (!string.IsNullOrEmpty(packageAttributes?.Find(a => a.ParentRefId == "Package")?.ParentRefId))
                            {
                                nodeRefid = packageAttributes?.Find(a => a.ParentRefId == "Package")?.ParentRefId;

                            }
                            if (!string.IsNullOrEmpty(packageAttributes?.Find(a => a.ParentNodeName == "DTS:Executable")?.ParentNodeName))
                            {
                                nodeName = packageAttributes?.Find(a => a.ParentNodeName == "DTS:Executable")?.ParentNodeName;

                            }

                        }

                        XConfig XmlConfig = new XConfig()
                        {
                            FileName = FileName,
                            nsmgr = nsmgr,
                            nodeRefid = nodeRefid,
                            nodeName = nodeName,
                        };
                        if (options.IsVerbose)
                        {
                            logger.LogInformation(@$"Scanning file: {XmlConfig.FileName} Package {XmlConfig?.PackageName()}");

                        }


                        if (XmlConfig != null)
                        {
                            xpath = "//pipeline/components/component/properties/property[@name='OpenRowset' or @name='SqlCommandVariable' or @name='SqlCommand' or @name='OpenRowsetVariable']";
                            var allChildren = nav.Select(xpath, nsmgr);

                            XmlConfig.Children = allChildren;
                            if (options.IsVerbose)
                            {
                                logger.LogInformation($@"Running GetFlowDataMapper");
                            }
                            packageDataMapper.AddRange(processingService.GetFlowDataMapper(XmlConfig));

                            xpath = "//pipeline/components/component[@componentClassID='Microsoft.FlatFileDestination']";
                            allChildren = nav.Select(xpath, nsmgr);

                            XmlConfig.Children = allChildren;
                            if (options.IsVerbose)
                            {
                                logger.LogInformation($@"Running GetFlowDataMapper");
                            }
                            packageDataMapper.AddRange(processingService.GetFlowDataMapper(XmlConfig));



                            // Collect SQL Execute
                            xpath = "//DTS:Executable/DTS:ObjectData/SQLTask:SqlTaskData";

                            allChildren = nav.Select(xpath, nsmgr);
                            XmlConfig.Children = allChildren;
                            if (options.IsVerbose)
                            {
                                logger.LogInformation($@"Running GetSqlExeMapper");
                            }
                            packageDataMapper.AddRange(processingService.GetSqlExeMapper(XmlConfig));


                            // Find Variables

                            xpath = "//DTS:Variables/child::*";

                            allChildren = nav.Select(xpath, nsmgr);
                            XmlConfig.Children = allChildren;
                            if (options.IsVerbose)
                            {
                                logger.LogInformation($@"Running GetVariables");
                            }
                            packageVariables.AddRange(processingService.GetVariables(XmlConfig));
                        }

                        if (configuration != null && configuration.GetSection("ScanElements").GetChildren().Any())
                        {
                            xpaths = configuration.GetSection("ScanElements").Get<List<string>>();
                            if (xpaths != null && xpaths.Count >= 1)
                            {
                                foreach (var x in xpaths)
                                {
                                    if (XmlConfig != null)
                                    {

                                        var allChildren = nav.Select(x, nsmgr);
                                        XmlConfig.Children = allChildren;
                                        if (options.IsVerbose)
                                        {
                                            logger.LogInformation($@"Running GetElements");
                                        }
                                        elements = processingService.GetElements(XmlConfig);
                                        packageElements.AddRange(elements);
                                    }
                                }
                            }

                        }

                        currentCount++;
                    }
                    try
                    {
                        if (options.IsLite)
                        {
                            if (packageVariables != null && packageDataMapper != null)
                            {
                                if (options.IsVerbose)
                                {
                                    logger.LogInformation(@$"Saving data to database");
                                }
                                await databaseService.SaveLiteEtlToDb(packageVariables, packageDataMapper, options.IsTruncate);
                            }
                            else
                            {
                                logger.LogError($@"Not all lists available to insert to database");
                            }
                        }
                        else
                        {
                            logger.LogInformation($@"Getting Attribute List From Elements");
                            var elementAttributes = processingService?.GetAttributeListFromElements(packageElements);
                            if (elementAttributes != null)
                            {
                                packageAttributes?.AddRange(elementAttributes);
                            }
                            if (packageAttributes != null && packageVariables != null && packageElements != null && packageDataMapper != null)
                            {
                                if (options.IsVerbose) { logger.LogInformation(@$"Saving data to database"); }
                                await databaseService.SaveAllEtlToDb(packageElements, packageAttributes, packageVariables, packageDataMapper, options.IsTruncate);
                            }
                            else
                            {
                                logger.LogError($@"Not all lists available to insert to database");
                            }
                        }




                    }
                    catch (Exception e)
                    {
                        logger.LogInformation($@"Display Error = {e}");
                        throw;
                    }

                }
                else
                {
                    logger.LogCritical($@"File {FileName} Missing or empty selection");
                    Environment.Exit(-1);
                }
            }
        }
        catch (Exception e)
        {
            logger.LogInformation($@"Program Error = {e}");
            throw;
        }
    }
}

using System.Reflection.PortableExecutable;
using System.Xml.XPath;
using System.Xml;
using DTSXDataLoaderCore.Models;
using DTSXDataLoader.Service;
using DTSXDataLoaderCore.Service;
using System.Diagnostics;
using System.Xml.Linq;
using System.Collections.Generic;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;
using Microsoft.Extensions.Configuration.CommandLine;
using Microsoft.Extensions.Configuration.Binder;
using Microsoft.Extensions.Configuration.EnvironmentVariables;
using System;
using System.IO;
using CommandLine;
using System.Formats.Tar;
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
            .AddSingleton<IDatabaseService, DatabaseService>()
            .AddSingleton<INavigationService, NavigationService>()
            .AddScoped<IConfiguration>(_ => configuration)
            .AddScoped<ILogger>(_ => logger)
            .BuildServiceProvider();

        var processingService = serviceProvider.GetService<IDocumentProcessingService>();
        var commandLineService = serviceProvider.GetService<ICommandLineService>();
        var databaseService = serviceProvider.GetService<IDatabaseService>();
        var navigationService = serviceProvider.GetService<INavigationService>();

        // Setup Lists for output storage
        List<DtsAttribute>? packageAttributes = new List<DtsAttribute>();
        List<DtsElement>? packageElements = new List<DtsElement>();
        List<DtsElement>? elements = new List<DtsElement>();
        List<DtsVariable>? packageVariables = new List<DtsVariable>();

        string? FileName = null;
        string? nodeRefid = null;
        string? nodeName = null;
        string? xpath = null;
        bool? IsDbActive = false;
        List<string>? xpaths;

        // Confirm access to DB
        try
        {
            IsDbActive = false;
            IsDbActive = databaseService?.IsDbConnectionActive().Result;

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
            FileName = configuration?.GetSection("Settings").GetValue<string>("DefaultPackageFile");
            commandLineService?.CheckCommandArguments(args);

            logger.LogInformation(@$"Scanning file: {FileName}");
        }
        catch (Exception e)
        {
            logger.LogInformation($@"Error getting file for scanning = {e}");

            throw;
        }

        try
        {

            // Define and load the XML Objects
            if (string.IsNullOrEmpty(FileName) || !File.Exists(FileName) || navigationService == null || processingService == null || configuration == null)
            {
                logger.LogCritical($@"File {FileName} Missing or empty selection");
                Environment.Exit(-1);
            }
            else
            {
                XmlDocument doc = navigationService.NewXmlDocument(FileName);
                XPathNavigator nav = navigationService.CreateNavigator(doc);
                XmlNamespaceManager nsmgr = navigationService.CreateNameSpaceManager(nav.NameTable);


                XConfig XmlConfig = new XConfig()
                {
                    FileName = FileName,
                    nsmgr = nsmgr,
                    nodeRefid = nodeRefid,
                    nodeName = nodeName,
                };
                logger.LogInformation(@$"Scanning file: {XmlConfig.FileName} Package {XmlConfig?.PackageName()}");

                if (packageAttributes != null && packageAttributes.Count >= 1)
                {
                    nodeRefid = packageAttributes?.FirstOrDefault(a => a.ParentRefId == "Package")?.ParentRefId?.ToString();
                    nodeName = packageAttributes?.FirstOrDefault(a => a.ParentNodeName == "DTS:Executable")?.ParentNodeName?.ToString();
                }


                // Collect all defined varibles in package


                xpath = "//DTS:Variables/child::*";
                if (XmlConfig != null)
                {
                    var allChildren = nav.Select(xpath, nsmgr);
                    XmlConfig.Children = allChildren;
                    logger.LogInformation($@"Running GetVariables");
                    packageVariables = processingService.GetVariables(XmlConfig);
                }
                if (configuration != null && configuration.GetSection("ScanElements").GetChildren().Any())
                {
                    xpaths = configuration.GetSection("ScanElements").Get<List<string>>();
                    if (xpaths != null && xpaths.Count >= 1)
                    {
                        foreach (string x in xpaths)
                        {
                            if (XmlConfig != null)
                            {

                                var allChildren = nav.Select(x, nsmgr);
                                XmlConfig.Children = allChildren;
                                logger.LogInformation($@"Running GetElements");
                                elements = processingService.GetElements(XmlConfig);
                                packageElements.AddRange(elements);
                            }
                        }
                    }

                }

            }

        }
        catch (Exception e)
        {
            logger.LogInformation($@"Program Error = {e}");
            throw;
        }


        /*
         Refactor all displays to be based off of CommandLineService.cs options
        */
        try
        {
            var elementAttributes = processingService?.GetAttributeListFromElements(packageElements);
            if (elementAttributes != null)
            {
                packageAttributes?.AddRange(elementAttributes);
            }
            if(databaseService != null)
            {
                if (packageAttributes != null)
                {
                    var returnCount = await databaseService.InsertAttributesAsync(packageAttributes);
                    Console.WriteLine($@"Writting {returnCount} attributes");
                }
                if (packageVariables != null)
                {
                    var returnCount = await databaseService.InsertVariablesAsync(packageVariables);
                    Console.WriteLine($@"Writting {returnCount} Variables");
                }
                if (packageElements != null)
                {
                    var returnCount = await databaseService.InsertElementsAsync(packageElements);
                    Console.WriteLine($@"Writting {returnCount} Elements");
                }

            }






        }
        catch (Exception e)
        {
            logger.LogInformation($@"Display Error = {e}");
            throw;
        }


    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DTSXDataLoader.Core.Models;
using DTSXDataLoader.Core.Service;
using DTSXDataLoader.Service;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace DTSXDataLoader.Tests.Services;
public class FileService_Returns_IsNotNull
{
    [Fact]
    public void FileService_Return_NotNull()
    {
        var logger = new LoggerFactory().CreateLogger<DocumentProcessingService>();
        IConfiguration configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
            .AddEnvironmentVariables()
            .Build();
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
      
        var fileService = serviceProvider.GetService<IFileService>();


        Assert.NotNull(fileService);
    }
}

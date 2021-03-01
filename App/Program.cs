using System;
using System.IO;
using System.Threading.Tasks;
using App.Services;
using App.Settings;
using AzureManagementLib;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SqlServerDacFxLib;
using SqlServerPackageLib;
using SqlServerPackageLib.Helpers;

namespace App
{
    public static class Program
    {
        public static async Task Main(string[] args)
        {
            using (var host = CreateHostBuilder(args).Build())
            {
                var facade = host.Services.GetRequiredService<IExportFacade>();
                await facade.ExportAsync();
            }

            Console.WriteLine("Press any key to exit !");
            Console.ReadKey();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((_, config) =>
                {
                    config.AddCommandLine(args);
                    config.AddEnvironmentVariables();
                    config.SetBasePath(Directory.GetCurrentDirectory());
                    var environment = Environment.GetEnvironmentVariable("ENVIRONMENT");
                    config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
                    config.AddJsonFile($"appsettings.{environment}.json", optional: true, reloadOnChange: true);
                })
                .ConfigureLogging((_, loggingBuilder) =>
                {
                    loggingBuilder.AddNonGenericLogger();
                })
                .ConfigureServices((hostingContext, services) =>
                {
                    services.AddOptions();
                    services.AddTransient<IExportFacade, ExportFacade>();
                    services.AddTransient<IDacFxWrapper, DacFxWrapper>();
                    services.AddTransient<IProcessHelper, ProcessHelper>();
                    services.AddTransient<IAzureSdkWrapper, AzureSdkWrapper>();
                    services.AddTransient<ISqlPackageWrapper, SqlPackageWrapper>();
                    services.Configure<ExportSettings>(hostingContext.Configuration.GetSection(nameof(ExportSettings)));
                })
                .UseConsoleLifetime();

        private static void AddNonGenericLogger(this ILoggingBuilder loggingBuilder)
        {
            var categoryName = typeof(Program).Namespace;
            var services = loggingBuilder.Services;
            services.AddSingleton(serviceProvider =>
            {
                var loggerFactory = serviceProvider.GetRequiredService<ILoggerFactory>();
                return loggerFactory.CreateLogger(categoryName);
            });
        }
    }
}

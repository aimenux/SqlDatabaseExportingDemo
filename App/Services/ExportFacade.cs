using System;
using System.Threading;
using System.Threading.Tasks;
using App.Settings;
using AzureManagementLib;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SqlServerDacFxLib;
using SqlServerPackageLib;

namespace App.Services
{
    public class ExportFacade : IExportFacade
    {
        private readonly IDacFxWrapper _dacFxWrapper;
        private readonly ISqlPackageWrapper _sqlPackageWrapper;
        private readonly IAzureSdkWrapper _azureSdkWrapper;
        private readonly ExportSettings _exportSettings;
        private readonly ILogger _logger;

        public ExportFacade(
            IDacFxWrapper dacFxWrapper,
            ISqlPackageWrapper sqlPackageWrapper,
            IAzureSdkWrapper azureSdkWrapper,
            IOptions<ExportSettings> options,
            ILogger logger)
        {
            _dacFxWrapper = dacFxWrapper;
            _sqlPackageWrapper = sqlPackageWrapper;
            _azureSdkWrapper = azureSdkWrapper;
            _exportSettings = options.Value;
            _logger = logger;
        }

        public async Task ExportAsync()
        {
            LogMessage(@"Exporting with SqlPackage");

            await _sqlPackageWrapper.ExportAsync(
                _exportSettings.SqlServerName, 
                _exportSettings.SqlDatabaseName,
                _exportSettings.BacPacFilePath1);

            LogMessage(@"Exporting with DacFx");

            var dacFxCancellationToken = BuildCancellationToken();
            await _dacFxWrapper.ExportAsync(
                _exportSettings.SqlServerName, 
                _exportSettings.SqlDatabaseName,
                _exportSettings.BacPacFilePath2,
                dacFxCancellationToken);

            LogMessage(@"Exporting with AzureSdk");

            var settings = GetAzureSdkSettings(_exportSettings);
            var azureCancellationToken = BuildCancellationToken();
            await _azureSdkWrapper.ExportAsync(settings, azureCancellationToken);
        }

        private void LogMessage(string message)
        {
            if (string.IsNullOrWhiteSpace(message)) return;
            _logger.LogInformation("An output was received: {message}", TrimMessage(message));
        }

        private static string TrimMessage(string message)
        {
            return message?.Trim(' ', '\n', '\r', '\t');
        }

        private static CancellationToken BuildCancellationToken()
        {
            var cancellationTokenSource = new CancellationTokenSource(TimeSpan.FromMinutes(5));
            return cancellationTokenSource.Token;
        }

        private static AzureSdkSettings GetAzureSdkSettings(ExportSettings exportSettings)
        {
            return new AzureSdkSettings
            {
                PrincipalClientId = exportSettings.AzureSettings.PrincipalClientId,
                PrincipalClientSecret = exportSettings.AzureSettings.PrincipalClientSecret,
                PrincipalTenantId = exportSettings.AzureSettings.PrincipalTenantId,
                SqlServerResourceGroupName = exportSettings.AzureSettings.SqlServerResourceGroupName,
                SqlServerName = exportSettings.AzureSettings.SqlServerName,
                DatabaseName = exportSettings.AzureSettings.DatabaseName,
                DatabaseLogin = exportSettings.AzureSettings.DatabaseLogin,
                DatabasePassword = exportSettings.AzureSettings.DatabasePassword,
                StorageResourceGroupName = exportSettings.AzureSettings.StorageResourceGroupName,
                StorageContainerName = exportSettings.AzureSettings.StorageContainerName,
                StorageName = exportSettings.AzureSettings.StorageName,
                BacPacBlobName = exportSettings.AzureSettings.BacPacBlobName,
                IpAddress = exportSettings.AzureSettings.FirewallRule.IpAddress,
                IpName = exportSettings.AzureSettings.FirewallRule.Name
            };
        }
    }
}

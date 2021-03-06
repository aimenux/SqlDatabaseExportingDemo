using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.Management.Fluent;
using Microsoft.Azure.Management.ResourceManager.Fluent.Authentication;
using Microsoft.Extensions.Logging;

namespace AzureManagementLib
{
    public class AzureSdkWrapper : IAzureSdkWrapper
    {
        private readonly ILogger _logger;

        public AzureSdkWrapper(ILogger logger)
        {
            _logger = logger;
        }

        public async Task ExportAsync(AzureSdkSettings settings, CancellationToken cancellationToken)
        {
            LogMessage(@"[Step 1] azure authentication");

            var credentials = GetAzureCredentials(settings);
            var azure = await Azure.Authenticate(credentials).WithDefaultSubscriptionAsync();

            LogMessage(@"[Step 2] azure sql server connection");

            var sqlServerResourceGroupName = settings.SqlServerResourceGroupName;
            var sqlServerName = settings.SqlServerName;
            var databaseName = settings.DatabaseName;
            var ipName = settings.IpName;
            var ipAddress = settings.IpAddress;
            var sqlServer = await azure.SqlServers.GetByResourceGroupAsync(sqlServerResourceGroupName, sqlServerName, cancellationToken);
            await sqlServer.FirewallRules.Define(ipName).WithIPAddress(ipAddress).CreateAsync(cancellationToken);
            var database = await sqlServer.Databases.GetAsync(databaseName, cancellationToken);

            LogMessage(@"[Step 3] azure storage account connection");

            var storageResourceGroupName = settings.StorageResourceGroupName;
            var storageName = settings.StorageName;
            var storageAccount = await azure.StorageAccounts.GetByResourceGroupAsync(storageResourceGroupName, storageName, cancellationToken);

            LogMessage(@"[Step 4] azure sql server export bacpac");

            var storageContainerName = settings.StorageContainerName;
            var bacPacBlobName = settings.BacPacBlobName;
            var databaseLogin = settings.DatabaseLogin;
            var databasePassword = settings.DatabasePassword;
            await database.ExportTo(storageAccount, storageContainerName, bacPacBlobName)
                .WithSqlAdministratorLoginAndPassword(databaseLogin, databasePassword)
                .ExecuteAsync(cancellationToken);
        }

        private static AzureCredentials GetAzureCredentials(AzureSdkSettings settings)
        {
            var clientId = settings.PrincipalClientId;
            var clientSecret = settings.PrincipalClientSecret;
            var tenantId = settings.PrincipalTenantId;
            var environment = settings.AzureEnvironment;
            return new AzureCredentialsFactory().FromServicePrincipal(clientId, clientSecret, tenantId, environment);
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
    }
}
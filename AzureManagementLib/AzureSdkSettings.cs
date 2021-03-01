using Microsoft.Azure.Management.ResourceManager.Fluent;

namespace AzureManagementLib
{
    public class AzureSdkSettings
    {
        public string PrincipalClientId { get; set; }

        public string PrincipalClientSecret { get; set; }

        public string PrincipalTenantId { get; set; }

        public string SqlServerName { get; set; }

        public string SqlServerResourceGroupName { get; set; }

        public string DatabaseName { get; set; }

        public string DatabaseLogin { get; set; }

        public string DatabasePassword { get; set; }

        public string StorageResourceGroupName { get; set; }

        public string StorageName { get; set; }

        public string StorageContainerName { get; set; }

        public string BacPacBlobName { get; set; }

        public string IpName { get; set; }

        public string IpAddress { get; set; }

        public AzureEnvironment AzureEnvironment { get; set; } = AzureEnvironment.AzureGlobalCloud;
    }
}
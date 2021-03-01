using System.Threading;
using System.Threading.Tasks;

namespace AzureManagementLib
{
    public interface IAzureSdkWrapper
    {
        Task ExportAsync(AzureSdkSettings settings, CancellationToken cancellationToken);
    }
}

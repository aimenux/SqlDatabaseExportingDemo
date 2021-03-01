using System.Threading;
using System.Threading.Tasks;

namespace SqlServerDacFxLib
{
    public interface IDacFxWrapper
    {
        Task ExportAsync(string sourceServerName, string sourceDatabaseName, string targetBacPacFilePath, CancellationToken cancellationToken);
    }
}

using System.Threading.Tasks;

namespace SqlServerPackageLib
{
    public interface ISqlPackageWrapper
    {
        Task ExportAsync(string sourceServerName, string sourceDatabaseName, string targetBacPacFilePath);
    }
}

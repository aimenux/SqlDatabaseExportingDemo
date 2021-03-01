using System.Threading.Tasks;
using SqlServerPackageLib.Helpers;

namespace SqlServerPackageLib
{
    public class SqlPackageWrapper : ISqlPackageWrapper
    {
        private readonly IProcessHelper _processHelper;

        public SqlPackageWrapper(IProcessHelper processHelper)
        {
            _processHelper = processHelper;
        }

        public Task ExportAsync(string sourceServerName, string sourceDatabaseName, string targetBacPacFilePath)
        {
            const string name = @"SqlPackage";
            var arguments = $@"/Action:Export /ssn:{sourceServerName} /sdn:{sourceDatabaseName} /tf:{targetBacPacFilePath}";
            _processHelper.RunProcess(name, arguments);
            return Task.CompletedTask;
        }
    }
}
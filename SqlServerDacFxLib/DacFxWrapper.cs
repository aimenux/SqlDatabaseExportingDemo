using System.Threading;
using System.Threading.Tasks;
using Microsoft.SqlServer.Dac;
using Microsoft.Extensions.Logging;

namespace SqlServerDacFxLib
{
    public class DacFxWrapper : IDacFxWrapper
    {
        private readonly ILogger _logger;

        public DacFxWrapper(ILogger logger)
        {
            _logger = logger;
        }

        public Task ExportAsync(string sourceServerName, string sourceDatabaseName, string targetBacPacFilePath, CancellationToken cancellationToken)
        {
            var connectionString = $@"Data Source={sourceServerName};Integrated Security=True";
            var services = new DacServices(connectionString);
            services.Message += (_, e) => LogMessage(e.Message?.Message);
            services.ExportBacpac(targetBacPacFilePath, sourceDatabaseName, cancellationToken: cancellationToken);
            return Task.CompletedTask;
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
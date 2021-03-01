using System.Diagnostics;
using Microsoft.Extensions.Logging;

namespace SqlServerPackageLib.Helpers
{
    public class ProcessHelper : IProcessHelper
    {
        private readonly ILogger _logger;

        public ProcessHelper(ILogger logger)
        {
            _logger = logger;
        }

        public void RunProcess(string name, string arguments)
        {
            var startInfo = new ProcessStartInfo
            {
                UseShellExecute = false,
                CreateNoWindow = true,
                RedirectStandardError = true,
                RedirectStandardOutput = true,
                FileName = $@"{name}",
                Arguments = $@"{arguments}"
            };

            var process = new Process
            {
                StartInfo = startInfo
            };

            process.ErrorDataReceived += (_, args) => LogProcessError(args.Data);
            process.OutputDataReceived += (_, args) => LogProcessOutput(args.Data);

            process.Start();
            process.BeginErrorReadLine();
            process.BeginOutputReadLine();
            process.WaitForExit();
            process.Close();
        }

        private void LogProcessOutput(string message)
        {
            if (string.IsNullOrWhiteSpace(message)) return;
            _logger.LogInformation("An output was received: {message}", TrimMessage(message));
        }

        private void LogProcessError(string message)
        {
            if (string.IsNullOrWhiteSpace(message)) return;
            _logger.LogError("An error has occurred: {message}", TrimMessage(message));
        }

        private static string TrimMessage(string message)
        {
            return message?.Trim(' ', '\n', '\r', '\t');
        }
    }
}
using PakMaster.Infrastructure.Diagnostics;
using System;
using System.Threading;
using System.Threading.Tasks;
using PakMaster.UI.Notifications;

namespace PakMaster.Core.Engines
{
    public static class RetocEngine
    {
        public static async Task ExecuteCommandAsync(string commandString, Action<string> outputCallback, CancellationToken ct = default)
        {
            GLogger.Here().Information("Starting Retoc command execution: {CommandString}", commandString);
            if (string.IsNullOrWhiteSpace(commandString))
            {
                await MessageManager.ShowWarning("Command cannot be empty.");
                return;
            }

            if (commandString.StartsWith("retoc.exe", StringComparison.OrdinalIgnoreCase))
            {
                commandString = commandString.Substring("retoc.exe".Length).TrimStart();
            }

            await ProcessEngine.RunToolAsync("retoc", "retoc.exe", commandString, outputCallback, ct);
        }
    }
}


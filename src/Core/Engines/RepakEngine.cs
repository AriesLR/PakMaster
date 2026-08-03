using PakMaster.Infrastructure.Diagnostics;
namespace PakMaster.Core.Engines
{
    public static class RepakEngine
    {
        public static async Task ExecuteCommandAsync(string commandString, Action<string> outputCallback, CancellationToken ct = default)
        {
            GLogger.Here().Information("Starting Repak command execution: {CommandString}", commandString);
            if (string.IsNullOrWhiteSpace(commandString))
            {
                await MessageManager.ShowWarning("Command cannot be empty.");
                return;
            }

            if (commandString.StartsWith("repak.exe", StringComparison.OrdinalIgnoreCase))
            {
                commandString = commandString.Substring("repak.exe".Length).TrimStart();
            }

            await ProcessEngine.RunToolAsync("repak", "repak.exe", commandString, outputCallback, ct);
        }
    }
}

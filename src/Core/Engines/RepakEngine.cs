namespace PakMaster.Core.Engines
{
    public static class RepakEngine
    {
        public static async Task ExecuteCommandAsync(string commandString, Action<string> outputCallback, CancellationToken ct = default)
        {
            GLogger.Here().Information("Starting Repak command execution: {CommandString}", commandString);
            if (string.IsNullOrWhiteSpace(commandString))
            {
                await MessageManager.ShowWarning(Lang.CommandCannotBeEmpty);
                return;
            }

            if (commandString.StartsWith("repak.exe", StringComparison.OrdinalIgnoreCase))
            {
                commandString = commandString["repak.exe".Length..].TrimStart();
            }

            string branchName = ConfigManager.CurrentSettings?.ActiveRepakBranch ?? "main";
            string exeName = "repak.exe";

            if (!string.IsNullOrWhiteSpace(branchName) && branchName != "main")
            {
                exeName = $"repak-{branchName}.exe";
            }

            await ProcessEngine.RunToolAsync("repak", exeName, commandString, outputCallback, ct);
        }
    }
}
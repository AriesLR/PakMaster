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
                await MessageManager.ShowWarning(Lang.CommandCannotBeEmpty);
                return;
            }

            if (commandString.StartsWith("retoc.exe", StringComparison.OrdinalIgnoreCase))
            {
                commandString = commandString.Substring("retoc.exe".Length).TrimStart();
            }

            await ProcessEngine.RunToolAsync("retoc", "retoc.exe", commandString, outputCallback, ct);
        }

        public static string BuildCommandString(ToolConfigModel settings)
        {
            if (settings == null) return "retoc.exe";

            string cmd = settings.ActiveRetocCommand ?? "manifest";
            string finalCmd = "retoc.exe";

            string inputPath = string.Empty;
            string outputPath = string.Empty;
            string targetId = string.Empty;
            string aesKey = string.Empty;
            string headerVer = string.Empty;
            string tocVer = string.Empty;
            string engineVer = string.Empty;
            string filterStr = string.Empty;
            string scriptCell = string.Empty;

            bool listAll = false;
            bool listHash = false;
            bool listPackage = false;
            bool listSize = false;
            bool listPath = false;
            bool listStore = false;

            bool noAssets = false;
            bool noShaders = false;
            bool noScriptObjects = false;
            bool noCompressShaders = false;
            bool dryRun = false;
            bool verbose = false;
            bool debug = false;
            bool noParallel = false;

            switch (cmd)
            {
                case "manifest":
                    inputPath = settings.RetocManifest.InputPath;
                    aesKey = settings.RetocManifest.AesKey;
                    headerVer = settings.RetocManifest.OverrideContainerHeaderVersion;
                    tocVer = settings.RetocManifest.OverrideTocVersion;
                    break;
                case "list":
                    inputPath = settings.RetocList.InputPath;
                    aesKey = settings.RetocList.AesKey;
                    headerVer = settings.RetocList.OverrideContainerHeaderVersion;
                    tocVer = settings.RetocList.OverrideTocVersion;
                    listAll = settings.RetocList.All;
                    listHash = settings.RetocList.Hash;
                    listPackage = settings.RetocList.Package;
                    listSize = settings.RetocList.Size;
                    listPath = settings.RetocList.Path;
                    listStore = settings.RetocList.Store;
                    break;
                case "verify":
                    inputPath = settings.RetocVerify.InputPath;
                    aesKey = settings.RetocVerify.AesKey;
                    headerVer = settings.RetocVerify.OverrideContainerHeaderVersion;
                    tocVer = settings.RetocVerify.OverrideTocVersion;
                    break;
                case "unpack":
                    inputPath = settings.RetocUnpack.InputPath;
                    outputPath = settings.RetocUnpack.OutputPath;
                    aesKey = settings.RetocUnpack.AesKey;
                    headerVer = settings.RetocUnpack.OverrideContainerHeaderVersion;
                    tocVer = settings.RetocUnpack.OverrideTocVersion;
                    verbose = settings.RetocUnpack.Verbose;
                    break;

                case "unpack-raw":
                    inputPath = settings.RetocUnpackRaw.InputPath;
                    outputPath = settings.RetocUnpackRaw.OutputPath;
                    aesKey = settings.RetocUnpackRaw.AesKey;
                    headerVer = settings.RetocUnpackRaw.OverrideContainerHeaderVersion;
                    tocVer = settings.RetocUnpackRaw.OverrideTocVersion;
                    break;
                case "pack-raw":
                    inputPath = settings.RetocPackRaw.InputPath;
                    outputPath = settings.RetocPackRaw.OutputPath;
                    aesKey = settings.RetocPackRaw.AesKey;
                    headerVer = settings.RetocPackRaw.OverrideContainerHeaderVersion;
                    tocVer = settings.RetocPackRaw.OverrideTocVersion;
                    break;
                case "to-legacy":
                    inputPath = settings.RetocToLegacy.InputPath;
                    outputPath = settings.RetocToLegacy.OutputPath;
                    aesKey = settings.RetocToLegacy.AesKey;
                    headerVer = settings.RetocToLegacy.OverrideContainerHeaderVersion;
                    tocVer = settings.RetocToLegacy.OverrideTocVersion;
                    engineVer = settings.RetocToLegacy.EngineVersion;
                    filterStr = settings.RetocToLegacy.Filter;
                    scriptCell = settings.RetocToLegacy.ScriptCell;
                    noAssets = settings.RetocToLegacy.NoAssets;
                    noShaders = settings.RetocToLegacy.NoShaders;
                    noScriptObjects = settings.RetocToLegacy.NoScriptObjects;
                    noCompressShaders = settings.RetocToLegacy.NoCompressShaders;
                    dryRun = settings.RetocToLegacy.DryRun;
                    verbose = settings.RetocToLegacy.Verbose;
                    debug = settings.RetocToLegacy.Debug;
                    noParallel = settings.RetocToLegacy.NoParallel;
                    break;
                case "to-zen":
                    inputPath = settings.RetocToZen.InputPath;
                    outputPath = settings.RetocToZen.OutputPath;
                    aesKey = settings.RetocToZen.AesKey;
                    headerVer = settings.RetocToZen.OverrideContainerHeaderVersion;
                    tocVer = settings.RetocToZen.OverrideTocVersion;
                    engineVer = settings.RetocToZen.EngineVersion;
                    filterStr = settings.RetocToZen.Filter;
                    scriptCell = settings.RetocToZen.ScriptCell;
                    verbose = settings.RetocToZen.Verbose;
                    debug = settings.RetocToZen.Debug;
                    noParallel = settings.RetocToZen.NoParallel;
                    break;
                case "get":
                    inputPath = settings.RetocGet.InputPath;
                    outputPath = settings.RetocGet.OutputPath;
                    targetId = settings.RetocGet.ChunkId;
                    aesKey = settings.RetocGet.AesKey;
                    headerVer = settings.RetocGet.OverrideContainerHeaderVersion;
                    tocVer = settings.RetocGet.OverrideTocVersion;
                    break;
                case "dump-test":
                    inputPath = settings.RetocDumpTest.InputPath;
                    outputPath = settings.RetocDumpTest.OutputPath;
                    targetId = settings.RetocDumpTest.PackageId;
                    aesKey = settings.RetocDumpTest.AesKey;
                    headerVer = settings.RetocDumpTest.OverrideContainerHeaderVersion;
                    tocVer = settings.RetocDumpTest.OverrideTocVersion;
                    break;
                case "gen-script-objects":
                    inputPath = settings.RetocGenScriptObjects.InputPath;
                    outputPath = settings.RetocGenScriptObjects.OutputPath;
                    aesKey = settings.RetocGenScriptObjects.AesKey;
                    headerVer = settings.RetocGenScriptObjects.OverrideContainerHeaderVersion;
                    tocVer = settings.RetocGenScriptObjects.OverrideTocVersion;
                    engineVer = settings.RetocGenScriptObjects.EngineVersion;
                    break;
                case "print-script-objects":
                    inputPath = settings.RetocPrintScriptObjects.InputPath;
                    aesKey = settings.RetocPrintScriptObjects.AesKey;
                    headerVer = settings.RetocPrintScriptObjects.OverrideContainerHeaderVersion;
                    tocVer = settings.RetocPrintScriptObjects.OverrideTocVersion;
                    break;
                case "asset-registry":
                    inputPath = settings.RetocAssetRegistry.InputPath;
                    aesKey = settings.RetocAssetRegistry.AesKey;
                    headerVer = settings.RetocAssetRegistry.OverrideContainerHeaderVersion;
                    tocVer = settings.RetocAssetRegistry.OverrideTocVersion;
                    break;
            }

            if (!string.IsNullOrWhiteSpace(aesKey)) finalCmd += $" -a \"{aesKey}\"";
            if (!string.IsNullOrWhiteSpace(headerVer)) finalCmd += $" --override-container-header-version {headerVer}";
            if (!string.IsNullOrWhiteSpace(tocVer)) finalCmd += $" --override-toc-version {tocVer}";

            finalCmd += $" {cmd}";

            if (cmd == "list")
            {
                if (listAll) finalCmd += " --all";
                if (listHash) finalCmd += " --hash";
                if (listPackage) finalCmd += " --package";
                if (listSize) finalCmd += " --size";
                if (listPath) finalCmd += " --path";
                if (listStore) finalCmd += " --store";
            }

            bool needsEngineVer = cmd == "to-legacy" || cmd == "to-zen" || cmd == "gen-script-objects";
            if (needsEngineVer && !string.IsNullOrWhiteSpace(engineVer)) finalCmd += $" --version {engineVer}";
            
            bool needsFilter = cmd == "to-legacy" || cmd == "to-zen";
            if (needsFilter && !string.IsNullOrWhiteSpace(filterStr)) finalCmd += $" --filter \"{filterStr}\"";
            
            bool needsVerbose = cmd == "unpack" || cmd == "to-legacy" || cmd == "to-zen";
            if (needsVerbose && verbose) finalCmd += " -v";

            if (cmd == "to-legacy")
            {
                if (!string.IsNullOrWhiteSpace(scriptCell)) finalCmd += $" --script-cell \"{scriptCell}\"";
                if (noAssets) finalCmd += " --no-assets";
                if (noShaders) finalCmd += " --no-shaders";
                if (noScriptObjects) finalCmd += " --no-script-objects";
                if (noCompressShaders) finalCmd += " --no-compres-shaders";
                if (dryRun) finalCmd += " -d";
            }
            else if (cmd == "to-zen")
            {
                if (!string.IsNullOrWhiteSpace(scriptCell)) finalCmd += $" --script-cell \"{scriptCell}\"";
            }

            bool needsDebugParallel = cmd == "to-legacy" || cmd == "to-zen";
            if (needsDebugParallel)
            {
                if (debug) finalCmd += " --debug";
                if (noParallel) finalCmd += " --no-parallel";
            }

            if (!string.IsNullOrWhiteSpace(inputPath)) finalCmd += $" \"{inputPath}\"";

            if (cmd == "get" && !string.IsNullOrWhiteSpace(targetId)) finalCmd += $" \"{targetId}\"";

            if (cmd == "dump-test" && !string.IsNullOrWhiteSpace(outputPath)) finalCmd += $" \"{outputPath}\"";
            if (cmd == "dump-test" && !string.IsNullOrWhiteSpace(targetId)) finalCmd += $" \"{targetId}\"";

            bool needsOutput = cmd == "unpack" || cmd == "unpack-raw" || cmd == "pack-raw" || cmd == "to-legacy" || cmd == "to-zen" || cmd == "get" || cmd == "dump-test" || cmd == "gen-script-objects";
            if (cmd != "dump-test" && needsOutput && !string.IsNullOrWhiteSpace(outputPath))
            {
                string modifiedOutputPath = outputPath;

                bool shouldAppend = (cmd == "unpack" || cmd == "unpack-raw");
                if ((cmd == "to-legacy" || cmd == "to-zen") && string.IsNullOrEmpty(System.IO.Path.GetExtension(modifiedOutputPath))) 
                {
                    shouldAppend = true;
                }

                if (shouldAppend && !string.IsNullOrWhiteSpace(inputPath))
                {
                    string baseName = System.IO.Path.GetFileNameWithoutExtension(inputPath.TrimEnd(System.IO.Path.DirectorySeparatorChar, System.IO.Path.AltDirectorySeparatorChar));
                    modifiedOutputPath = System.IO.Path.Combine(modifiedOutputPath, baseName);
                }

                finalCmd += $" \"{modifiedOutputPath}\"";
            }

            return finalCmd;
        }
    }
}
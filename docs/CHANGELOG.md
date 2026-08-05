## [v1.0.0-rc.1]

> [!WARNING]
> **Pre-Release Build:** Expect bugs! Please [report any issues](https://github.com/AriesLR/PakMaster/issues/new?template=issue.md) you run into so I can get them fixed.

> [!NOTE]
> This release represents a nearly complete rewrite of the application. Because the scope of the rewrite was so extensive, it is difficult to articulate every single change. Consider this a fresh slate; all future updates will resume with detailed changelogs.

### Added
- Integrated a custom app template, introducing a suite of features including language switching, accent colors, interface scaling, and customizable application behaviors.
- Created a new application icon.

### Changed
- Changed how the application is packaged; it now outputs as a single `.exe` file rather than an unpackaged folder, significantly improving portability.
- Migrated the project framework from .NET 8 to .NET 10.
- Integrated CliWrap for improved command-line process handling.
- Implemented much deeper configurations, including the ability to save input/output paths between sessions (Addresses [Issue #2](https://github.com/AriesLR/PakMaster/issues/2)).

### Fixed
- Fixed a bug related to an outdated Repak version (Addresses [Issue #5](https://github.com/AriesLR/PakMaster/issues/5)).

### Removed
- Totally removed support for ZenTools and UnrealPak, as Retoc is now powerful enough to replace the need for them entirely (Addresses [Issue #4](https://github.com/AriesLR/PakMaster/issues/4)).

## Virus Scan

> [!NOTE]
> [VirusTotal Report (1/70)](https://www.virustotal.com/gui/file/YOUR_HASH_HERE)
>
> The single flag from **SecureAge** is a false positive, I've had this vendor flag another project of mine in the past. A false positive report has been submitted to the vendor and the flag should clear within a few days.


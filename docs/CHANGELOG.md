## [v1.0.0-rc.1]

###### *Note: This release represents a nearly complete rewrite of the application. Because the scope of the rewrite was so extensive, it is difficult to articulate every single change. Consider this a fresh slate; all future updates will resume with detailed changelogs.*

### Added
- Integrated a custom app template, introducing a suite of features including language switching, accent colors, interface scaling, and customizable application behaviors.
- Created a new application icon.

### Changed
- Changed how the application is packaged; it now outputs as a single `.exe` file rather than an unpackaged folder, significantly improving portability.
- Migrated the project framework from .NET 8 to .NET 10.
- Integrated CliWrap for improved command-line process handling.
- Implemented much deeper configurations, including the ability to save input/output paths between sessions (Addresses #2).

### Fixed
- Fixed a bug related to an outdated Repak version (Addresses #5).

### Removed
- Totally removed support for ZenTools and UnrealPak, as Retoc is now powerful enough to replace the need for them entirely (Addresses #4).

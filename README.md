# PakMaster

#

#### Hopefully the ultimate solution to unreal engine pak modding (.pak/.ucas/.utoc)

## Table of Contents

- [Features](#features)
- [Planned Features](#planned-features)
- [Getting Started](#getting-started)
    - [Installer Version](#installer-version)
    - [Portable Version](#portable-version)
- [Acknowledgements](#acknowledgements)
- [License](#license)
- [Tips](#tips)
- [Notes](#notes)

## Features

- **Pak File Operations**
  - Ability to **pack** and **unpack** `.pak` files.
  - Pack folders into a `.pak` file.
  - Unpack the contents of a `.pak` file into a folder.[^1]

- **AES Key Support**
  - Support for games that require an AES key to unpack files.
  - Easily enter and save your AES key for future use, streamlining the process for supported games.

- **User-Friendly GUI**
  - Intuitive and easy-to-use graphical user interface.
  - **Folder selection**: Choose input and output folders for packing/unpacking.

- **CLI Output Display**
  - Since PakMaster is reliant on CLI tools, their outputs are displayed in the GUI so users can troubleshoot issues if they arise.

## Planned Features

- `.ucas` & `.utoc` support - Currently only `.pak` is supported.

## Getting Started

- Section WIP

### Installer Version

- Section WIP

### Portable Version

- Section WIP

## Acknowledgements
- [repak](https://github.com/trumank/repak) - For the Unreal Engine .pak file library and CLI in rust.
    - [unpak](https://github.com/bananaturtlesandwich/unpak) - (Used by repak) Original crate featuring read-only pak operations.
    - [rust-u4pak](https://github.com/bananaturtlesandwich/unpak) - (Used by repak) rust-u4pak's README detailing the pak file layout.
    - [jieyouxu](https://github.com/jieyouxu) - (Used by repak) for serialization implementation of the significantly more complex V11 index.

- [ZenTools](https://github.com/LongerWarrior/ZenTools) - For the Tools for extracting cooked packages from the IoStore container files.

## License

[MIT License](LICENSE)

## Tips
[Buy Me a Coffee](https://www.buymeacoffee.com/arieslr)

## Notes

- No Notes, that's probably good, right?

#

[^1]: From Repak's notes: UnrealPak includes a directory entry in the full directory index for all parent directories back to the pak root for a given file path regardless of whether those directories contain any files or just other directories. repak only includes directories that contain files. So far no functional differences have been observed as a result
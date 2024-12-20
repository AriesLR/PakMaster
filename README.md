# PakMaster

#

#### Hopefully the ultimate solution to unreal engine pak modding (.pak/.ucas/.utoc)

## Readme under construction
<img src="https://i.imgflip.com/1u2oyu.jpg" alt="Under Construction" width="300">

## Table of Contents

- [How It Works](#how-it-works)
- [OS Support](#os-support)
- [Features](#features)
- [Planned Features](#planned-features)
- [Getting Started](#getting-started)
  - [How To Use](#how-to-use)
    - [Unpacking](#unpacking)
    - [Repacking](#repacking)
- [Acknowledgements](#acknowledgements)
- [License](#license)
- [Tips](#tips)
- [Notes](#notes)

## How It Works

PakMaster leverages the existing tools [repak](https://github.com/trumank/repak) and [ZenTools](https://github.com/LongerWarrior/ZenTools) to pack and unpack files.
PakMaster does not come with either of these tools, but upon launching PakMaster the latest version supported by PakMaster is downloaded automatically.
By using PakMaster you also have to abide by repak and ZenTools' licenses, not just PakMaster's.

## OS Support
- Windows 10/11
  - Older versions of windows may still work.

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

To get started with **PakMaster**, download the [Latest Version](https://github.com/arieslr/Aetherium/releases/latest). Once downloaded, extract the contents of `PakMaster-x86-windows-portable.zip`. This will create a `PakMaster` folder, which you can place anywhere on your computer.

### How To Use

1. Open `PakMaster.exe`.
2. Select your **input** (Packed) folder.
3. Select your **output** (Unpacked) folder.

#### Unpacking
- Choose a `.pak` file from the **Input** list on the left and click **"Unpack"**.

#### Repacking
- Select the folder you'd like to pack from the **Output** list on the right and click **"Repack"**.
 
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
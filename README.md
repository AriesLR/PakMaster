# PakMaster - A GUI Wrapper for Repak and ZenTools

#### Hopefully the ultimate solution to Unreal Engine 5 modding (.pak/.ucas/.utoc)

## Table of Contents

- [How It Works](#how-it-works)
  - [Tools Used](#tools-used)
- [OS Support](#os-support)
- [Features](#features)
- [Planned Features](#planned-features)
- [Known Issues](#known-issues)
- [Getting Started](#getting-started)
  - [How To Use](#how-to-use)
    - [AES Key](#aes-key)
    - [Unpacking](#unpacking)
    - [Repacking](#repacking)
- [Screenshots](#screenshots)
- [Acknowledgements](#acknowledgements)
- [License](#license)
- [Tips](#tips)

## How It Works

PakMaster simplifies the process of packing and unpacking files by providing a GUI on top of the existing tools [Repak](https://github.com/trumank/repak) and [ZenTools](https://github.com/LongerWarrior/ZenTools). 
While these tools handle the core functionality, PakMaster streamlines the user experience, making repetitive tasks quicker and more accessible.

PakMaster does not include Repak or ZenTools. 
Instead, it automatically downloads the latest supported versions of these tools upon launch. 
By using PakMaster, users must also adhere to the licenses of Repak and ZenTools in addition to PakMaster's own.

### Tools Used

**Unpacking (.pak)** - [Repak](https://github.com/trumank/repak)

**Repacking (.pak)** - [Repak](https://github.com/trumank/repak)

**Unpacking (.ucas/.utoc)** - [ZenTools](https://github.com/LongerWarrior/ZenTools)

**Repacking (.ucas/.utoc)** - Currently Unsupported

## OS Support
- Windows 10/11
  - Older versions of windows may still work.

## Features

- **Pak File Operations**
  - **Unpack** the contents of a `.pak` file into a folder.[^1]
  - **Pack** folders into a `.pak` file.

- **IoStore File Unpacking**
  - **Unpack** the contents of `.ucas` & `.utoc` files into a folder.

- **AES Key Support**
  - Support for games that require an AES key to unpack files.
  - Easily enter and save your AES key for future use, streamlining the process for supported games.

- **User-Friendly GUI**
  - Intuitive and easy-to-use graphical user interface.
  - **Folder selection**: Choose input and output folders for packing/unpacking.

- **CLI Output Display**
  - Since PakMaster is reliant on CLI tools, their outputs are displayed in the GUI so users can troubleshoot issues if they arise.
    - Please see [notes](#issues) about this.

## Planned Features

- `.ucas` & `.utoc` packing - Currently only unpacking is supported.

- Ability to choose pak version, currently PakMaster is using V11, possible options are: `V0, V1, V2, V3, V4, V5, V6, V7, V8A, V8B, V9, V10, V11`

- Load Order Editor - if your input folder is set to something like a game's mod folder you could easily modify load orders via PakMaster.

## Known Issues

- Packing IoStore Assets is not supported yet.

## Getting Started

To get started with **PakMaster**, download the [Latest Release](https://github.com/AriesLR/PakMaster/releases/latest). Once downloaded, extract the contents of `PakMaster-x86-windows-portable.zip`. This will create a `PakMaster` folder, which you can place anywhere on your computer.

### How To Use

1. Open `PakMaster.exe`.

2. Select your **input** (Packed) folder.

3. Select your **output** (Unpacked) folder.

#### AES Key

Some games require an AES Key to unpack files, if you don't know how to find your AES Key, look into [AESDumpster](https://github.com/GHFear/AESDumpster). 

If you don't require an AES Key you can ignore this section.

To set your AES Key, open the AES Key Settings and set your key in the "Repak AES Key" section and in the "ZenTools AES Key" section, for ZenTools it requires a GUID:Hex format, some games you can just leave the GUID as zeros hence why PakMaster defaults to that, but the hex still needs to be set if the game requires an AES Key.

#### Unpacking

- **.pak**
  - Choose a `.pak` file from the **Input** list on the left and click **"Unpack"**.

- **.ucas/.utoc**
  - Make sure you have an **Input** folder selected and click **Unpack**.[^2]

#### Repacking

- **.pak**
  - Select the folder you'd like to pack from the **Output** list on the right and click **"Repack"**.

- **.ucas/.utoc**
  - Currently Unsupported.

## Screenshots

![PakMaster Main Window](https://raw.githubusercontent.com/AriesLR/PakMaster/refs/heads/main/docs/images/pakmaster-main.png)

![PakMaster AES Key Settings](https://raw.githubusercontent.com/AriesLR/PakMaster/refs/heads/main/docs/images/pakmaster-aeskeys.png)
 
## Acknowledgements
- [Repak](https://github.com/trumank/repak) - For the Unreal Engine .pak file library and CLI in rust.
    - [unpak](https://github.com/bananaturtlesandwich/unpak) - (Used by Repak) Original crate featuring read-only pak operations.
    - [rust-u4pak](https://github.com/bananaturtlesandwich/unpak) - (Used by Repak) rust-u4pak's README detailing the pak file layout.
    - [jieyouxu](https://github.com/jieyouxu) - (Used by Repak) for serialization implementation of the significantly more complex V11 index.

- [ZenTools](https://github.com/LongerWarrior/ZenTools) - For the Tools for extracting cooked packages from the IoStore container files.

## License

[MIT License](LICENSE)

## Tips
[Buy Me a Coffee](https://www.buymeacoffee.com/arieslr)

## Issues
If any issues do happen, PLEASE report them here first. It is very likely an issue on my part and if it is not I'll relay the information to the authors of the responsible dependency. Don't bother other authors about PakMaster as I am entirely responsible for it. If you are 100% sure that it's an issue with ZenTools or Repak then you can create an issue on their repos, but if you are not sure about it always report it to me.

#

<img src="https://i.imgflip.com/1u2oyu.jpg" alt="I like this doge" width="100">

#

[^1]: From Repak's notes: UnrealPak includes a directory entry in the full directory index for all parent directories back to the pak root for a given file path regardless of whether those directories contain any files or just other directories. Repak only includes directories that contain files. So far no functional differences have been observed as a result

[^2]: If unpacking a mod or group of files only have that mod or group's files in the folder - this will unpack all IoStore assets in the folder that you selected. For example, have only ExampleMod.ucas/ExampleMod.utoc in the folder.
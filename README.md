# PakMaster - A GUI Wrapper for Repak, ZenTools, and UnrealPak

#### Hopefully the ultimate solution to Unreal Engine 5 modding (.pak/.ucas/.utoc)

> [!IMPORTANT]
> PakMaster assumes you have some knowledge about UE5 modding and Unreal Engine. If you are new to Unreal Engine modding, I suggest starting [HERE](https://github.com/Dmgvol/UE_Modding/#ue45-modding-guides).

## Table of Contents

- [How It Works](#how-it-works)
- [Requirements](#requirements)
  - [Software](#software)
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
- [Tools Used](#tools-used)
- [Acknowledgements](#acknowledgements)
- [FAQ](#faq)
- [License](#license)
- [Tips](#tips)

## How It Works

PakMaster simplifies the process of packing and unpacking files by providing a GUI on top of the existing tools [Repak](https://github.com/trumank/repak), [ZenTools](https://github.com/LongerWarrior/ZenTools), and UnrealPak. 
While these tools handle the core functionality, PakMaster streamlines the user experience, making repetitive tasks quicker and more accessible.

PakMaster does not include Repak, ZenTools, or UnrealPak. 
Instead, it automatically downloads the latest supported versions of Repak and ZenTools upon launch. UnrealPak comes with Unreal Engine, you are required to download this on your own if you require the ability to package cooked assets.

By using PakMaster, users must also adhere to the licenses of Repak and ZenTools in addition to PakMaster's own. Unreal Engine has their own license.

## Requirements

### Software

- [.NET 8.0 Runtime](https://dotnet.microsoft.com/en-us/download/dotnet/8.0)
  - This is probably already installed on your system.

- [Unreal Engine](https://www.unrealengine.com/en-US/download) v4.25+[^1][^2]
  - This is only required for IoStore **packing**, unpacking works without it.

### OS Support

- Windows 10/11
  - Older versions of windows may still work.

## Features

- **Pak File Operations**
  - **Unpack** the contents of a `.pak` file into a folder.[^3]
  - **Pack** folders into a `.pak` file.

- **IoStore File Operations**
  - **Unpack** the contents of `IoStore` files into a folder.
  - **Pack** the contents of `IoStore` files into a `.pak`/`.ucas`/`.utoc` stack.

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

- Ability to choose pak version, currently PakMaster is using V11, possible options are: `V0, V1, V2, V3, V4, V5, V6, V7, V8A, V8B, V9, V10, V11`

- ~~Load Order Editor - if your input folder is set to something like a game's mod folder you could easily modify load orders via PakMaster.~~
  - This is outside the scope of this project, maybe once I'm satisfied with the state of PakMaster I'll make something like that as a stanadlone app.

## Known Issues

- None, see [Issues](#issues) if you find any.

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

- **Pak Files**
  - Choose a `.pak` file from the **Input** list on the left and click **"Unpack"**.

- **IoStore Files**
  - Make sure you have an **Input** folder selected and click **Unpack**.[^4]

#### Repacking

- **Pak files**
  - Select the folder you'd like to pack from the **Output** list on the right and click **"Repack"**.

- **IoStore Files**[^5]
  - Switch to IoStore Mode and click **Repack** configure your file paths as needed and then click **Pack**
    - This will require you to have above average knowledge. [This](https://www.youtube.com/watch?v=89s0akNvpU4) might be a good place to start.

## Screenshots

### Normal Mode Unpacking
![Normal Mode Unpacking](https://raw.githubusercontent.com/AriesLR/PakMaster/refs/heads/main/docs/images/pakmaster-normal-unpack.png)

### Normal Mode Packing
![Normal Mode Repacking](https://raw.githubusercontent.com/AriesLR/PakMaster/refs/heads/main/docs/images/pakmaster-normal-repack.png)

### IoStore Mode Unpacking
![IoStore Mode Unpacking](https://raw.githubusercontent.com/AriesLR/PakMaster/refs/heads/main/docs/images/pakmaster-iostore-unpack.png)

### IoStore Mode Packing
![IoStore Mode Repacking](https://raw.githubusercontent.com/AriesLR/PakMaster/refs/heads/main/docs/images/pakmaster-iostore-repack.png)

## Tools Used

**Unpacking (Pak Files)** - [Repak](https://github.com/trumank/repak)

**Repacking (Pak Files)** - [Repak](https://github.com/trumank/repak)

**Unpacking (IoStore Files)** - [ZenTools](https://github.com/LongerWarrior/ZenTools)

**Repacking (IoStore Files)** - UnrealPak (Requires Unreal Engine)
 
## Acknowledgements
- [Repak](https://github.com/trumank/repak) - For the Unreal Engine .pak file library and CLI in rust.
    - [unpak](https://github.com/bananaturtlesandwich/unpak) - (Used by Repak) Original crate featuring read-only pak operations.
    - [rust-u4pak](https://github.com/bananaturtlesandwich/unpak) - (Used by Repak) rust-u4pak's README detailing the pak file layout.
    - [jieyouxu](https://github.com/jieyouxu) - (Used by Repak) for serialization implementation of the significantly more complex V11 index.

- [ZenTools](https://github.com/LongerWarrior/ZenTools) - For the Tools for extracting cooked packages from the IoStore container files.
  - [LongerWarrior](https://github.com/LongerWarrior/) - Special thanks to LongerWarrior for maintaining ZenTools.

- [Buckminsterfullerene02](https://github.com/Buckminsterfullerene02/) - For the [UE Modding Tools](https://github.com/Buckminsterfullerene02/UE-Modding-Tools/) databank and contributing to the [UE Modding Guide](https://github.com/Dmgvol/UE_Modding#ue45-modding-guides).
  - [elbadcode](https://github.com/elbadcode) - For contributing to the UE Modding Tools databank.
  - [spuds](https://github.com/bananaturtlesandwich) - For contributing to the UE Modding Tools databank.

- [Dmgvol](https://github.com/Dmgvol/) - For the [UE Modding Guide](https://github.com/Dmgvol/UE_Modding#ue45-modding-guides).

- [Narknon](https://github.com/narknon) - For helping me understand how UE Modding works regarding IoStore Files.
  - Link may be wrong, our conversations took place on discord, but I think this is their GitHub.

- [1Armageddon1](https://github.com/1armageddon1) - For being the OG tester for the tool as well as helping give insight on how the tool should work. Wouldn't be nearly as far as I am progress-wise without them.

## FAQ
- **Q:** Why not just use UnrealPak for everything?
  - **A:** The reason I don't use UnrealPak to handle everything is to keep the majority of PakMaster lightweight, if Unreal Engine was required for everything the tool would be less accessible.

## License

[MIT License](LICENSE)

## Tips
[Buy Me a Coffee](https://www.buymeacoffee.com/arieslr)

## Issues
If any issues do happen, PLEASE report them here first. It is very likely an issue on my part and if it is not I'll relay the information to the authors of the responsible dependency. Don't bother other authors about PakMaster as I am entirely responsible for it. If you are 100% sure that it's an issue with ZenTools or Repak then you can create an issue on their repos, but if you are not sure about it always report it to me.

<img src="https://i.imgflip.com/1u2oyu.jpg" alt="I like this doge" width="100">

#

[^1]: Engine version should match game version, if your game is Unreal Engine 5.1, you should install Unreal Engine 5.1. Some games may work on similar versions, for example Stalker 2 at the time of writing this seems to work with either UE 5.1 or UE 5.1.1.

[^2]: While PakMaster only officially supports UE5+, IoStore packing should work for any engine versions higher than or equal to UE 4.25.

[^3]: From Repak's notes: UnrealPak includes a directory entry in the full directory index for all parent directories back to the pak root for a given file path regardless of whether those directories contain any files or just other directories. Repak only includes directories that contain files. So far no functional differences have been observed as a result

[^4]: If unpacking a mod or group of files only have one mod or group's files in the folder - this will unpack all IoStore assets in the folder that you selected. For example, have only ExampleMod.pak/ExampleMod.ucas/ExampleMod.utoc in the folder.

[^5]: IoStore File Packaging requires Unreal Engine.
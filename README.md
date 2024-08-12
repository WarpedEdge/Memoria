![memoria-logo.jpg](https://github.com/user-attachments/assets/625396cc-7553-4607-9626-9f98917d6073)

### The Memoria engine is an open-source community rewrite of Final Fantasy IX's engine that aims to add features, fix bugs and expand modding possibilities. It includes built-in improvements (QoL, camera, framerate, audio, controller, UI, options, cheats, shaders...), bugfixes, a mod manager/downloader and a model viewer. [More info here](https://github.com/Albeoris/Memoria/wiki/Project-Overview)

## Features


- Main features:
    - Easy to use, everything optional, main settings in the launcher, the rest in Memoria.ini
    - **Mod Manager/downloader**, using individual mod folders
    - **Widescreen (for all resolutions)**
    - **Higher framerate (60fps+)**
    - **Smooth and stabilized camera movement**
    - **Improved rendering**: Anti-aliasing, texture filtering, layer edges, shaders
    - **Triple Triad** / Tetra Triad option
    - Better **sound engine**, volume control
    - **Many, many bugfixes**
- UI
    - **Font change (includes the original PSX font)**
    - **Battle speed change & Swirl skip**
    - **Turn-based mode / Simultaneous mode**
    - **Controller support with full analog movement**
    - **Battle UI layouts (includes original PSX layout)**
    - More items displayed at once
    - PSX disc change screens
    - Model Viewer
- Modding support for:
    - UI, backgrounds, shaders, texture, FMV modding/upscale (e.g. Moguri)
    - Voice acting (e.g. WIP project ECHO-S)
    - Translations
    - Expanded features for mods
    - Moddable game data (StreamingAssets\Data\) and abilities (StreamingAssets\Scripts\)
    - More character playable mods (e.g. Playable character pack, Tantalus...)
    - Import/export text/audio/textures
- Optional Cheats:
    - **Stealing 100% success**
    - Enable/Disable vanilla cheats
    - Easy minigames (rope, frogs, racing...)
    - Excalibur II time limit removal
    - Cards: lower randomness, card limit raised, auto discard cards
    - Manual Trance
 
![screenshots.jpg](https://github.com/user-attachments/assets/2bacaa4c-c380-44a8-bc67-9814594154d0)


## Use

- **INSTALL**: Download and run [Memoria.Patcher.exe](https://github.com/Albeoris/Memoria/releases/)
  - Automatically finds the game path from Windows registry (if you've launched the game once and haven't moved the install folder)
  - If not, launch the patcher from the game folder, or provide a custom path in command line: 'Memoria.Patcher.exe "gameDirectory"'
  - Mac users: The launcher has been shown to be currently only compatible with Crossover in Windows 10 32bit mode (Whiskey and 64bit give an error "mono-io-layer-error")
- **UPDATE**: with the latest patcher exe or use "check for updates" in the launcher
- **CONFIGURE**: Most crucial options and cheats are embedded in the game launcher
  - More in-depth configuration is available in the file Memoria.ini (in the game folder)

## DEVELOPERS

- [INFO for developers](https://github.com/Albeoris/Memoria/wiki/Developer-instructions)
- [Knowledge base](../../wiki#knowledge-base)

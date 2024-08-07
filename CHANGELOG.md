# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.1.0/), and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [Unreleased]

## [1.2.0] - 2024-08-07

### Added

- Ability to pick [innate enchantments](https://stardewvalleywiki.com/Forge#Innate_enchantments). Togglable via GMCM, enabled by default.

### Changed

- Update to Stardew Valley 1.6/SMAPI 4.
- Remove dependency on AtraCore/AtraBase.

### Fixed

- Keybinds for enchantment picker (e.g. controller L/R) now work correctly.

### Removed

- Patches for [Enchantable Scythes](https://www.nexusmods.com/stardewvalley/mods/10668) as these were broken in recent updates. Compatibility may be added back in a future update.

## [1.1.3] - 2023-03-29

### Added

- Last official release of original mod. For earlier history, refer to the [original project's changelog](https://github.com/atravita-mods/StardewMods/blob/8a885849ae3dd933bf795262368cadadc6bea8f8/ForgeMenuChoice/ForgeMenuChoice/docs/Changelog.md).


Changelog
==============

#### Todo
1. Document the tooltip override feature + how to replace the textures in Content Patcher.
2. Make sure all strings I want translated are complete, and ask for translations.

#### Known issues
* Controllers probably won't be able to lock onto the arrows properly, owing to the....nontraditional menu setup here. It's likely not something I can fix, either. Use the LeftArrow/RightArrow keybinds (shoulder buttons by default).
* I apply the chosen enchantment when forging, not before. So if you're using a predictor, it'll just show what the game would have picked. (This goes for Many Enchantment's debug logging as well.)

#### Version 1.2.0
* Update to Stardew 1.6
* Added innate enchantments.
* Performance optimizations.

[Unreleased]: https://github.com/focustense/StardewForgeMenuChoice/compare/v1.2.0...HEAD
[1.2.0]: https://github.com/focustense/StardewForgeMenuChoice/compare/v1.1.3...v1.2.0
[1.1.3]: https://github.com/focustense/StardewForgeMenuChoice/tree/v1.1.3
<div align="center">
  <h2><strong>SWS2 Flashlight</strong></h2>
  <h3>Press <kbd>F</kbd> to toggle a flashlight for SwiftlyS2.</h3>
</div>

<p align="center">
  <img src="https://img.shields.io/github/downloads/XBDJ504764827/sws2-flashlight/total" alt="Downloads">
  <img src="https://img.shields.io/github/v/release/XBDJ504764827/sws2-flashlight" alt="Release">
  <img src="https://img.shields.io/github/license/XBDJ504764827/sws2-flashlight" alt="License">
</p>

## Features

- Press <kbd>F</kbd> to toggle your flashlight (press again to turn it off)
- Flashlight follows your eye position and view direction in real time
- Automatic cleanup on death, team switch, disconnect, and map change
- Hot-reloadable configuration (`config.jsonc`)
- Light only visible to the owner by default (configurable)
- Commands: `/fl`, `/flashlight`, `/fl2`
- Translations: English + 简体中文

## Requirements

- [SwiftlyS2](https://github.com/swiftly-solution/swiftlys2) running on a CS2 dedicated server

## Installation

1. Download the latest release zip.
2. Extract the folder into `csgo/addons/swiftlys2/plugins/`:

```
csgo/addons/swiftlys2/plugins/
└── sws2-flashlight/
```

3. Start / reload the server, or run `sw plugins load sws2-flashlight` in console.

## Configuration

First run creates `csgo/addons/swiftlys2/configs/sws2-flashlight/config.jsonc`
(from the bundled template `resources/templates/config.template.jsonc`):

```jsonc
{
  "sws2-flashlight": {
    // Enable debug logging
    "Debug": false,

    "Behavior": {
      // Light update interval in ticks (64 ticks/s). 1 = every tick (smooth).
      "UpdateIntervalTicks": 1,
      // Show a chat message when toggling
      "NotifyOnToggle": true,
      // Turn off the flashlight when the player dies
      "TurnOffOnDeath": true,
      // Turn off the flashlight when the player switches teams
      "TurnOffOnTeamSwitch": true,
      // Disable flashlight for spectators
      "DisableInSpectator": true
    },

    "Light": {
      // Light color (hex RGB)
      "Color": "#FFFFFF",
      // Color temperature in Kelvin (1000-12000); 6500K = daylight white
      "ColorTemperature": 6500.0,
      // Light brightness (float, engine clamps internally)
      "Brightness": 1.0,
      // Maximum range in game units (1 unit ≈ 1 cm; 2048 ≈ 20m)
      "Range": 2048.0,
      // Cast shadows (more realistic but more expensive)
      "CastShadows": true,
      // Beam texture path for the light cookie projection
      "LightCookie": "materials/effects/lightcookies/flashlight.vtex",
      // Beam cross-section size (X/Y) and depth (Z)
      "SizeX": 45.0,
      "SizeY": 45.0,
      "SizeZ": 0.03,
      // Beam edge softness
      "SoftX": 1.0,
      "SoftY": 1.0,
      // Beam shape skirt factors
      "Skirt": 0.5,
      "SkirtNear": 1.0,
      // Distance in front of the eye to place the light origin (units)
      "ForwardDistance": 54.0,
      // Eye height offsets (units) - standing / crouching
      "StandEyeOffsetZ": 64.0,
      "CrouchEyeOffsetZ": 46.0,
      // Only the owner can see the light
      "OwnerOnly": true
    }
  }
}
```

## Building

```bash
dotnet build
dotnet publish -c Release
```

The zip is generated in `build/sws2-flashlight.zip`.

## Versioning

The release workflow bumps the patch version automatically:

- Every push to `main` (with source changes) computes the next version from the latest `vX.Y.Z` tag (patch +1).
- `WORKFLOW_VERSION` placeholder in `src/Metadata.cs` is replaced at release build time.

## License

[MIT](LICENSE)

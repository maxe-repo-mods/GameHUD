# GameHUD

Display level number and stage timer on screen during gameplay in R.E.P.O.

## Features

- Level number display with current level count
- Stage timer showing elapsed time (MM:SS or HH:MM:SS format)
- Configurable position, font size, and visibility for both elements
- Toggle HUD visibility with Tab key or set to always show
- Display final time after stage completion
- Optional hour display in timer
- Independent enable/disable per HUD element
- Real-time configuration without restart

## Installation

1. Install [BepInEx 5.x](https://github.com/BepInEx/BepInEx/releases) for R.E.P.O.
2. Download the latest `GameHUD.dll` from releases
3. Place `GameHUD.dll` in `BepInEx/plugins/`
4. Launch the game to generate config file

## Configuration

Configuration file: `BepInEx/config/maxenterme.GameHUD.cfg`

| Section | Key | Type | Default | Description |
|---------|-----|------|---------|-------------|
| Level Display | Enabled | bool | true | Enable the level number display. |
| Level Display | PositionX | int | -10 | HUD X offset from top-right corner. Range: -1000 to 1000 |
| Level Display | PositionY | int | -10 | HUD Y offset from top-right corner. Range: -1000 to 1000 |
| Level Display | FontSize | int | 22 | Font size. Range: 10-60 |
| Level Display | AlwaysShow | bool | false | Always show level display. If false, only shown while holding Tab. |
| Stage Timer | Enabled | bool | true | Enable the stage timer display. |
| Stage Timer | PositionX | int | -10 | HUD X offset from top-right corner. Range: -1000 to 1000 |
| Stage Timer | PositionY | int | -60 | HUD Y offset from top-right corner. Range: -1000 to 1000 |
| Stage Timer | FontSize | int | 20 | Font size. Range: 10-60 |
| Stage Timer | AlwaysShow | bool | false | Always show timer. If false, only shown while holding Tab. |
| Stage Timer | ShowHours | bool | false | Use HH:MM:SS format instead of MM:SS. |
| Stage Timer | ShowFinalTime | bool | true | Keep displaying the final time after stage clear. |

## Build

```bash
dotnet build -c Release
```

Output: `bin/Release/netstandard2.1/GameHUD.dll`


## AI Disclosure

This mod was developed with the assistance of AI (Claude by Anthropic). All code has been reviewed and tested by the developer.

## License

MIT

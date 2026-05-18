using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;

namespace GameHUD;

[BepInPlugin(PluginGuid, PluginName, PluginVersion)]
public class Plugin : BaseUnityPlugin
{
    private const string PluginGuid = "maxenterme.GameHUD";
    private const string PluginName = "GameHUD";
    private const string PluginVersion = "1.0.0";

    internal static Plugin Instance { get; private set; } = null!;
    internal new static ManualLogSource Logger => Instance._logger;
    private ManualLogSource _logger => base.Logger;

    // Level Display
    internal static ConfigEntry<bool> LevelEnabled = null!;
    internal static ConfigEntry<int> LevelPositionX = null!;
    internal static ConfigEntry<int> LevelPositionY = null!;
    internal static ConfigEntry<int> LevelFontSize = null!;
    internal static ConfigEntry<bool> LevelAlwaysShow = null!;

    // Stage Timer
    internal static ConfigEntry<bool> TimerEnabled = null!;
    internal static ConfigEntry<int> TimerPositionX = null!;
    internal static ConfigEntry<int> TimerPositionY = null!;
    internal static ConfigEntry<int> TimerFontSize = null!;
    internal static ConfigEntry<bool> TimerAlwaysShow = null!;
    internal static ConfigEntry<bool> TimerShowHours = null!;
    internal static ConfigEntry<bool> TimerShowFinalTime = null!;

    private void Awake()
    {
        Instance = this;

        // Level Display config
        LevelEnabled = Config.Bind(
            "Level Display", "Enabled", true,
            "Enable the level number display."
        );
        LevelPositionX = Config.Bind(
            "Level Display", "PositionX", 285,
            new ConfigDescription("HUD X offset", new AcceptableValueRange<int>(-1000, 1000))
        );
        LevelPositionY = Config.Bind(
            "Level Display", "PositionY", 60,
            new ConfigDescription("HUD Y offset", new AcceptableValueRange<int>(-1000, 1000))
        );
        LevelFontSize = Config.Bind(
            "Level Display", "FontSize", 22,
            new ConfigDescription("Font size", new AcceptableValueRange<int>(10, 60))
        );
        LevelAlwaysShow = Config.Bind(
            "Level Display", "AlwaysShow", false,
            "Always show level display. If false, only shown while holding Tab."
        );

        // Stage Timer config
        TimerEnabled = Config.Bind(
            "Stage Timer", "Enabled", true,
            "Enable the stage timer display."
        );
        TimerPositionX = Config.Bind(
            "Stage Timer", "PositionX", 285,
            new ConfigDescription("HUD X offset", new AcceptableValueRange<int>(-1000, 1000))
        );
        TimerPositionY = Config.Bind(
            "Stage Timer", "PositionY", 40,
            new ConfigDescription("HUD Y offset", new AcceptableValueRange<int>(-1000, 1000))
        );
        TimerFontSize = Config.Bind(
            "Stage Timer", "FontSize", 20,
            new ConfigDescription("Font size", new AcceptableValueRange<int>(10, 60))
        );
        TimerAlwaysShow = Config.Bind(
            "Stage Timer", "AlwaysShow", false,
            "Always show timer. If false, only shown while holding Tab."
        );
        TimerShowHours = Config.Bind(
            "Stage Timer", "ShowHours", false,
            "Use HH:MM:SS format instead of MM:SS."
        );
        TimerShowFinalTime = Config.Bind(
            "Stage Timer", "ShowFinalTime", true,
            "Keep displaying the final time after stage clear."
        );

        new Harmony(PluginGuid).PatchAll(typeof(Plugin).Assembly);
        Logger.LogInfo($"{PluginName} v{PluginVersion} loaded!");
    }
}

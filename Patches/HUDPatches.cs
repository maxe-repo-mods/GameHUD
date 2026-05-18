using System;
using HarmonyLib;
using TMPro;
using UnityEngine;

namespace GameHUD.Patches;

[HarmonyPatch]
public static class HUDPatches
{
    // Shared
    private static TMP_FontAsset? _cachedFont;
    private static Transform? _gameHudTransform;

    // Level Display
    private static GameObject? _levelObject;
    private static TextMeshProUGUI? _levelText;
    private static float _levelLastFontSize;
    private static float _levelLastPosX;
    private static float _levelLastPosY;

    // Stage Timer
    private static GameObject? _timerObject;
    private static TextMeshProUGUI? _timerText;
    private static float _timerLastFontSize;
    private static float _timerLastPosX;
    private static float _timerLastPosY;
    private static float _startTime;
    private static float _elapsedTime;
    private static bool _running;
    private static bool _stageCleared;

    [HarmonyPatch(typeof(LevelGenerator), nameof(LevelGenerator.GenerateDone))]
    [HarmonyPostfix]
    private static void LevelGenerator_GenerateDone_Postfix()
    {
        _startTime = Time.time;
        _elapsedTime = 0f;
        _running = true;
        _stageCleared = false;
    }

    [HarmonyPatch(typeof(RoundDirector), nameof(RoundDirector.ExtractionCompletedAllRPC))]
    [HarmonyPostfix]
    private static void ExtractionCompleted_Postfix()
    {
        if (_running)
        {
            _elapsedTime = Time.time - _startTime;
            _running = false;
            _stageCleared = true;
        }
    }

    [HarmonyPatch(typeof(RoundDirector), "Update")]
    [HarmonyPostfix]
    private static void RoundDirector_Update_Postfix()
    {
        try
        {
            if (!SemiFunc.RunIsLevel()) return;

            if (Plugin.LevelEnabled.Value)
                UpdateLevelDisplay();

            if (Plugin.TimerEnabled.Value)
                UpdateTimerDisplay();
        }
        catch (Exception e)
        {
            Plugin.Logger.LogError($"GameHUD Update error: {e}");
        }
    }

    [HarmonyPatch(typeof(SemiFunc), nameof(SemiFunc.OnSceneSwitch))]
    [HarmonyPrefix]
    private static void OnSceneSwitch_Prefix()
    {
        if (_levelObject != null)
        {
            UnityEngine.Object.Destroy(_levelObject);
            _levelObject = null;
            _levelText = null;
        }
        if (_timerObject != null)
        {
            UnityEngine.Object.Destroy(_timerObject);
            _timerObject = null;
            _timerText = null;
        }
        _running = false;
        _stageCleared = false;
        _cachedFont = null;
        _gameHudTransform = null;
    }

    private static void UpdateLevelDisplay()
    {
        if (_levelObject == null)
        {
            _levelText = null;
            CreateHUDElement(ref _levelObject, ref _levelText, "LevelDisplay HUD",
                Plugin.LevelFontSize.Value, Plugin.LevelPositionX.Value, Plugin.LevelPositionY.Value,
                new Color(0.79f, 0.91f, 0.90f, 1f));
            if (_levelObject == null) return;
        }

        bool shouldShow = Plugin.LevelAlwaysShow.Value || SemiFunc.InputHold((InputKey)8);
        _levelObject.SetActive(shouldShow);

        if (shouldShow && _levelText != null)
        {
            int level = 0;
            if (StatsManager.instance?.runStats != null &&
                StatsManager.instance.runStats.ContainsKey("level"))
            {
                level = StatsManager.instance.runStats["level"] + 1;
            }
            _levelText.text = $"<b>Level {level}</b>";

            ApplyConfigChanges(ref _levelLastFontSize, ref _levelLastPosX, ref _levelLastPosY,
                _levelObject, _levelText, Plugin.LevelFontSize.Value, Plugin.LevelPositionX.Value, Plugin.LevelPositionY.Value);
        }
    }

    private static void UpdateTimerDisplay()
    {
        if (_timerObject == null)
        {
            _timerText = null;
            CreateHUDElement(ref _timerObject, ref _timerText, "StageTimer HUD",
                Plugin.TimerFontSize.Value, Plugin.TimerPositionX.Value, Plugin.TimerPositionY.Value,
                new Color(0.79f, 0.91f, 0.90f, 1f));
            if (_timerObject == null) return;
        }

        bool showAfterClear = _stageCleared && Plugin.TimerShowFinalTime.Value;
        bool shouldShow = Plugin.TimerAlwaysShow.Value || SemiFunc.InputHold((InputKey)8) || showAfterClear;
        _timerObject.SetActive(shouldShow);

        if (shouldShow && _timerText != null)
        {
            float elapsed = _running ? Time.time - _startTime : _elapsedTime;
            var ts = TimeSpan.FromSeconds(elapsed);
            string timeStr = Plugin.TimerShowHours.Value
                ? $"{(int)ts.TotalHours:D2}:{ts.Minutes:D2}:{ts.Seconds:D2}"
                : $"{(int)ts.TotalMinutes:D2}:{ts.Seconds:D2}";
            string color = _stageCleared ? "#55ff55" : "#ffffff";
            _timerText.text = $"<color={color}><b>{timeStr}</b></color>";

            ApplyConfigChanges(ref _timerLastFontSize, ref _timerLastPosX, ref _timerLastPosY,
                _timerObject, _timerText, Plugin.TimerFontSize.Value, Plugin.TimerPositionX.Value, Plugin.TimerPositionY.Value);
        }
    }

    private static void CreateHUDElement(ref GameObject? obj, ref TextMeshProUGUI? text,
        string name, float fontSize, float posX, float posY, Color color)
    {
        if (_gameHudTransform == null)
        {
            // Use HUD.instance (always available) instead of GameObject.Find
            if (HUD.instance == null || HUD.instance.hideParent == null) return;
            _gameHudTransform = HUD.instance.hideParent.transform;
        }

        if (_cachedFont == null)
        {
            // Find any TMP font in the scene instead of relying on a specific object name
            var allTexts = Resources.FindObjectsOfTypeAll<TMP_Text>();
            foreach (var t in allTexts)
            {
                if (t.font != null)
                {
                    _cachedFont = t.font;
                    break;
                }
            }
            if (_cachedFont == null) return;
        }

        obj = new GameObject(name);
        obj.SetActive(false);
        obj.transform.SetParent(_gameHudTransform, false);

        text = obj.AddComponent<TextMeshProUGUI>();
        text.font = _cachedFont;
        text.fontSize = fontSize;
        text.color = color;
        text.alignment = TextAlignmentOptions.TopRight;
        text.enableWordWrapping = false;
        text.raycastTarget = false;

        var rect = obj.GetComponent<RectTransform>();
        rect.pivot = new Vector2(1f, 1f);
        rect.anchorMin = new Vector2(1f, 1f);
        rect.anchorMax = new Vector2(1f, 1f);
        rect.sizeDelta = new Vector2(300f, 50f);
        rect.anchoredPosition = new Vector2(posX, posY);
    }

    private static void ApplyConfigChanges(ref float lastFontSize, ref float lastPosX, ref float lastPosY,
        GameObject obj, TextMeshProUGUI text, float fontSize, float posX, float posY)
    {
        if (fontSize != lastFontSize || posX != lastPosX || posY != lastPosY)
        {
            lastFontSize = fontSize;
            lastPosX = posX;
            lastPosY = posY;
            text.fontSize = fontSize;
            var rect = obj.GetComponent<RectTransform>();
            rect.anchoredPosition = new Vector2(posX, posY);
        }
    }
}

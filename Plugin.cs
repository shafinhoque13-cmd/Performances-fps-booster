using UnityEngine;
using BepInEx;

[BepInPlugin("com.ershad.fpsbooster", "Silksong 30FPS Stable Mod", "1.1.0")]
public class Plugin : BaseUnityPlugin
{
    void Awake()
    {
        // Force the frame rate to at least 30 (Unity will try to match this)
        Application.targetFrameRate = 30;
        
        // Disable VSync to prevent the game from locking to 15 or 20 if it misses a frame
        QualitySettings.vSyncCount = 0;

        // --- Performance "Nukes" to stay above 30 ---
        
        // Disable shadows (One of the biggest CPU/GPU killers in Unity)
        QualitySettings.shadows = ShadowQuality.Disable;
        
        // Lower texture quality to free up RAM (0 = Full, 1 = Half, 2 = Quarter)
        QualitySettings.masterTextureLimit = 1;

        // Reduce draw distance/detail slightly for more stability
        QualitySettings.lodBias = 0.5f; 
        
        // Log to confirm the mod is active
        Logger.LogInfo("Minimum 30 FPS Target Set & Graphics Optimized!");
    }

    void Update()
    {
        // Emergency Check: If frame rate is being overridden by the game, force it back
        if (Application.targetFrameRate != 30)
        {
            Application.targetFrameRate = 30;
        }
    }
}


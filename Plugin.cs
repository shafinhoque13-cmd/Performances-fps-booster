using UnityEngine;
using BepInEx;

[BepInPlugin("com.ershad.fpsbooster", "Silksong Lightweight Mod", "1.3.0")]
public class Plugin : BaseUnityPlugin
{
    private Rect windowRect = new Rect(20, 20, 280, 160);
    private bool showGUI = true;
    private float deltaTime = 0.0f;

    void Awake()
    {
        // 1. Set Max FPS to 60 for smoothness
        Application.targetFrameRate = 60;
        QualitySettings.vSyncCount = 0;

        // 2. Aggressive Graphics Reduction
        QualitySettings.shadows = ShadowQuality.Disable;
        QualitySettings.masterTextureLimit = 1; // 1 = Half-res (Save RAM)
        QualitySettings.antiAliasing = 0;
        QualitySettings.softParticles = false; // Makes particles "lighter"
        
        Logger.LogInfo("Lightweight Mod Active: 60 FPS Cap + Particle Reduction.");
    }

    void Update()
    {
        deltaTime += (Time.unscaledDeltaTime - deltaTime) * 0.1f;

        // Maintain the 60 FPS target if the game tries to reset it
        if (Application.targetFrameRate != 60) Application.targetFrameRate = 60;

        // Toggle UI with 3-finger tap
        if (Input.touchCount == 3 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            showGUI = !showGUI;
        }

        // 3. Particle Removal Logic (Runs every 2 seconds to save CPU)
        if (Time.frameCount % 120 == 0)
        {
            RemoveExtraParticles();
        }
    }

    void RemoveExtraParticles()
    {
        // Find all particle systems in the current scene
        ParticleSystem[] particles = FindObjectsOfType<ParticleSystem>();
        foreach (var p in particles)
        {
            var main = p.main;
            // Reduce max particles by 70% to make the game feel "lighter"
            main.maxParticles = Mathf.Max(5, (int)(main.maxParticles * 0.3f));
        }
    }

    void OnGUI()
    {
        if (!showGUI) return;
        windowRect = GUI.Window(0, windowRect, DrawBubble, "Silksong Lite Status");
    }

    void DrawBubble(int windowID)
    {
        float fps = 1.0f / deltaTime;
        float perfPercent = (fps / 60f) * 100f; // Percentage relative to 60 FPS
        if (perfPercent > 100f) perfPercent = 100f;

        GUI.Label(new Rect(10, 25, 260, 30), $"Current FPS: {fps:0.} / 60");
        GUI.Label(new Rect(10, 55, 260, 30), $"Efficiency: {perfPercent:0.}%");
        
        GUI.color = (perfPercent > 75) ? Color.green : (perfPercent > 45 ? Color.yellow : Color.red);
        GUI.Label(new Rect(10, 85, 260, 30), $"Particles: REDUCED (70%)");
        GUI.Label(new Rect(10, 115, 260, 30), "STATUS: LIGHTWEIGHT MODE");

        GUI.DragWindow(new Rect(0, 0, 10000, 10000));
    }
}

using UnityEngine;
using BepInEx;

[BepInPlugin("com.ershad.fpsbooster", "Silksong Lightweight Mod", "1.4.0")]
public class Plugin : BaseUnityPlugin
{
    // Window and Button Positions
    private Rect windowRect = new Rect(20, 20, 280, 180);
    private Rect buttonRect = new Rect(Screen.width - 120, 100, 80, 80); // Floating toggle button
    
    private bool showMenu = false;
    private float deltaTime = 0.0f;

    void Awake()
    {
        Application.targetFrameRate = 60;
        QualitySettings.vSyncCount = 0;
        QualitySettings.shadows = ShadowQuality.Disable;
        QualitySettings.masterTextureLimit = 1;
        QualitySettings.softParticles = false;
        
        Logger.LogInfo("Mod Loaded: Floating Toggle Button Active.");
    }

    void Update()
    {
        deltaTime += (Time.unscaledDeltaTime - deltaTime) * 0.1f;
        
        if (Application.targetFrameRate != 60) Application.targetFrameRate = 60;

        // Optimized Particle Cleanup (Runs every 3 seconds to keep FPS high)
        if (Time.frameCount % 180 == 0)
        {
            RemoveExtraParticles();
        }
    }

    void RemoveExtraParticles()
    {
        ParticleSystem[] particles = FindObjectsOfType<ParticleSystem>();
        foreach (var p in particles)
        {
            var main = p.main;
            main.maxParticles = Mathf.Max(5, (int)(main.maxParticles * 0.3f));
        }
    }

    void OnGUI()
    {
        // 1. DRAW THE FLOATING TOGGLE BUTTON
        // This button is always visible and used to open/close the main menu
        buttonRect = GUI.Window(1, buttonRect, DrawToggleButton, "");

        // 2. DRAW THE MAIN MENU (Only if showMenu is true)
        if (showMenu)
        {
            windowRect = GUI.Window(0, windowRect, DrawMainMenu, "Silksong Lite Status");
        }
    }

    // The small draggable button logic
    void DrawToggleButton(int windowID)
    {
        if (GUI.Button(new Rect(0, 0, 80, 80), "MOD"))
        {
            showMenu = !showMenu;
        }
        GUI.DragWindow(new Rect(0, 0, 10000, 10000)); // Make the button draggable
    }

    // The main status window logic
    void DrawMainMenu(int windowID)
    {
        float fps = 1.0f / deltaTime;
        float perfPercent = (fps / 60f) * 100f;
        if (perfPercent > 100f) perfPercent = 100f;

        GUI.Label(new Rect(10, 25, 260, 30), $"FPS: {fps:0.} / 60");
        GUI.Label(new Rect(10, 55, 260, 30), $"Efficiency: {perfPercent:0.}%");
        
        GUI.color = (perfPercent > 75) ? Color.green : (perfPercent > 45 ? Color.yellow : Color.red);
        GUI.Label(new Rect(10, 85, 260, 30), $"Particles: REDUCED");
        
        GUI.color = Color.white;
        if (GUI.Button(new Rect(10, 120, 260, 40), "CLOSE MENU"))
        {
            showMenu = false;
        }

        GUI.DragWindow(new Rect(0, 0, 10000, 10000)); // Make the menu draggable
    }
}

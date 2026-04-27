using UnityEngine;
using BepInEx;

[BepInPlugin("com.ershad.fpsbooster", "Silksong Lightweight 30FPS", "1.7.0")]
public class Plugin : BaseUnityPlugin
{
    private Rect windowRect = new Rect(50, 50, 300, 220);
    private Rect buttonRect = new Rect(10, 100, 120, 120);
    private bool showMenu = false;
    private float deltaTime = 0.0f;

    void Awake()
    {
        // 1. Force 30 FPS for maximum stability
        Application.targetFrameRate = 30;
        QualitySettings.vSyncCount = 0;

        // 2. Nuke Graphics (This makes the game "Lighter")
        QualitySettings.shadows = ShadowQuality.Disable;
        QualitySettings.softParticles = false; // Disables expensive particle blending
        QualitySettings.particleRaycastBudget = 0; // Particles won't hit walls (Saves CPU)
        QualitySettings.masterTextureLimit = 1; // Half-res textures to save RAM
        
        Logger.LogInfo("Mod Loaded: Forced 30 FPS and Particle Nuker Active.");
    }

    void Update()
    {
        deltaTime += (Time.unscaledDeltaTime - deltaTime) * 0.1f;

        // Force 30 FPS every frame in case the game tries to change it
        if (Application.targetFrameRate != 30) 
            Application.targetFrameRate = 30;

        // 3. Constant Particle Cleanup
        // This stops particles from accumulating during combat
        if (Time.frameCount % 60 == 0) // Check every 1 second at 60Hz/2s at 30Hz
        {
            foreach (var p in FindObjectsOfType<ParticleSystem>())
            {
                var main = p.main;
                main.maxParticles = 5; // Allow almost no particles
                p.Stop(); // Kill existing ones immediately
            }
        }

        HandleTouch();
    }

    void HandleTouch()
    {
        if (Input.touchCount > 0)
        {
            Touch t = Input.GetTouch(0);
            Vector2 touchPos = new Vector2(t.position.x, Screen.height - t.position.y);

            if (t.phase == TouchPhase.Moved)
            {
                if (buttonRect.Contains(touchPos) && !showMenu) buttonRect.center = touchPos;
                else if (showMenu && windowRect.Contains(touchPos)) windowRect.center = touchPos;
            }
        }
    }

    void OnGUI()
    {
        buttonRect = GUI.Window(1, buttonRect, (id) => {
            if (GUI.Button(new Rect(5, 5, 110, 110), "MOD")) showMenu = !showMenu;
            GUI.DragWindow(); 
        }, "");

        if (showMenu)
        {
            windowRect = GUI.Window(0, windowRect, (id) => {
                float fps = 1.0f / deltaTime;
                GUI.Label(new Rect(10, 30, 280, 40), $"FPS: {fps:0.} / 30");
                GUI.Label(new Rect(10, 70, 280, 40), "Particles: DISABLED");
                GUI.Label(new Rect(10, 100, 280, 40), "Textures: OPTIMIZED");
                if (GUI.Button(new Rect(10, 150, 280, 50), "HIDE")) showMenu = false;
                GUI.DragWindow();
            }, "Silksong Performance");
        }
    }
}

using UnityEngine;
using BepInEx;

[BepInPlugin("com.ershad.fpsbooster", "Silksong Lightweight 30FPS", "1.7.1")]
public class Plugin : BaseUnityPlugin
{
    private Rect windowRect = new Rect(50, 50, 300, 220);
    private Rect buttonRect = new Rect(10, 100, 120, 120);
    private bool showMenu = false;
    private float deltaTime = 0.0f;

    void Awake()
    {
        Application.targetFrameRate = 30;
        QualitySettings.vSyncCount = 0;
        QualitySettings.shadows = ShadowQuality.Disable;
        QualitySettings.softParticles = false;
        QualitySettings.particleRaycastBudget = 0;
        
        // Updated to use the newer name to stop warnings
        QualitySettings.globalTextureMipmapLimit = 1; 
        
        Logger.LogInfo("Mod Loaded: Optimized and Particle Nuker Active.");
    }

    void Update()
    {
        deltaTime += (Time.unscaledDeltaTime - deltaTime) * 0.1f;

        if (Application.targetFrameRate != 30) Application.targetFrameRate = 30;

        if (Time.frameCount % 60 == 0)
        {
            // Updated to the newer version of FindObjects
            foreach (var p in Object.FindObjectsByType<ParticleSystem>(FindObjectsSortMode.None))
            {
                var main = p.main;
                main.maxParticles = 5;
                p.Stop();
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
                GUI.Label(new Rect(10, 70, 280, 40), "Particles: NUKED");
                if (GUI.Button(new Rect(10, 150, 280, 50), "HIDE")) showMenu = false;
                GUI.DragWindow();
            }, "Silksong Performance");
        }
    }
}

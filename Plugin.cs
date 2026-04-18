using UnityEngine;
using BepInEx;

[BepInPlugin("com.ershad.fpsbooster", "Silksong Performance Bubble", "1.2.0")]
public class Plugin : BaseUnityPlugin
{
    private Rect windowRect = new Rect(20, 20, 250, 150); // Position and size of the bubble
    private bool showGUI = true;
    private float deltaTime = 0.0f;

    void Awake()
    {
        Application.targetFrameRate = 30;
        QualitySettings.vSyncCount = 0;
        QualitySettings.shadows = ShadowQuality.Disable;
        QualitySettings.masterTextureLimit = 1;
        
        Logger.LogInfo("Mod and Bubble UI Loaded!");
    }

    void Update()
    {
        // Calculate smooth FPS for the display
        deltaTime += (Time.unscaledDeltaTime - deltaTime) * 0.1f;
        
        // Ensure 30 FPS lock stays active
        if (Application.targetFrameRate != 30) Application.targetFrameRate = 30;

        // Toggle visibility with a three-finger tap
        if (Input.touchCount == 3 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            showGUI = !showGUI;
        }
    }

    void OnGUI()
    {
        if (!showGUI) return;

        // Create a draggable window (ID 0)
        windowRect = GUI.Window(0, windowRect, DrawBubble, "Mod Performance Status");
    }

    void DrawBubble(int windowID)
    {
        float fps = 1.0f / deltaTime;
        
        // Calculate Performance Percentage
        // (Current FPS / Target 30) * 100. 100% means you are hitting your 30fps goal perfectly.
        float perfPercent = (fps / 30f) * 100f;
        if (perfPercent > 100f) perfPercent = 100f;

        GUI.Label(new Rect(10, 25, 230, 30), $"Current FPS: {fps:0.}");
        GUI.Label(new Rect(10, 55, 230, 30), $"Mod Working: {perfPercent:0.}%");
        
        // Color indicator
        if (perfPercent > 80) GUI.color = Color.green;
        else if (perfPercent > 50) GUI.color = Color.yellow;
        else GUI.color = Color.red;

        GUI.Label(new Rect(10, 85, 230, 30), perfPercent >= 90 ? "STATUS: STABLE" : "STATUS: BOOSTING...");

        // Make the whole window draggable
        GUI.DragWindow(new Rect(0, 0, 10000, 10000));
    }
}

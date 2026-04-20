using UnityEngine;
using BepInEx;

[BepInPlugin("com.ershad.fpsbooster", "Silksong Lightweight Mod", "1.6.0")]
public class Plugin : BaseUnityPlugin
{
    private Rect windowRect = new Rect(50, 50, 300, 200);
    private Rect buttonRect = new Rect(10, 100, 120, 120); // Bigger for easy touch
    private bool showMenu = false;
    private float deltaTime = 0.0f;

    void Update()
    {
        deltaTime += (Time.unscaledDeltaTime - deltaTime) * 0.1f;

        if (Input.touchCount > 0)
        {
            Touch t = Input.GetTouch(0);
            // Flip Y coordinate because GUI and Input use different origins
            Vector2 touchPos = new Vector2(t.position.x, Screen.height - t.position.y);

            if (t.phase == TouchPhase.Moved || t.phase == TouchPhase.Stationary)
            {
                // Drag the small "MOD" button
                if (buttonRect.Contains(touchPos) && !showMenu)
                {
                    buttonRect.center = touchPos;
                }
                // Drag the status menu
                else if (showMenu && windowRect.Contains(touchPos))
                {
                    windowRect.center = touchPos;
                }
            }
        }
    }

    void OnGUI()
    {
        // 1. Always show the draggable toggle button
        buttonRect = GUI.Window(1, buttonRect, (id) => {
            if (GUI.Button(new Rect(5, 5, 110, 110), "MOD")) showMenu = !showMenu;
            GUI.DragWindow(); 
        }, "");

        // 2. Show the main status menu
        if (showMenu)
        {
            windowRect = GUI.Window(0, windowRect, (id) => {
                float fps = 1.0f / deltaTime;
                GUI.Label(new Rect(10, 30, 280, 40), $"FPS: {fps:0.} / 60");
                GUI.Label(new Rect(10, 70, 280, 40), "Status: BOOSTING");
                if (GUI.Button(new Rect(10, 130, 280, 50), "HIDE")) showMenu = false;
                GUI.DragWindow();
            }, "Silksong Booster");
        }
    }
}

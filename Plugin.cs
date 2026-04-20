using BepInEx;
using UnityEngine;

namespace MobileBridge
{
    [BepInPlugin("com.shafin.bridge", "MobileBridge", "1.4.3")]
    public class Plugin : BaseUnityPlugin
    {
        private bool _active = true;
        private Rect _winRect = new Rect(100, 100, 250, 120);
        private bool _isDragging = false;
        private Vector2 _dragOffset;

        void OnGUI()
        {
            GUI.depth = -1000;
            float s = Screen.height / 1080f;
            GUI.matrix = Matrix4x4.TRS(Vector3.zero, Quaternion.identity, new Vector3(s, s, 1));

            // Manual Touch Handling for Moving the Bubble
            HandleDrag(s);

            _winRect = GUI.Window(0, _winRect, DrawWindow, "Bridge (Drag Me)");
        }

        void HandleDrag(float scale)
        {
            Vector2 mousePos = new Vector2(Input.mousePosition.x / scale, (Screen.height - Input.mousePosition.y) / scale);

            if (Input.GetMouseButtonDown(0) && _winRect.Contains(mousePos))
            {
                _isDragging = true;
                _dragOffset = mousePos - new Vector2(_winRect.x, _winRect.y);
            }

            if (_isDragging)
            {
                _winRect.x = mousePos.x - _dragOffset.x;
                _winRect.y = mousePos.y - _dragOffset.y;
            }

            if (Input.GetMouseButtonUp(0)) _isDragging = false;
        }

        void DrawWindow(int id)
        {
            string statusColor = _active ? "cyan" : "red";
            if (GUILayout.Button($"<color={statusColor}>BRIDGE: {(_active ? "ON" : "OFF")}</color>", GUILayout.ExpandHeight(true)))
                _active = !_active;
        }

        void Update()
        {
            if (!_active) return;

            // Bench Bypass Logic
            GameObject pd = GameObject.Find("PlayerData");
            if (pd != null)
            {
                pd.SendMessage("SetBool", new object[] { "atBench", true }, SendMessageOptions.DontRequireReceiver);
                pd.SendMessage("SetBool", new object[] { "canEquip", true }, SendMessageOptions.DontRequireReceiver);
            }

            // UI Interaction Unlock
            GameObject[] allObjects = Object.FindObjectsByType<GameObject>(FindObjectsSortMode.None);
            foreach (GameObject obj in allObjects)
            {
                if (obj.name.ToLower().Contains("equip") || obj.name.ToLower().Contains("slot"))
                {
                    obj.SendMessage("set_interactable", true, SendMessageOptions.DontRequireReceiver);
                }
            }
        }
    }
}

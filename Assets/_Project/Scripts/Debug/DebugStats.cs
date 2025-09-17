#if UNITY_EDITOR || DEVELOPMENT_BUILD
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class DebugStats : MonoBehaviour
{
    [SerializeField] private KeyCode toggleKey = KeyCode.F4;
    private bool showStats = false;
    private float deltaTime = 0.0f;

    void Update()
    {
        if (Input.GetKeyDown(toggleKey))
            showStats = !showStats;

        deltaTime += (Time.deltaTime - deltaTime) * 0.1f;
    }

    void OnGUI()
    {
        if (!showStats) return;

        float fps = 1.0f / deltaTime;
        string text = $"FPS: {fps:0.}";

#if UNITY_EDITOR
        // Drawcall sayısı sadece editorde erişilebilir
        int drawCalls = UnityStats.drawCalls;
        text += $" | DrawCalls: {drawCalls}";
#endif

        GUI.Label(new Rect(10, 10, 400, 30), text);
    }
}
#endif

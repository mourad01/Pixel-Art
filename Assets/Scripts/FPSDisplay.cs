using UnityEngine;

public class FPSDisplay : MonoBehaviour
{
    float deltaTime;

    void Update()
    {
        if (VoxConstants.isTest)
        {
            deltaTime += (Time.unscaledDeltaTime - deltaTime) * 0.1f;
        }
    }

    void OnGUI()
    {
        if (VoxConstants.isTest)
        {
            int width = Screen.width;
            int height = Screen.height;
            var gUIStyle = new GUIStyle();
            Rect position = new Rect(0f, 0f, (float)width, (float)(height * 2 / 100));
            gUIStyle.alignment = TextAnchor.UpperLeft;
            gUIStyle.fontSize = height * 2 / 100;
            gUIStyle.normal.textColor = new Color(0f, 0f, 0.5f, 1f);
            float msec = deltaTime * 1000f;
            float fps = 1f / deltaTime;
            string text = string.Format("{0:0.0} ms ({1:0.} fps)", msec, fps);
            GUI.Label(position, text, gUIStyle);
        }
    }
}



using UnityEngine;

public class OrientatableCamera : MonoBehaviour
{
    [SerializeField] Canvas m_canvas;
    [SerializeField] float m_uiSize;
    [SerializeField] float m_topUiSize;

    Camera m_mainCamera;

    public Rect CameraRect { get; private set; }

    void Awake()
    {
        m_mainCamera = base.GetComponent<Camera>();
        OnOrientationChangedHandler(ScreenOrientation.Portrait);
    }

    void OnOrientationChangedHandler(ScreenOrientation orientation)
    {
        Rect rect = (m_canvas.transform as RectTransform).rect;
        float maxSide = Mathf.Max(rect.width, rect.height);
        switch (orientation)
        {
            case ScreenOrientation.LandscapeLeft:
            case ScreenOrientation.LandscapeRight:
                {
                    float uiWidth = m_uiSize / maxSide + 0.005f;
                    uiWidth = Mathf.Max(0.01f, uiWidth);
                    CameraRect = new Rect(0f, 0f, 1f - uiWidth, 1f);
                    break;
                }
            case ScreenOrientation.Portrait:
            case ScreenOrientation.PortraitUpsideDown:
                {
                    float b = m_uiSize / maxSide - 0.005f;
                    b = Mathf.Max(0.01f, b);
                    float topPart = 0f;
                    topPart = m_topUiSize / maxSide;
                    CameraRect = new Rect(0f, b, 1f, 1f - b - topPart);
                    break;
                }
        }
    }
}



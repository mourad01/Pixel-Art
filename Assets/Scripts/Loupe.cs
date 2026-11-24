using UnityEngine;

public class Loupe : MonoBehaviour
{
    [SerializeField] Transform m_loupeCamera;
    [SerializeField] Canvas m_canvas;
    [SerializeField] Color m_color = Color.white;
    [SerializeField] RectTransform m_loupeRect;

    public float Precision { get;   set; }

    public Vector2 Size
    {
        get
        {
            return ((RectTransform)transform).sizeDelta;
        }
    }

    public void UpdatePosition(Vector3 screenPos, Vector3 worldPos)
    {
        Vector2 v = screenPos / m_canvas.scaleFactor;
        Vector2 sizeDelta = ((RectTransform)transform).sizeDelta;
        float height = ((RectTransform)m_canvas.transform).rect.height;
        float width = ((RectTransform)m_canvas.transform).rect.width;
        v.x -= ((RectTransform)m_canvas.transform).rect.width / 2f;
        v.y -= height / 2f;
        if (v.y + sizeDelta.y > height / 2f)
        {
            Vector2 size = Size;
            float dy = size.y / 2f - Mathf.Abs(height / 2f - (v.y + sizeDelta.y));
            Vector2 size2 = Size;
            float x = size2.x;
            Vector2 size3 = Size;
            float dx = Mathf.Sqrt(x * size3.x / 4f - dy * dy);
            v.x -= dx;
            v.y = height / 2f - sizeDelta.y / 2f;
            if (v.x - sizeDelta.x / 2f < (0f - width) / 2f)
            {
                v.x += dx * 2f;
            }
        }
        else
        {
            v.y += sizeDelta.y / 2f;
        }
        float x2 = v.x;
        Vector2 size4 = Size;
        if (x2 + size4.x / 4f > width / 2f)
        {
            float x3 = v.x;
            float spaceRight = width / 2f - v.x;
            Vector2 size5 = Size;
            v.x = x3 + (spaceRight - size5.x / 4f);
        }
        else
        {
            float x4 = v.x;
            Vector2 size6 = Size;
            if (x4 - size6.x / 4f < (0f - width) / 2f)
            {
                float x5 = v.x;
                float spaceLeft = (0f - width) / 2f - v.x;
                Vector2 size7 = Size;
                v.x = x5 + (spaceLeft + size7.x / 4f);
            }
        }
        transform.localPosition = v;
        worldPos.z = m_loupeCamera.position.z;
        m_loupeCamera.position = worldPos;
    }

    public void SwitchOnOff(bool value)
    {
        if (AppData.LoupeEnabled)
        {
            gameObject.SetActive(value);
            m_loupeCamera.gameObject.SetActive(value);
        }
    }

    public void SetLoupePrecision(Camera mainCamera, float koef)
    {
        float orthoSize = m_loupeRect.rect.height / (((RectTransform)m_canvas.transform).rect.height * mainCamera.rect.height) * mainCamera.orthographicSize;
        ((Component)m_loupeCamera).GetComponent<Camera>().orthographicSize = orthoSize * koef;
    }

    public void SetLoupePrecision(float camSize)
    {
        Precision = camSize;
        ((Component)m_loupeCamera).GetComponent<Camera>().orthographicSize = camSize;
    }
}



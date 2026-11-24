

using System.Collections;
using UnityEngine;

public class CameraManager3D : MonoBehaviour
{
    [SerializeField] RectTransform m_canvasRectTransform;
    [SerializeField] Camera m_camera;
    [SerializeField] RectTransform m_cameraSpace;

    public void Init()
    {
        base.StartCoroutine(DefferedUpdate());
    }

    IEnumerator DefferedUpdate()
    {
        float topOffset = 0f;
        var rectTransform = m_cameraSpace;
        while (true)
        {
            if (!(rectTransform != null))
            {
                break;
            }
            if (!(rectTransform.name != "Canvas"))
            {
                break;
            }
            float value2 = topOffset;
            Vector2 anchoredPosition = rectTransform.anchoredPosition;
            topOffset = value2 + anchoredPosition.y;
            rectTransform = (RectTransform)rectTransform.parent;
        }
        float value3 = 0f - topOffset;
        Vector2 sizeDelta = m_cameraSpace.sizeDelta;
        float y = 1f - (value3 + sizeDelta.y) / m_canvasRectTransform.rect.height;
        Vector2 sizeDelta2 = m_cameraSpace.sizeDelta;
        float height = sizeDelta2.y / m_canvasRectTransform.rect.height;
        m_camera.rect = new Rect(0f, y, 1f, height);
        yield break;
    }
}



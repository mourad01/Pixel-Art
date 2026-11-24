

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
            topOffset = topOffset + rectTransform.anchoredPosition.y;
            rectTransform = (RectTransform)rectTransform.parent;
        }
        float top = 0f - topOffset;
        float y = 1f - (top + m_cameraSpace.sizeDelta.y) / m_canvasRectTransform.rect.height;
        float height = m_cameraSpace.sizeDelta.y / m_canvasRectTransform.rect.height;
        m_camera.rect = new Rect(0f, y, 1f, height);
        yield break;
    }
}



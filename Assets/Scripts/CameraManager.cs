

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    [SerializeField] Rect m_bounds = default(Rect);
    [SerializeField] Rect m_zeroBounds = default(Rect);
    [SerializeField] float m_zoom = 1f;
    [SerializeField] float m_maxZoom = 1f;
    [SerializeField] RectTransform m_canvasRectTransform;
    [SerializeField] Camera m_camera;
    [SerializeField] Camera m_backCamera;
    [SerializeField] RectTransform m_cameraSpace;
    [SerializeField] Canvas m_canvas;
    [SerializeField] Loupe m_loupe;

    public float Zoom
    {
        get
        {
            return m_zoom;
        }
    }

    public float MaxZoom
    {
        get
        {
            return m_maxZoom;
        }
    }

    public float RecommendMaxZoom
    {
        get
        {
            return m_maxZoom + 0.5f;
        }
    }

    public Vector2 CameraPos
    {
        get
        {
            return m_camera.transform.position;
        }
    }

    public RectTransform WorkspaceRect
    {
        get
        {
            return m_cameraSpace;
        }
    }

    public Loupe Loupe
    {
        get
        {
            return m_loupe;
        }
    }

    public float DefaultOrtoSize { get; private set; }

    public Rect DefaultRect { get; private set; }

    public void Init(float width, float height)
    {
        float offset = 0.9f;

        float halfSize = Mathf.Max(width, height) / 2f;
        m_bounds = new Rect(-width / halfSize / 2f - offset, -height / halfSize / 2f - 2 * offset, width / halfSize + 2 * offset, height / halfSize + 2 * offset);

        m_zeroBounds = new Rect(-width / halfSize / 2f, -height / halfSize / 2f, width / halfSize, height / halfSize);
        Rect zeroBounds = m_zeroBounds;
        zeroBounds.center += new Vector2(0f, 0.2f);
        m_bounds.center = new Vector2(0f, 0.2f);
        if (!MySystemInfo.IsTablet)
        {
            m_loupe.SetLoupePrecision(1.92f / halfSize);
        }
        else
        {
            m_loupe.SetLoupePrecision(2f / halfSize);
        }
        InternalUpdate(Vector2.zero);
        base.StartCoroutine(DefferedUpdate());
    }

    void InternalUpdate(Vector2 cameraDeltaPos)
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
        float value4 = (sizeDelta2.y + 150f) / m_canvasRectTransform.rect.height;
        m_camera.rect = new Rect(0f, y, 1f, value4);
        DefaultRect = m_camera.rect;
        float a = value4 * (float)Screen.height / (m_camera.rect.width * (float)Screen.width);
        a = (DefaultOrtoSize = Mathf.Max(a, 1.2f));
        m_camera.orthographicSize = a / m_zoom;
        float value6 = MySystemInfo.IsTablet ? (m_loupe.Precision / (400f / (m_canvasRectTransform.rect.height * m_camera.rect.height))) : (m_loupe.Precision / (500f / (m_canvasRectTransform.rect.height * m_camera.rect.height)));
        m_maxZoom = (a / value6);
        Rect rect = default(Rect);
        rect.height = m_camera.orthographicSize * 2f;
        rect.width = (float)Screen.width * m_camera.rect.width * rect.height / ((float)Screen.height * m_camera.rect.height);
        rect.center = (Vector2)m_camera.transform.position + cameraDeltaPos;
        Rect zeroBounds = (!(m_zoom > 1f)) ? m_zeroBounds : m_bounds;
        if (rect.width > zeroBounds.width)
        {
            Vector2 center = zeroBounds.center;
            float x = center.x;
            Vector2 center2 = rect.center;
            rect.center = new Vector2(x, center2.y);
        }
        else
        {
            if (rect.xMin < zeroBounds.xMin)
            {
                rect.center += new Vector2(zeroBounds.xMin - rect.xMin, 0f);
            }
            if (rect.xMax > zeroBounds.xMax)
            {
                rect.center += new Vector2(zeroBounds.xMax - rect.xMax, 0f);
            }
        }
        if (rect.height > zeroBounds.height)
        {
            Vector2 center3 = rect.center;
            float x2 = center3.x;
            Vector2 center4 = zeroBounds.center;
            rect.center = new Vector2(x2, center4.y);
        }
        else
        {
            if (rect.yMin < zeroBounds.yMin)
            {
                rect.center += new Vector2(0f, zeroBounds.yMin - rect.yMin);
            }
            if (rect.yMax > zeroBounds.yMax)
            {
                rect.center += new Vector2(0f, zeroBounds.yMax - rect.yMax);
            }
        }
        var transform = m_camera.transform;
        Vector2 center5 = rect.center;
        float x3 = center5.x;
        Vector2 center6 = rect.center;
        transform.position = new Vector3(x3, center6.y, -10f);
    }

    public void Move(Vector2 deltaPos)
    {
        float d = m_camera.orthographicSize / m_camera.rect.height / (float)Screen.height * 2f;
        InternalUpdate(-deltaPos * d);
    }

    public bool CheckMinZoom(Vector3 mousePos)
    {
        return false;
    }

    public void ChangeZoom(float koef)
    {
        m_zoom *= koef;
        m_zoom = Mathf.Max(1f, m_zoom);
        m_zoom = Mathf.Min(m_zoom, m_maxZoom);
        InternalUpdate(Vector2.zero);
    }

    public void EndZoom()
    {
        if (m_zoom > RecommendMaxZoom)
        {
            base.StartCoroutine(ToRecommendMaxZoomCoroutine());
        }
    }

    public void SwitchLoupeOnOff(bool value)
    {
        m_loupe.SwitchOnOff(value);
    }

    public void UpdateLoupe(Vector2 screenPos)
    {
        var ray = m_camera.ScreenPointToRay(screenPos);
        Vector2 zero = Vector2.zero;
        if (GetWorldPos(ray, out zero))
        {
            m_loupe.UpdatePosition(screenPos, zero);
        }
    }

    bool GetWorldPos(Ray ray, out Vector2 worldPos)
    {
        var raycastHits = Physics.RaycastAll(ray);
        if (raycastHits != null && raycastHits.Length > 0)
        {
            worldPos = raycastHits[0].point;
            return true;
        }
        worldPos = Vector2.zero;
        return false;
    }

    public void Scroll(float delta)
    {
        m_zoom *= Mathf.Pow(1.1f, delta);
        m_zoom = Mathf.Max(1f, m_zoom);
        m_zoom = Mathf.Min(m_zoom, m_maxZoom);
        InternalUpdate(Vector2.zero);
    }

    public void EnableBackCamera()
    {
        m_backCamera.gameObject.SetActive(true);
    }

    IEnumerator DefferedUpdate()
    {
        yield return null;
        InternalUpdate(Vector2.zero);
    }


    IEnumerator MoveZoomCoroutine(Vector2 worldPos)
    {
        var startPos = m_camera.transform.position;
        var startZoom = 1;
        var time = 0.3f;
        var timer = time; 
        Vector3 worldPos3D = worldPos;
        worldPos3D.z = m_camera.transform.position.z;
        while (true)
        {
            var deltaTime = Time.deltaTime;
            timer -= deltaTime;
            if (timer > 0)
            {
                m_zoom += (m_maxZoom - (float)startZoom) * deltaTime / time;
                InternalUpdate(Vector2.zero);
                worldPos3D.x = worldPos.x - worldPos.x / m_zoom * (float)startZoom;
                worldPos3D.y = worldPos.y - worldPos.y / m_zoom * (float)startZoom;
                m_camera.transform.position = worldPos3D;
                yield return null;
            }
            else
            {
                break;
            }
        }
        worldPos3D.x = worldPos.x - worldPos.x / m_maxZoom * (float)startZoom;
        worldPos3D.y = worldPos.y - worldPos.y / m_maxZoom * (float)startZoom;
        m_camera.transform.position = worldPos3D;
        m_zoom = m_maxZoom;
        InternalUpdate(Vector2.zero);

        yield break;
    }

    IEnumerator ToRecommendMaxZoomCoroutine()
    {
        var step = (RecommendMaxZoom - m_zoom) / 3f;
        for(int i = 0; i < 3; i++)
        {
            m_zoom += step;
            InternalUpdate(Vector2.zero);
            yield return null;
        } 
    }
}



using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class IphoneXExtraPixels : MonoBehaviour
{
    Vector2 m_sizeDelta = Vector2.zero;

    [SerializeField] RectTransform m_canvas;

    LayoutElement m_layoutElement;
    float m_pixels;

    [SerializeField] bool m_bottom;

    public float Height { get;  set; }

    void Awake()
    {
        base.StartCoroutine(InitCoroutine());
        m_layoutElement = base.GetComponent<LayoutElement>();
    }

    void UpdateVisibility()
    {
        if (m_pixels == 0f)
        {
            (transform as RectTransform).sizeDelta = Vector2.zero;
            if (m_layoutElement != null)
            {
                var layoutElement = m_layoutElement;
                float height = 0f;
                m_layoutElement.preferredHeight = height;
                layoutElement.minHeight = height;
            }
        }
        else
        {
            (transform as RectTransform).sizeDelta = m_sizeDelta;
            if (m_layoutElement != null)
            {
                var layoutElement = m_layoutElement;
                float height = 0f;
                m_layoutElement.preferredHeight = height;
                layoutElement.minHeight = height;
            }
            if (m_layoutElement != null)
            {
                var layoutElement3 = m_layoutElement;
                float height = m_sizeDelta.y;
                m_layoutElement.preferredHeight = height;
                layoutElement3.minHeight = height;
            }
        }
    }

    IEnumerator InitCoroutine()
    {
        yield return null;

        m_pixels = ((!m_bottom) ? ScreenToolWrapper.IphoneXExtraPixels : ScreenToolWrapper.IphoneXExtraBottomPixels);
        Height = m_pixels / (float)Screen.height * m_canvas.rect.height * ScreenToolWrapper.Density;
        m_sizeDelta = new Vector2(0f, Height);
        UpdateVisibility();
    }
}

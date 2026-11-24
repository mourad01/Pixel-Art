

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PixelSize : MonoBehaviour
{
    [SerializeField] RectTransform m_canvas;
    [SerializeField] float m_pixels;

    void Awake()
    {
        base.StartCoroutine(InitCoroutine());
    }
    IEnumerator InitCoroutine()
    {
        yield return null;

        var size = m_pixels * ScreenToolWrapper.Density * 1.777778f / (m_canvas.rect.height / m_canvas.rect.width);
        var sizeDelta = new Vector2(size, size);
        (transform as RectTransform).sizeDelta = sizeDelta;
    }
}



using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HeightDependsOnWidth : MonoBehaviour
{
    enum DependType
    {
        WidthMinus20Div2,
        Equal
    }

    [SerializeField] DependType m_dependType = DependType.Equal;

    RectTransform m_rectTransform;
    float m_width = -1f;

    void Start()
    {
        m_rectTransform = (RectTransform)transform;
        UpdateHeight();
    }

    void Update()
    {
        base.StartCoroutine(UpdateHeightCoroutine());
        enabled = false;
    }

    void UpdateHeight()
    {
        m_width = m_rectTransform.sizeDelta.x;
        float height = 0f;
        switch (m_dependType)
        {
            case DependType.Equal:
                height = m_width;
                break;
            case DependType.WidthMinus20Div2:
                height = (m_width - 20f) / 2f;
                break;
        }
        Vector2 size = m_rectTransform.sizeDelta;
        size.y = height;
        m_rectTransform.sizeDelta = size;
        var component = base.GetComponent<LayoutElement>();
        if (component != null)
        {
            component.preferredHeight = height;
            component.minHeight = component.preferredHeight;
        }
    }
    IEnumerator UpdateHeightCoroutine()
    {
        yield return null;
        UpdateHeight();
    }
}

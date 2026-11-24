

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
        Vector2 sizeDelta = m_rectTransform.sizeDelta;
        m_width = sizeDelta.x;
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
        Vector2 sizeDelta2 = m_rectTransform.sizeDelta;
        sizeDelta2.y = height;
        m_rectTransform.sizeDelta = sizeDelta2;
        var component = base.GetComponent<LayoutElement>();
        if (component != null)
        {
            var layoutElement = component;
            component.preferredHeight = height; float value4 = layoutElement.minHeight = (component.preferredHeight);
        }
    }
    IEnumerator UpdateHeightCoroutine()
    {
        yield return null;
        UpdateHeight();
    }
}

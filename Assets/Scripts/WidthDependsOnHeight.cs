

using UnityEngine;
using UnityEngine.UI;

public class WidthDependsOnHeight : MonoBehaviour
{
    [SerializeField] string m_expression;

    RectTransform m_rectTransform;
    float m_height = -1f;

    void Start()
    {
        m_rectTransform = (RectTransform)transform;
        UpdateWidth();
    }

    void Update()
    {
        if (m_rectTransform.rect.height != m_height)
        {
            UpdateWidth();
            enabled = false;
        }
    }

    void UpdateWidth()
    {
        m_height = m_rectTransform.rect.height;
        float height = m_height;
        Vector2 sizeDelta = m_rectTransform.sizeDelta;
        sizeDelta.x = height;
        m_rectTransform.sizeDelta = sizeDelta;
        var layout = base.GetComponent<LayoutElement>();
        if (layout != null)
        {
            var layoutElement = layout;
            layout.preferredWidth = height; float value3 = layoutElement.minWidth = (layout.preferredWidth);
        }
    }
}



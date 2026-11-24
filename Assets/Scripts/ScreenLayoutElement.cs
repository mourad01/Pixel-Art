

using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(LayoutElement))]
public class ScreenLayoutElement : MonoBehaviour
{
    RectTransform m_canvas;

    [SerializeField] float m_extends;
    [SerializeField] bool m_onlyWidth;
    [SerializeField] bool m_heightDependsOnWidth;
    [SerializeField] float m_extraHeight;

    void Awake()
    {
        var layout = base.GetComponent<LayoutElement>();
        FindCanvas();
        if (m_canvas != null)
        {
            float minSide = Mathf.Min(m_canvas.rect.width, m_canvas.rect.height);
            layout.preferredWidth = minSide - m_extends * 2f;
            layout.minWidth = minSide - m_extends * 2f;
            if (!m_onlyWidth)
            {
                layout.preferredHeight = minSide - m_extends * 2f;
                layout.minHeight = minSide - m_extends * 2f;
                if (m_heightDependsOnWidth)
                {
                    var layoutElement = layout;
                    layout.minHeight = layout.minWidth + m_extraHeight; float value4 = layoutElement.preferredHeight = (layout.minHeight);
                }
            }
        }
    }

    void FindCanvas()
    {
        var transform = base.transform;
        while (true)
        {
            if (transform != null)
            {
                var canvas = ((Component)transform).GetComponent<Canvas>();
                if (!(canvas != null))
                {
                    transform = transform.parent;
                    continue;
                }
                break;
            }
            return;
        }
        m_canvas = (transform as RectTransform);
    }
}



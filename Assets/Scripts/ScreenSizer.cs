

using UnityEngine;

public class ScreenSizer : MonoBehaviour
{
    RectTransform m_canvas;

    [SerializeField] float m_extends;
    [SerializeField] bool m_onlyWidth;
    [SerializeField] bool m_heightDependsOnWidth;
    [SerializeField] float m_extraHeight;
    [SerializeField] float m_extraHeight3x4 = -1f;

    void Awake()
    {
        FindCanvas();
        if (m_canvas != null)
        {
            float minSide = Mathf.Min(m_canvas.rect.width, m_canvas.rect.height);
            float side = minSide;
            float y = ((RectTransform)transform).sizeDelta.y;
            if (!m_onlyWidth)
            {
                y = m_canvas.rect.height;
                if (m_heightDependsOnWidth)
                {
                    //aspect ratio under 1.55 uses the 3x4 extra height
                    bool is3x4 = false;
                    if (Screen.width > Screen.height)
                    {
                        if ((float)Screen.width / (float)Screen.height < 1.55f)
                        {
                            is3x4 = true;
                        }
                    }
                    else if ((float)Screen.height / (float)Screen.width < 1.55f)
                    {
                        is3x4 = true;
                    }
                    y = ((!is3x4 || m_extraHeight3x4 != -1f) ? (side + m_extraHeight) : (side + m_extraHeight3x4));
                }
            }
            (transform as RectTransform).sizeDelta = new Vector2(side - m_extends * 2f, y);
            var rectTransform = transform as RectTransform;
            Vector2 anchoredPosition = (transform as RectTransform).anchoredPosition;
            rectTransform.anchoredPosition = new Vector2(0f, anchoredPosition.y);
        }
    }

    void FindCanvas()
    {
        //walk up the parents until a Canvas is found
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



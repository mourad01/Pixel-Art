

using UnityEngine;
using UnityEngine.UI;

internal class ScreenFormatRectSize : MonoBehaviour
{
    int taktsCounter;

    [SerializeField] float m_forceWidth4_3 = 100f;
    [SerializeField] float m_forceHeight4_3 = 100f;
    [SerializeField] int m_takts;

    void Awake()
    {
    }

    void Update()
    {
        //wait m_takts frames, then apply the 4:3 size once if the ratio is under 1.51
        if (taktsCounter < m_takts)
        {
            taktsCounter++;
        }
        else
        {
            bool found = false;
            if (Screen.width > Screen.height)
            {
                if ((float)Screen.width / (float)Screen.height < 1.51f)
                {
                    found = true;
                }
            }
            else if ((float)Screen.height / (float)Screen.width < 1.51f)
            {
                found = true;
            }
            if (found)
            {
                var rectTransform = transform as RectTransform;
                Vector2 anchorMin = rectTransform.anchorMin;
                Vector2 anchorMax = rectTransform.anchorMax;
                rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
                rectTransform.anchorMax = rectTransform.anchorMin;
                rectTransform.sizeDelta = new Vector2(m_forceWidth4_3, m_forceHeight4_3);
                rectTransform.anchorMin = anchorMin;
                rectTransform.anchorMax = anchorMax;
                var layoutElement = base.GetComponent<LayoutElement>();
                if (layoutElement != null)
                {
                    layoutElement.preferredWidth = m_forceWidth4_3;
                    layoutElement.minWidth = m_forceWidth4_3;
                    layoutElement.preferredHeight = m_forceHeight4_3;
                    layoutElement.minHeight = m_forceHeight4_3;
                }
            }
            enabled = false;
        }
    }
}



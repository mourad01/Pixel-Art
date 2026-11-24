using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CustomVerticalLayoutGroup : MonoBehaviour
{
    List<RectTransform> m_childs;
    float m_rectHeight;

    [ContextMenu("ForceUpdate")]
    void ContextMenuForceUpdate()
    {
        ForceUpdate();
    }

    void Start()
    {
        m_childs = new List<RectTransform>();
        m_rectHeight = (transform as RectTransform).rect.height;
        Process();
    }

    void Update()
    {
        if ((transform as RectTransform).rect.height != m_rectHeight)
        {
            m_rectHeight = (transform as RectTransform).rect.height;
            Process();
        }
    }

    public void ForceUpdate()
    {
        Process();
    }

    void Process()
    {
        m_childs.Clear();
        var componentsInChildren = ((Component)transform).GetComponentsInChildren<CustomVerticalLayoutGroupElement>();
        if (componentsInChildren.Length > 0)
        {
            componentsInChildren = (from a in componentsInChildren
                                    orderby a.SiblingIndex
                                    select a).ToArray();
            for (int i = 0; i < componentsInChildren.Length; i++)
            {
                var rectTransform = componentsInChildren[i].transform as RectTransform;
                if (rectTransform.gameObject.activeSelf)
                {
                    m_childs.Add(rectTransform);
                }
            }
        }
        else
        {
            for (int j = 0; j < transform.childCount; j++)
            {
                var rectTransform = transform.GetChild(j) as RectTransform;
                if (rectTransform.gameObject.activeSelf)
                {
                    m_childs.Add(rectTransform);
                }
            }
        }
        float childsHeight = m_childs.Sum((RectTransform a) => a.rect.height);
        float value2 = m_rectHeight - childsHeight;
        float value3 = value2 / (float)m_childs.Count;
        float value4 = 0f;
        for (int k = 0; k < m_childs.Count; k++)
        {
            m_childs[k].anchoredPosition = new Vector2(0f, (0f - value3) * ((float)k + 0.5f) - value4 - m_childs[k].rect.height / 2f);
            value4 += m_childs[k].rect.height;
        }
    }
}



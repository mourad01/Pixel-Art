using System.Collections.Generic;
using UnityEngine;

public class MyHorizontalLayoutGroup : MonoBehaviour
{
    float m_fullLength;

    void Start()
    {
        Reinit();
    }

    void Update()
    {
        if (m_fullLength != ((RectTransform)transform).rect.width)
        {
            Reinit();
        }
    }

    void Reinit()
    {
        var list = new List<RectTransform>();
        for (int i = 0; i < transform.childCount; i++)
        {
            var rectTransform = (RectTransform)transform.GetChild(i);
            if (rectTransform.gameObject.activeSelf)
            {
                list.Add(rectTransform);
            }
        }
        m_fullLength = ((RectTransform)transform).rect.width;
        float step = m_fullLength / (float)(list.Count + 1);
        for (int j = 0; j < list.Count; j++)
        {
            list[j].anchoredPosition = new Vector2(step * (float)(j + 1), 0f);
        }
    }
}



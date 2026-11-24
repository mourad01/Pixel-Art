using UnityEngine;

public class SetRectAsTarget : MonoBehaviour
{
    [SerializeField] RectTransform m_target;

    void Start()
    {
        Rect rect = ((RectTransform)m_target.transform).rect;
        var rectTransform = (RectTransform)transform;
        rectTransform.sizeDelta = new Vector2(rect.width, rect.height);
        rectTransform.anchoredPosition = rect.center;
    }
}



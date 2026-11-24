using System;
using UnityEngine;
using UnityEngine.UI;

public class FilterElement : MonoBehaviour
{
    public Action<FilterElement> OnClick;

    [SerializeField] LayoutElement m_layoutElement;
    [SerializeField] RawImage m_image;
    [SerializeField] RawImage m_dark;

    public int FilterIndex { get; private set; }

    public void Init(FilterInfo info)
    {
        m_image.texture = info.Texture;
        FilterIndex = info.FilterIndex;
    }

    public void InitEmpty(Texture2D tex)
    {
        m_image.texture = tex;
        m_image.material = null;
        FilterIndex = 0;
    }

    public void Click()
    {
        OnClick.SafeInvoke(this);
    }

    public void Scale(float scale, float maxScale)
    {
        float a = scale / maxScale;
        a = Mathf.Min(a, 1f);
        m_layoutElement.preferredWidth = 150f * a;
        m_layoutElement.minWidth = 150f * a;
        ((RectTransform)transform).sizeDelta = 150f * new Vector2(a, a);
        m_dark.SetAlpha(1f - a);
    }
}





using UnityEngine;
using UnityEngine.UI;

public class ColorPointer : MonoBehaviour
{
    ColorImage m_colorImage;

    [SerializeField] Image m_image;
    [SerializeField] GameObject m_iphoneXAngle;

    public void UpdateColor(ColorImage colorImage, SpecialColorPosition pos)
    {
        if (m_colorImage != null)
        {
            m_colorImage.Unselect();
        }
        m_colorImage = colorImage;
        m_colorImage.Select();
        transform.SetParent(colorImage.transform);
        ((RectTransform)transform).sizeDelta = Vector2.zero;
        ((RectTransform)transform).anchoredPosition = Vector2.zero;
        m_iphoneXAngle.SetActive(false);
    }
}



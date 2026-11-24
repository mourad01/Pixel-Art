using UnityEngine;
using UnityEngine.UI;

public class BackgroundTileController : MonoBehaviour
{
    RectTransform m_rectTransform;
    Rect m_rect = default(Rect);
    Image m_image;

    [SerializeField] float m_elementSize = 512f;

    void Awake()
    {
        m_rectTransform = (RectTransform)transform;
        m_image = base.GetComponent<Image>();
        m_image.material = new Material(Shader.Find("Custom/TilingShader"));
    }

    void Update()
    {
        Rect rect = m_rectTransform.rect;
        if (rect != m_rect)
        {
            m_rect = rect;
            m_image.material.mainTextureScale = new Vector2(rect.width / m_elementSize, rect.height / m_elementSize);
        }
    }
}



using UnityEngine;
using UnityEngine.UI;

public class LibraryTabButton : MonoBehaviour
{
    [SerializeField] Image m_image;
    [SerializeField] Sprite m_activeSprite;
    [SerializeField] Sprite m_nonactiveSprite;
    [SerializeField] Text m_text;
    [SerializeField] Color m_activeColor = Color.white;
    [SerializeField] Color m_nonactiveColor = Color.white;

    public void SetHighlighted(bool value)
    {
        m_image.sprite = ((!value) ? m_nonactiveSprite : m_activeSprite);
        m_text.color = ((!value) ? m_nonactiveColor : m_activeColor);
    }
}



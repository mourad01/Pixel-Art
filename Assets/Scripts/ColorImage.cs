using System;
using UnityEngine;
using UnityEngine.UI;

public class ColorImage : MonoBehaviour
{
    public Action<ColorImage> OnClick;

    [SerializeField] Image m_image;
    [SerializeField] Text m_title;
    [SerializeField] Image m_mark;

    public Color Color { get; set; }

    public int ColorIndex { get;  set; }

    public void Init(Color color, int number)
    {
        Color = color;
        m_image.color = color;
        ColorIndex = number;
        m_title.text = number.ToString();
        m_title.color = ((!color.IsDark()) ? Color.black : Color.white);
        m_mark.color = ((!color.IsDark()) ? Color.black : Color.white);
    }

    public void Select()
    {
        m_title.transform.localScale = new Vector2(1.5f, 1.5f);
    }

    public void Unselect()
    {
        m_title.transform.localScale = new Vector2(1f, 1f);
    }

    public void Click()
    {
        OnClick.SafeInvoke(this);
        AudioManager.Instance.PlayClick();
    }

    public void Disable()
    {
        m_image.SetAlpha(0.5f);
        m_title.gameObject.SetActive(false);
        m_mark.gameObject.SetActive(true);
        ((Component)m_image).GetComponent<Button>().enabled = false;
    }
}



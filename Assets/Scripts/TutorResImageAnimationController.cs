using UnityEngine;
using UnityEngine.UI;

public class TutorResImageAnimationController : MonoBehaviour
{
    [SerializeField] Image m_resImage;

    Texture2D m_startTex;

    void Awake()
    {
        m_startTex = m_resImage.sprite.texture;
        ResetTexture();
    }

    public void ResetTexture()
    {
        var texture2D = new Texture2D(m_startTex.width, m_startTex.height, TextureFormat.ARGB32, false);
        texture2D.filterMode = FilterMode.Point;
        texture2D.SetPixels(m_startTex.GetPixels());
        texture2D.Apply();
        m_resImage.sprite = Sprite.Create(texture2D, new Rect(0f, 0f, (float)m_startTex.width, (float)m_startTex.height), new Vector2(0.5f, 0.5f));
    }

    public void AnimationEventHandler(int index)
    {
        switch (index)
        {
            case -1:
                ResetTexture();
                break;
            case 0:
                m_resImage.sprite.texture.SetPixel(12, 18, new Color(0.976f, 0.93f, 0.353f));
                break;
            case 1:
                m_resImage.sprite.texture.SetPixel(12, 17, new Color(0.976f, 0.93f, 0.353f));
                break;
            case 2:
                m_resImage.sprite.texture.SetPixel(12, 16, new Color(0.976f, 0.93f, 0.353f));
                break;
            case 3:
                m_resImage.sprite.texture.SetPixel(13, 16, new Color(0.976f, 0.93f, 0.353f));
                break;
            case 4:
                m_resImage.sprite.texture.SetPixel(13, 17, new Color(0.976f, 0.93f, 0.353f));
                break;
            case 5:
                m_resImage.sprite.texture.SetPixel(14, 18, new Color(0.976f, 0.93f, 0.353f));
                break;
            case 6:
                m_resImage.sprite.texture.SetPixel(14, 17, new Color(0.976f, 0.93f, 0.353f));
                break;
            case 7:
                m_resImage.sprite.texture.SetPixel(14, 16, new Color(0.976f, 0.93f, 0.353f));
                break;
            case 8:
                m_resImage.sprite.texture.SetPixel(14, 15, new Color(0.976f, 0.93f, 0.353f));
                break;
            case 9:
                m_resImage.sprite.texture.SetPixel(15, 15, new Color(0.976f, 0.93f, 0.353f));
                break;
            case 10:
                m_resImage.sprite.texture.SetPixel(15, 16, new Color(0.976f, 0.93f, 0.353f));
                break;
            case 11:
                m_resImage.sprite.texture.SetPixel(15, 17, new Color(0.976f, 0.93f, 0.353f));
                break;
            case 12:
                m_resImage.sprite.texture.SetPixel(16, 18, new Color(0.976f, 0.93f, 0.353f));
                break;
            case 13:
                m_resImage.sprite.texture.SetPixel(16, 17, new Color(0.976f, 0.93f, 0.353f));
                break;
            case 14:
                m_resImage.sprite.texture.SetPixel(16, 16, new Color(0.976f, 0.93f, 0.353f));
                break;
            case 15:
                m_resImage.sprite.texture.SetPixel(13, 15, new Color(0.976f, 0.93f, 0.353f));
                break;
        }
        m_resImage.sprite.texture.Apply();
    }
}



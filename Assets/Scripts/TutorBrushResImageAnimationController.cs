using UnityEngine;
using UnityEngine.UI;

public class TutorBrushResImageAnimationController : MonoBehaviour
{
    [SerializeField] Image m_resImage;
    [SerializeField] Image m_loupeResImage;

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
        m_loupeResImage.sprite = m_resImage.sprite;
    }

    // each animation event paints one green pixel of the brush demo, -1 resets
    public void AnimationEventHandler(int index)
    {
        switch (index)
        {
            case -1:
                ResetTexture();
                break;
            case 0:
                m_resImage.sprite.texture.SetPixel(14, 14, new Color(0.682f, 0.8f, 0.333f));
                break;
            case 1:
                m_resImage.sprite.texture.SetPixel(14, 13, new Color(0.682f, 0.8f, 0.333f));
                break;
            case 2:
                m_resImage.sprite.texture.SetPixel(14, 12, new Color(0.682f, 0.8f, 0.333f));
                break;
            case 3:
                m_resImage.sprite.texture.SetPixel(14, 11, new Color(0.682f, 0.8f, 0.333f));
                break;
            case 4:
                m_resImage.sprite.texture.SetPixel(14, 10, new Color(0.682f, 0.8f, 0.333f));
                break;
            case 5:
                m_resImage.sprite.texture.SetPixel(14, 9, new Color(0.682f, 0.8f, 0.333f));
                break;
            case 6:
                m_resImage.sprite.texture.SetPixel(15, 11, new Color(0.682f, 0.8f, 0.333f));
                break;
            case 7:
                m_resImage.sprite.texture.SetPixel(13, 12, new Color(0.682f, 0.8f, 0.333f));
                break;
            case 8:
                m_resImage.sprite.texture.SetPixel(13, 13, new Color(0.682f, 0.8f, 0.333f));
                break;
        }
        m_resImage.sprite.texture.Apply();
    }
}



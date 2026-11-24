using UnityObject = global::UnityEngine.Object;


using System;
using UnityEngine;
using UnityEngine.UI;

public class MyWorkPreview : MonoBehaviour
{
    public Action<string, MyWorkPreview> OnClick;

    string m_saveId;
    ISavedWorkData m_swd;

    [SerializeField] RawImage m_image;
    [SerializeField] GameObject m_imageWaiter;
    [SerializeField] GameObject m_completedTick;
    [SerializeField] RawImage m_filterImage;
    [SerializeField] GameObject m_3dMark;

    public bool IsInited { get; private set; }

    public bool Loaded
    {
        get
        {
            return m_image.texture != null;
        }
    }

    public Texture2D Texture
    {
        get
        {
            if (m_image.texture != null && m_image.texture is Texture2D)
                return (Texture2D)m_image.texture;
            return null;
        }
    }

    public string SaveId
    {
        get
        {
            return m_saveId;
        }
    }

    void Awake()
    {
        m_filterImage.material = new Material(Shader.Find("Custom/TilingShader"));
    }

    public void Init(string saveId)
    {
        IsInited = true;
        m_saveId = saveId;
        m_image.material = new Material(Shader.Find("Custom/GreyTextureShader"));
        //free textures of the previous save shown here
        if (m_image.texture != null)
        {
            UnityObject.Destroy(m_image.texture);
        }
        var texture = m_image.material.GetTexture("_ResTex");
        if (texture != null)
        {
            UnityObject.Destroy(texture);
            m_image.material.SetTexture("_ResTex", null);
        }
        m_image.texture = null;
        m_swd = MainManager.Instance.SavedWorksList.LoadById(m_saveId);
        if (m_swd != null)
        {
            m_completedTick.SetActive(m_swd.Completed);
            m_3dMark.SetActive(m_swd.ImageInfo.Is3D);
        }
        else
        {
            m_completedTick.SetActive(false);
            m_3dMark.SetActive(false);
            m_imageWaiter.SetActive(true);
        }
    }

    public void LoadIcon()
    {
        if (m_image.texture == null && m_swd != null)
        {
            m_imageWaiter.SetActive(true);
            if (m_swd.ImageInfo.Url != null)
            {
                DataManager.Instance.GetImageAsset(m_swd.ImageInfo, delegate (bool res, Texture2D tex)
                {
                    m_image.texture = tex;
                    m_image.enabled = true;
                    m_imageWaiter.SetActive(false);
                    // saved progress goes on top of the grey image, bilinear for 3D, point for 2D
                    var texture2D2 = new Texture2D(1, 1, TextureFormat.ARGB32, false);
                    texture2D2.filterMode = (FilterMode)(m_swd.ImageInfo.Is3D ? 1 : 0);
                    texture2D2.LoadImage(m_swd.Preview);
                    m_image.material.SetTexture("_ResTex", texture2D2);
                    var filter = MainManager.Instance.FilterManager.GetFilter(m_swd.FilterId);
                    if (filter != null)
                    {
                        m_filterImage.material.mainTextureScale = new Vector2((float)texture2D2.width, (float)texture2D2.height);
                        m_filterImage.texture = filter;
                        m_filterImage.enabled = true;
                        m_filterImage.transform.localScale = m_image.transform.localScale;

                        m_filterImage.material.SetFloat("RepeatX", m_image.texture.width);
                        m_filterImage.material.SetFloat("RepeatY", m_image.texture.height);
                    }
                    else
                    {
                        m_filterImage.enabled = false;
                    }
                });
            }
            else
            {
                DataManager.Instance.GetPhotoAsset(m_swd.ImageInfo.Id, delegate (bool res, Texture2D tex)
                {
                    m_image.texture = tex;
                    //photos keep their aspect ratio
                    if (tex.width > tex.height)
                    {
                        m_image.transform.localScale = new Vector3(1f, (float)tex.height / (float)tex.width, 1f);
                    }
                    else
                    {
                        m_image.transform.localScale = new Vector3((float)tex.width / (float)tex.height, 1f, 1f);
                    }
                    m_image.enabled = true;
                    m_imageWaiter.SetActive(false);
                    var texture2D = new Texture2D(1, 1, TextureFormat.ARGB32, false);
                    texture2D.filterMode = FilterMode.Point;
                    texture2D.LoadImage(m_swd.Preview);
                    m_image.material.SetTexture("_ResTex", texture2D);
                    var filter = MainManager.Instance.FilterManager.GetFilter(m_swd.FilterId);
                    if (filter != null)
                    {
                        m_filterImage.material.mainTextureScale = new Vector2((float)texture2D.width, (float)texture2D.height);
                        m_filterImage.texture = filter;
                        m_filterImage.enabled = true;
                        m_filterImage.transform.localScale = m_image.transform.localScale;

                        m_filterImage.material.SetFloat("RepeatX", m_image.texture.width);
                        m_filterImage.material.SetFloat("RepeatY", m_image.texture.height);
                    }
                    else
                    {
                        m_filterImage.enabled = false;
                    }
                });
            }
        }
    }

    public void UnloadIcon()
    {
        if (m_image.texture != null)
        {
            m_image.texture = null;
            m_image.enabled = false;
        }
    }

    public void Click()
    {
        OnClick.SafeInvoke(m_saveId, this);
        AudioManager.Instance.PlayClick();
    }
}



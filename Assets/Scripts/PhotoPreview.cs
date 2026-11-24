

using System;
using UnityEngine;
using UnityEngine.UI;

public class PhotoPreview : MonoBehaviour
{
    public Action<PhotoInfo, PhotoPreview> OnClick;

    PhotoInfo m_photoInfo;
    bool m_loadIcon;
    bool m_loadedIcon;
    bool m_materialInited;

    [SerializeField] RawImage m_image;
    [SerializeField] RawImage m_filterImage;
    [SerializeField] Text m_title;
    [SerializeField] GameObject m_imageWaiter;
    [SerializeField] GameObject m_completedMark;
    [SerializeField] GameObject m_plus;
    [SerializeField] GameObject m_adIcon;
    [SerializeField] GameObject m_premiumIcon;
    [SerializeField] Sprite m_enabledPlusSprite;
    [SerializeField] Sprite m_disabledPlusSprite;

    public bool Inited
    {
        get
        {
            return m_photoInfo != null;
        }
    }

    public bool Loaded
    {
        get
        {
            return m_image.texture != null;
        }
    }

    public string PhotoId
    {
        get
        {
            return (m_photoInfo != null) ? m_photoInfo.Id : null;
        }
    }

    public Texture2D Texture
    {
        get
        {
            return (Texture2D)m_image.texture;
        }
    }

    void Awake()
    {
        var debugController = DebugController.Instance;
        m_filterImage.material = new Material(Shader.Find("Custom/TilingShader"));
        //IAPWrapper instance2 = IAPWrapper.Instance;
        //instance2.OnPurchase = (Action<bool, SubscriptionType>)Delegate.Combine(instance2.OnPurchase, new Action<bool, SubscriptionType>(this.OnPurchaseHandler));
        //AdsWrapper instance3 = AdsWrapper.Instance;
        //instance3.OnVideoAvailabilityChanged = (Action)Delegate.Combine(instance3.OnVideoAvailabilityChanged, new Action(this.OnVideoAvailabilityChangedHandler));
    }

    public void Init(PhotoInfo photoInfo)
    {
        if (m_title != null && photoInfo != null)
        {
            m_title.text = photoInfo.Id;
        }
        m_photoInfo = photoInfo;
        m_image.texture = null;
        m_loadIcon = false;
        m_loadedIcon = false;
        if (!m_materialInited)
        {
            m_image.material = new Material(Shader.Find("Custom/GreyTextureShader"));
            m_materialInited = true;
        }
        m_image.material.SetTexture("_ResTex", null);
        Reinit();
        m_completedMark.SetActive(false);
        OnVideoAvailabilityChangedHandler();
    }

    public void LoadIcon()
    {
        //empty slot shows the plus button to add a photo
        if (string.IsNullOrEmpty(m_photoInfo.Id))
        {
            m_image.gameObject.SetActive(false);
            m_plus.SetActive(true);
            m_adIcon.SetActive(false);
            m_premiumIcon.SetActive(false);
        }
        else
        {
            m_plus.SetActive(false);
            if (!m_loadedIcon)
            {
                if (!m_loadIcon)
                {
                    m_loadIcon = true;
                    m_imageWaiter.SetActive(true);
                    DataManager.Instance.GetPhotoAsset(m_photoInfo.Id, delegate (bool res, Texture2D tex)
                    {
                        if (res)
                        {
                            try
                            {
                                m_loadedIcon = true;
                                m_image.texture = tex;
                                //keep the photo aspect ratio inside the square preview
                                if (tex.width > tex.height)
                                {
                                    m_image.transform.localScale = new Vector3(1f, (float)tex.height / (float)tex.width, 1f);
                                }
                                else
                                {
                                    m_image.transform.localScale = new Vector3((float)tex.width / (float)tex.height, 1f, 1f);
                                }
                                m_imageWaiter.SetActive(false);
                                m_image.gameObject.SetActive(true);
                                //draw the last saved progress and filter over the photo
                                string text = MainManager.Instance.SavedWorksList.LastSaveOfImageId(m_photoInfo.Id);
                                if (text != null)
                                {
                                    var texture2D = new Texture2D(1, 1, TextureFormat.ARGB32, false);
                                    texture2D.filterMode = FilterMode.Point;
                                    var savedWorkData = MainManager.Instance.SavedWorksList.LoadById(text);
                                    texture2D.LoadImage(savedWorkData.Preview);
                                    m_image.material.SetTexture("_ResTex", texture2D);
                                    m_completedMark.SetActive(savedWorkData.Completed);
                                    var filter = MainManager.Instance.FilterManager.GetFilter(savedWorkData.FilterId);
                                    if (filter != null)
                                    {
                                        m_filterImage.material.mainTextureScale = new Vector2((float)texture2D.width, (float)texture2D.height);
                                        m_filterImage.texture = filter;
                                        m_filterImage.enabled = true;
                                        m_filterImage.transform.localScale = m_image.transform.localScale;

                                        m_filterImage.material.SetFloat("RepeatX", texture2D.width);
                                        m_filterImage.material.SetFloat("RepeatY", texture2D.height);
                                    }
                                    else
                                    {
                                        m_filterImage.enabled = false;
                                    }
                                }
                                else
                                {
                                    m_filterImage.enabled = false;
                                    m_image.uvRect = new Rect(0f, 0f, 1f, 1f);
                                    m_filterImage.uvRect = m_image.uvRect;
                                }
                            }
                            catch (Exception ex)
                            {
                                Debug.Log(ex.Message);
                            }
                        }
                        m_loadIcon = false;
                    });
                }
            }
            else
            {
                m_image.gameObject.SetActive(true);
            }
        }
    }

    public void UnloadIcon()
    {
        m_loadIcon = false;
        m_image.gameObject.SetActive(false);
    }

    public void Reinit()
    {
    }

    public void Click()
    {
        OnClick.SafeInvoke(m_photoInfo, this);
        AudioManager.Instance.PlayClick();
    }

    void OnPurchaseHandler(bool res, SubscriptionType subscrType)
    {
        if (string.IsNullOrEmpty(m_photoInfo.Id))
        {
            m_adIcon.SetActive(false);
            m_premiumIcon.SetActive(false);
        }
    }

    void OnVideoAvailabilityChangedHandler()
    {
        if (m_plus.activeSelf)
        {
            if ( AppData.FreePhotos == 0)
            {
                /*if (INPluginWrapper.Instance.GetAbTestGroup() == ABTestGroup.RewardedNo_ContentEasy)
                {
                    //((Component)this.m_plus.transform.Find("plus")).GetComponent<Image>().sprite = ((!AdsWrapper.Instance.IsVideoAvailable()) ? this.m_disabledPlusSprite : this.m_enabledPlusSprite);
                    this.m_adIcon.SetActive(false);
                    this.m_premiumIcon.SetActive(false);
                }*/
            }
            else
            {
                ((Component)m_plus.transform.Find("plus")).GetComponent<Image>().sprite = m_enabledPlusSprite;
                m_adIcon.SetActive(false);
            }
        }
    }

    void OnDestroy()
    {
        var debugController = DebugController.Instance;
        //IAPWrapper instance2 = IAPWrapper.Instance;
        //instance2.OnPurchase = (Action<bool, SubscriptionType>)Delegate.Remove(instance2.OnPurchase, new Action<bool, SubscriptionType>(this.OnPurchaseHandler));
        //AdsWrapper instance3 = AdsWrapper.Instance;
        //instance3.OnVideoAvailabilityChanged = (Action)Delegate.Remove(instance3.OnVideoAvailabilityChanged, new Action(this.OnVideoAvailabilityChangedHandler));
    }
}



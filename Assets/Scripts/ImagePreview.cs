using UnityObject = global::UnityEngine.Object;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ImagePreview : MonoBehaviour
{
    public Action<ImageInfo, ImagePreview> OnClick;

    ImageInfo m_imageInfo;
    bool m_loadIcon;
    bool m_loadedIcon;
    bool m_materialInited;
    string m_saveId;

    [SerializeField] RawImage m_image;
    [SerializeField] RawImage m_filterImage;
    [SerializeField] GameObject m_locked;
    [SerializeField] GameObject m_adLocked;
    [SerializeField] GameObject m_adLocked_mozaic;
    [SerializeField] GameObject m_adLocked_mozaic_other_group;
    [SerializeField] GameObject m_adLocked_icon;
    [SerializeField] Text m_title;
    [SerializeField] GameObject m_imageWaiter;
    [SerializeField] GameObject m_completedMark;
    [SerializeField] GameObject m_3dMark;
    [SerializeField] bool m_isTodayArt = false;

    public bool TodayArt
    {
        get
        {
            return m_isTodayArt;
        }
    }

    public bool Inited
    {
        get
        {
            return m_imageInfo != null;
        }
    }
    public bool AdsAvailable
    {
        get
        {
            return m_adLocked.activeSelf;
        }
    }

    public bool Loaded
    {
        get
        {
            return m_image.texture != null;
        }
    }

    public bool Locked
    {
        get
        {
            return m_adLocked.activeSelf;
        }
    }

    public bool IsFreePremium
    {
        get
        {
            return m_locked.activeInHierarchy;
        }
    }

    public Texture2D Texture
    {
        get
        {
            return (!(m_image.texture == null)) ? ((Texture2D)m_image.texture) : null;
        }
    }

    public UnityAction<bool> CompleteMethod { get; private set; }

    void Awake()
    {
        m_filterImage.material = new Material(Shader.Find("Custom/TilingShader"));
    }

    public bool CheckTheSame(ImageInfo imageInfo)
    {
        return m_imageInfo != null && imageInfo != null && m_imageInfo.Id == imageInfo.Id;
    }

    public void Init(ImageInfo imageInfo)
    {
        if (m_title != null)
        {
            m_title.text = imageInfo.Id;
        }
        m_imageInfo = imageInfo;
        if (!m_materialInited)
        {
            m_image.material = new Material(Shader.Find("Custom/GreyTextureShader"));
            m_materialInited = true;
        }
        if (m_image.texture != null && !m_isTodayArt)
        {
            UnityObject.Destroy(m_image.texture);
        }
        var texture = m_image.material.GetTexture("_ResTex");
        if (texture != null && !m_isTodayArt)
        {
            UnityObject.Destroy(texture);
            m_image.material.SetTexture("_ResTex", null);
        }
        m_image.texture = null;
        m_loadIcon = false;
        m_loadedIcon = false;
        if (!m_isTodayArt)
        {
            Reinit();
        }
        m_completedMark.SetActive(false);
        m_3dMark.SetActive(imageInfo.Is3D);
    }

    void RecheckShaker(bool enabled)
    {
        if (m_isTodayArt)
        {
            var shaker = gameObject.GetComponentInParent<UIShaker>();
            if (shaker != null)
            {
                shaker.enabled = enabled;
            }
        }
    }
    public void LoadIcon()
    {
        if (!m_loadedIcon)
        {
            if (!m_loadIcon)
            {
                m_loadIcon = true;
                m_imageWaiter.SetActive(true);
                m_saveId = MainManager.Instance.SavedWorksList.LastSaveOfImageId(m_imageInfo.Id);
                //if (this.m_imageInfo.Is3D && this.m_saveId != null)
                //{
                //	this.m_imageWaiter.SetActive(false);
                //	this.m_image.gameObject.SetActive(true);
                //	base.StartCoroutine(this.LoadSaveCoroutine());
                //}
                //else
                {
                    DataManager.Instance.GetImageAsset(m_imageInfo, delegate (bool res, Texture2D tex)
                    {
                        if (res)
                        {
                            try
                            {
                                m_loadedIcon = true;
                                m_image.texture = tex;
                                m_imageWaiter.SetActive(false);
                                m_image.gameObject.SetActive(true);
                                if (m_saveId != null)
                                {
                                    if (gameObject.activeInHierarchy)
                                    {
                                        base.StartCoroutine(LoadSaveCoroutine());
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
                    if (m_imageInfo.Is3D)
                    {
                        DataManager.Instance.GetImageAsset3D(m_imageInfo, null);
                    }
                }
            }
        }
        else
        {
            m_image.gameObject.SetActive(true);
        }
    }

    public void UnloadIcon()
    {
        m_loadIcon = false;
        m_image.gameObject.SetActive(false);
    }


    public void Reinit()
    {


        /*if (IAPWrapper.Instance.NoAds || IAPWrapper.Instance.Subscribed)
        {
            this.m_adLocked.SetActive(false);
            if (this.m_adLocked_mozaic != null)
                this.m_adLocked_mozaic.SetActive(false);
            this.m_adLocked.GetComponent<Image>().enabled = false;
        }
        else if (INPluginWrapper.Instance.GetAbTestGroup() == ABTestGroup.None || INPluginWrapper.Instance.GetAbTestGroup() == ABTestGroup.RewardedNo_ContentMedium_Old)
        {
            this.m_adLocked.SetActive(this.m_imageInfo.AccessStatus != 0 && !IAPWrapper.Instance.Subscribed && !AppData.UnlockedImages.Contains(this.m_imageInfo.Id));
        }
        else if (INPluginWrapper.Instance.GetAbTestGroup() == ABTestGroup.RewardedNo_ContentMedium_Revealed)
        {
            this.m_adLocked.SetActive(this.m_imageInfo.AccessStatus != 0 && !IAPWrapper.Instance.Subscribed && !AppData.UnlockedImages.Contains(this.m_imageInfo.Id));
            if (!this.m_adLocked.activeInHierarchy)
            {
                if (this.m_adLocked_mozaic != null)
                    this.m_adLocked_mozaic.SetActive(false);
                this.m_adLocked.GetComponent<Image>().enabled = false;
            }
        }
        else
        {
            bool active = this.m_imageInfo.CustomAccessStatus != 0 && !IAPWrapper.Instance.Subscribed && !AppData.UnlockedImages.Contains(this.m_imageInfo.Id);
            this.m_locked.SetActive(active);
            //this.m_adLocked_mozaic_other_group.SetActive(active);
        }*/
    }





    public void Click()
    {

        if (m_adLocked.activeInHierarchy)
        {
            AdsManager.instance.ShowReward(()=>{
                CompleteMethod?.Invoke(true);
            },null);


        }
        else
        {
            AdsManager.instance.ShowInterstitial(null);


    }
        OnClick.SafeInvoke(m_imageInfo, this);
        AudioManager.Instance.PlayClick();

    }

   

    IEnumerator LoadSaveCoroutine()
    {
        //if (this.isActiveAndEnabled)
        //{
        //	yield return null;
        //}
        var resTex = new Texture2D(1, 1, TextureFormat.ARGB32, false);
        resTex.filterMode = (FilterMode)(m_imageInfo.Is3D ? 1 : 0);
        var swd = MainManager.Instance.SavedWorksList.LoadById(m_saveId);

        //yield return null;

        resTex.LoadImage(swd.Preview);
        m_image.material.SetTexture("_ResTex", resTex);
        m_completedMark.SetActive(swd.Completed);

        RecheckShaker(!swd.Completed);
        var filter = MainManager.Instance.FilterManager.GetFilter(swd.FilterId);
        if (filter != null)
        {
            m_filterImage.material.mainTextureScale = new Vector2((float)resTex.width, (float)resTex.height);
            m_filterImage.texture = filter;
            m_filterImage.enabled = true;
            m_filterImage.material.SetFloat("RepeatX", m_image.texture.width);
            m_filterImage.material.SetFloat("RepeatY", m_image.texture.height);
        }
        else
        {
            m_filterImage.enabled = false;
        }

        yield return null;
    }

}


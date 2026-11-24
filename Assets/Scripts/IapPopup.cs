using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class IapPopup : BaseWindow
{
    Action<bool> FinishHandler;
    ImageInfo m_imageInfo;

    [SerializeField] GameObject m_subscrPart;
    [SerializeField] GameObject m_subscrVideoPart;
    [SerializeField] GameObject mozaic;
    [SerializeField] GameObject mozaic2;
    [SerializeField] RawImage m_image1;
    [SerializeField] RawImage m_image2;
    [SerializeField] Image m_picImage1;
    [SerializeField] Image m_picImage2;
    [SerializeField] Sprite m_defaultTex;
    [SerializeField] Sprite m_bombTex;
    [SerializeField] Sprite m_lassoTex;
    [SerializeField] Text m_changableText;

    ImagePreview m_imagePreview;
    string m_place = string.Empty;

    protected override string WindowName
    {
        get
        {
            return "abtestWindow";
        }
    }

    void Awake()
    {
        //both previews share one grey material
        var image = m_image1;
        var material = new Material(Shader.Find("Custom/GreyTextureShader"));
        m_image2.material = material;
        image.material = material;
        //IAPWrapper instance = IAPWrapper.Instance;
        //instance.OnPurchase = (Action<bool, SubscriptionType>)Delegate.Combine(instance.OnPurchase, new Action<bool, SubscriptionType>(this.OnPurchaseHandler));
    }

    public void Init(ImageInfo imageInfo, ImagePreview imagePreview)
    {
        m_place = "image";
        m_imagePreview = imagePreview;

        AnalyticsManager.Instance.ABTestWindowOpened(m_place);
        m_picImage1.gameObject.SetActive(false);
        m_picImage2.gameObject.SetActive(false);
        m_image1.gameObject.SetActive(true);
        m_image2.gameObject.SetActive(true);
        m_image1.texture = null;
        m_image2.texture = null;
        DataManager.Instance.GetImageAsset(imageInfo, delegate (bool res, Texture2D tex)
        {
            m_image1.texture = tex;
            m_image2.texture = tex;
        });
        m_imageInfo = imageInfo;
        //this.SwitchAbTestGroup(INPluginWrapper.Instance.GetAbTestGroup());
        FinishHandler = null;
        m_changableText.text = LocalizationManager.Instance.GetString("get_this_image");
        CheckVisibleImage();
    }

    public void Init(Action<bool> handler, ABTestGroup abTestGroup, AbTestWindowMode mode)
    {
        CheckVisibleImage();
        m_imageInfo = null;
        FinishHandler = handler;
        SwitchAbTestGroup(abTestGroup);
        //booster or photo mode shows its own icon and text instead of an image
        var sprite = m_defaultTex;
        switch (mode)
        {
            case AbTestWindowMode.Bomb:
                sprite = m_bombTex;
                m_place = "grenade";
                m_changableText.text = LocalizationManager.Instance.GetString("get_bomb");
                break;
            case AbTestWindowMode.Lasso:
                sprite = m_lassoTex;
                m_place = "wand";
                m_changableText.text = LocalizationManager.Instance.GetString("get_wand");
                break;
            case AbTestWindowMode.Photo:
                sprite = m_defaultTex;
                m_place = "photo";
                m_changableText.text = LocalizationManager.Instance.GetString("create_art");
                break;
        }
        AnalyticsManager.Instance.ABTestWindowOpened(m_place);
        m_picImage1.gameObject.SetActive(true);
        m_picImage2.gameObject.SetActive(true);
        m_image1.gameObject.SetActive(false);
        m_image2.gameObject.SetActive(false);
        m_picImage1.sprite = sprite;
        m_picImage2.sprite = sprite;
    }

    void CheckVisibleImage()
    {
        if (m_imageInfo != null)
        {
            /*if (IAPWrapper.Instance.NoAds || IAPWrapper.Instance.Subscribed)
            {
                this.mozaic.SetActive(false);
            }
            else if (INPluginWrapper.Instance.GetAbTestGroup() == ABTestGroup.None || INPluginWrapper.Instance.GetAbTestGroup() == ABTestGroup.RewardedNo_ContentMedium_Old)
            {
                this.mozaic.SetActive(this.m_imageInfo.AccessStatus != 0 && !IAPWrapper.Instance.Subscribed && !AppData.UnlockedImages.Contains(this.m_imageInfo.Id));
            }
            else if (INPluginWrapper.Instance.GetAbTestGroup() == ABTestGroup.RewardedNo_ContentMedium_Revealed)
            {
                this.mozaic.SetActive(false);
            }
            else
            {
                bool active = this.m_imageInfo.CustomAccessStatus != 0 && !IAPWrapper.Instance.Subscribed && !AppData.UnlockedImages.Contains(this.m_imageInfo.Id);
                this.mozaic.SetActive(active);
            }*/
        }
        else
        {
            mozaic.SetActive(false);
        }
    }

    void SwitchAbTestGroup(ABTestGroup abTestGroup)
    {
        switch (abTestGroup)
        {
            case ABTestGroup.RewardedNo_ContentMedium:
            case ABTestGroup.RewardedNo_ContentHard:
            case ABTestGroup.RewardedNo_ContentEasy:
                //groups without rewarded video only get the subscribe part
                m_subscrPart.SetActive(true);
                m_subscrVideoPart.SetActive(false);
                break;
            default:
                /*if (AdsWrapper.Instance.IsVideoAvailable() && m_imagePreview != null && m_imagePreview.AdsAvailable)
                {
                    this.m_subscrPart.SetActive(false);
                    this.m_subscrVideoPart.SetActive(true);
                }
                else
                {
                    this.m_subscrPart.SetActive(true);
                    this.m_subscrVideoPart.SetActive(false);
                }*/
                break;
        }
    }

    public void BuyButtonClick()
    {
        //ABTestGroup abTestGroup = INPluginWrapper.Instance.GetAbTestGroup();
        //if (abTestGroup == ABTestGroup.RewardedNo_ContentMedium_Old || abTestGroup == ABTestGroup.None)
        {
            var newInappsWindow = WindowManager.Instance.OpenInappsWindow();
            newInappsWindow.Init(m_place, null);
            WindowManager.Instance.CloseMe(this);
        }
        //else
        //{
        //	TrialInappsWindow trialInappsWindow = WindowManager.Instance.OpenTrialInappsWindow();
        //	trialInappsWindow.Init(this.m_place, null, false, false);
        //	WindowManager.Instance.CloseMe(this);
        //}
        AnalyticsManager.Instance.ABTestWindowAction("subscribe", m_place);
    }

    public void WatchButtonClick()
    {
        /*AdsWrapper.Instance.ShowVideo(this.m_place, delegate (bool res)
        {
            if (res)
            {
                if (this.m_imageInfo != null)
                {
                    List<string> unlockedImages = AppData.UnlockedImages;
                    unlockedImages.Add(this.m_imageInfo.Id);
                    AppData.UnlockedImages = unlockedImages;
                    MainManager.Instance.StartWorkbook(this.m_imageInfo, ImageOpenType.New, MainMenuPage.Library, null);
                }
                else
                {
                    this.FinishHandler.SafeInvoke(true);
                }
            }
        });
        AnalyticsManager.Instance.ABTestWindowAction("view_rewarded", this.m_place);*/
    }

    public void CloseButtonClick()
    {
        WindowManager.Instance.CloseMe(this);
        AnalyticsManager.Instance.ABTestWindowAction("close", m_place);
    }

    void OnPurchaseHandler(bool res, SubscriptionType type)
    {
        if (gameObject.activeSelf)
        {
            WindowManager.Instance.CloseMe(this);
        }
    }

    void OnDestroy()
    {
        //IAPWrapper instance = IAPWrapper.Instance;
        //instance.OnPurchase = (Action<bool, SubscriptionType>)Delegate.Remove(instance.OnPurchase, new Action<bool, SubscriptionType>(this.OnPurchaseHandler));
    }
}



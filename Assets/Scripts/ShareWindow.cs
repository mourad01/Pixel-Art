


using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ShareWindow : BaseWindow
{
    //[SerializeField]
    //private GameObject m_watermark;

    [SerializeField] RawImage m_image;
    [SerializeField] Popup m_popup;
    [SerializeField] GameObject m_okButton;
    [SerializeField] protected RawImage m_filterImage;
    [SerializeField] Button m_facebookButton;
    [SerializeField] ButtonEffectComponent m_facebookButtonEventTrigger;
    [SerializeField] List<Image> m_facebookButtonImages;
    [SerializeField] Button m_instButton;
    [SerializeField] ButtonEffectComponent m_instButtonEventTrigger;
    [SerializeField] List<Image> m_instButtonImages;
    [SerializeField] GameObject m_playButton;
    [SerializeField] Text m_title;

    ISavedWorkData m_savedWorkData;
    bool m_waitTimeForRateUs;
    string m_videoPath;
    bool m_isColoring;
    bool m_completed;
    Vector2 _imageSize; 

    protected override string WindowName
    {
        get
        {
            return "shareWindow";
        }
    }

    protected void Awake()
    {
        m_image.material = new Material(Shader.Find("Custom/GreyTextureShader"));
        m_filterImage.material = new Material(Shader.Find("Custom/TilingShader"));
        OnOpen += OnOpenHandler;
    }

    public void Init(ISavedWorkData savedWorkData)
    {
        m_savedWorkData = savedWorkData;
        m_waitTimeForRateUs = false;
        ReinitImage(true);
        UpdateSocialButtons(); 
        m_title.text = LocalizationManager.Instance.GetString("share_picture");
    }

    public void InitFromLibrary(string saveId)
    {
        m_waitTimeForRateUs = false;
        m_popup.Init();
        m_savedWorkData = MainManager.Instance.SavedWorksList.LoadById(saveId);
        gameObject.SetActive(true);
        ReinitImage(false);
        UpdateSocialButtons(); 
        m_okButton.SetActive(false);
        m_title.text = LocalizationManager.Instance.GetString("share_picture");
    }

    void OnOpenHandler()
    {
        m_isColoring = false;
        m_completed = true; 
        PlayButtonClick();
    }

    void ReinitImage(bool withRect)
    {
        //saved coloring preview goes in _ResTex on top of the source image
        var resTex = new Texture2D(1, 1, TextureFormat.ARGB32, false);
        resTex.filterMode = FilterMode.Point;
        resTex.LoadImage(m_savedWorkData.Preview);
        m_image.material.SetTexture("_ResTex", resTex);
        m_image.enabled = true;
        m_filterImage.enabled = false;
        //library images have a url, user photos are loaded by id
        if (m_savedWorkData.ImageInfo.Url != null)
        {
            DataManager.Instance.GetImageAsset(m_savedWorkData.ImageInfo, delegate (bool res, Texture2D tex)
            {
                m_image.texture = tex;
                if (m_savedWorkData.Completed)
                {
                    m_image.material.SetInt("_HideMainTex", 1);
                }
                //filter overlay repeated once per image pixel
                var filter = MainManager.Instance.FilterManager.GetFilter(m_savedWorkData.FilterId);
                if (filter != null)
                {
                    m_filterImage.material.mainTextureScale = new Vector2((float)resTex.width, (float)resTex.height);
                    m_filterImage.texture = filter;
                    m_filterImage.enabled = true;

                    _imageSize = new Vector2(m_image.texture.width, m_image.texture.height);
                    m_filterImage.material.SetFloat("RepeatX", _imageSize.x);
                    m_filterImage.material.SetFloat("RepeatY", _imageSize.y);
                }
                else
                {
                    m_filterImage.enabled = false;
                }
                //keep the saved visible area of the work
                if (withRect)
                {
                    m_image.uvRect = m_savedWorkData.UvRect1;
                    m_filterImage.uvRect = m_image.uvRect;
                }
            });
        }
        else
        {
            DataManager.Instance.GetPhotoAsset(m_savedWorkData.ImageInfo.Id, delegate (bool res, Texture2D tex)
            {
                m_image.texture = tex;
                if (m_savedWorkData.Completed)
                {
                    m_image.material.SetInt("_HideMainTex", 1);
                }
                var filter = MainManager.Instance.FilterManager.GetFilter(m_savedWorkData.FilterId);
                if (filter != null)
                {
                    m_filterImage.material.mainTextureScale = new Vector2((float)resTex.width, (float)resTex.height);
                    m_filterImage.texture = filter;
                    m_filterImage.enabled = true;

                    _imageSize = new Vector2(m_image.texture.width, m_image.texture.height);
                    m_filterImage.material.SetFloat("RepeatX", _imageSize.x);
                    m_filterImage.material.SetFloat("RepeatY", _imageSize.y);
                }
                else
                {
                    m_filterImage.enabled = false;
                }
                if (withRect)
                {
                    m_image.uvRect = m_savedWorkData.UvRect1;
                    m_filterImage.uvRect = m_image.uvRect;
                }
            });
        }
    }

    void UpdateSocialButtons()
    {
        //unavailable share buttons are disabled and drawn at half alpha
        bool canFbShare = ShareWrapper.CanFbShare();
        m_facebookButton.interactable = canFbShare;
        foreach (Image facebookButtonImage in m_facebookButtonImages)
        {
            if (facebookButtonImage != null)
            {
                Color color = facebookButtonImage.color;
                color.a = ((!canFbShare) ? 0.5f : 1f);
                facebookButtonImage.color = color;
            }
        }
        canFbShare = ShareWrapper.CanInstShare();
        m_instButton.interactable = canFbShare;
        foreach (Image instButtonImage in m_instButtonImages)
        {
            if (instButtonImage != null)
            {
                Color instColor = instButtonImage.color;
                instColor.a = ((!canFbShare) ? 0.5f : 1f);
                instButtonImage.color = instColor;
            }
        }
    }

    public void DeleteWatermark()
    {
        AnalyticsManager.Instance.DeleteWatermarkAttempt();
        /*switch (INPluginWrapper.Instance.GetAbTestGroup())
        {
            case ABTestGroup.RewardedYes_ContentMedium_New:
            case ABTestGroup.RewardedYes_ContentHard:
            case ABTestGroup.Rewarded_yes_content_medium_no1screen:
            case ABTestGroup.Rewarded_yes_content_hard_no1screen:
                {
                    NewInappsWindow newInappsWindow = WindowManager.Instance.OpenInappsWindow();
                    newInappsWindow.Init("watermark", null);
                    break;
                }
            default:
                //IAPWrapper.Instance.BuyProduct(SubscriptionType.remove_ads, "watermark", null);
                break;
        }*/
    }

    void OnPurchaseHandler(bool res, SubscriptionType subscription)
    { 
    } 

    public void SaveButtonClick()
    {
        if (!m_completed)
        { 
            ShowMessage("Please wait...");
        }
        else
        { 
            //write the filtered image as png then copy it to the gallery
            string filePath = AppPathsConfig.SavedImagesPath + m_savedWorkData.Id + ".png";
            MainManager.Instance.FreeImageSaver.GetFilteredImage(m_savedWorkData, delegate (byte[] bytes)
            {
                File.WriteAllBytes(filePath, bytes);
                SaveIntoGallery(filePath);
            }, m_savedWorkData.ImageInfo.Is3D); 
            AudioManager.Instance.PlayClick();
        }
    } 

    void SaveIntoGallery(string filePath)
    {
        Debug.Log("Save into gallery: " + filePath);
        ShareWrapper.SaveIntoGallery(filePath, m_savedWorkData.Id, string.Empty, delegate (bool res)
        {
            ShowMessage((!res) ? "error" : "saved");
            if (res)
            {
                AnalyticsManager.Instance.SaveSuccess();
                base.StartCoroutine(ShowRateUsIfNeedCoroutine(RateUsReason.SaveInGallery));
            }
        });
    }

    public void ShareButtonClick()
    {
        if (!m_completed)
        { 
            ShowMessage("Please wait...");
        }
        else
        {
            AnalyticsManager.Instance.ShareButtonClicked(); 
            string filePath = AppPathsConfig.SavedImagesPath + m_savedWorkData.Id + ".png";
            MainManager.Instance.FreeImageSaver.GetFilteredImage(m_savedWorkData, delegate (byte[] bytes)
            {
                File.WriteAllBytes(filePath, bytes);
                ShareWrapper.ShareImage("Share image..", "Subject", LocalizationManager.Instance.GetString("share_image_text"), filePath);
            }, m_savedWorkData.ImageInfo.Is3D); 
            AudioManager.Instance.PlayClick();
        }
    }

    public void FacebookButtonClick()
    {
        if (!m_completed)
        { 
            ShowMessage("Please wait...");
        }
        else
        {
            AnalyticsManager.Instance.ShareViaFbButtonClicked(); 
            string filePath = AppPathsConfig.SavedImagesPath + m_savedWorkData.Id + ".png";
            MainManager.Instance.FreeImageSaver.GetFilteredImage(m_savedWorkData, delegate (byte[] bytes)
            {
                File.WriteAllBytes(filePath, bytes);
                ShareWrapper.ShareImageFb("Share image..", "Subject", LocalizationManager.Instance.GetString("share_image_text"), filePath);
            }, m_savedWorkData.ImageInfo.Is3D); 
            AudioManager.Instance.PlayClick();
        }
    }

    public void InstagramButtonClick()
    {
        if (!m_completed)
        { 
            ShowMessage("Please wait...");
        }
        else
        {
            AnalyticsManager.Instance.ShareViaInstagramButtonClicked(); 
            string filePath = AppPathsConfig.SavedImagesPath + m_savedWorkData.Id + ".png";
            Debug.Log(filePath);
            MainManager.Instance.FreeImageSaver.GetFilteredImage(m_savedWorkData, delegate (byte[] bytes)
            {
                File.WriteAllBytes(filePath, bytes);
                ShareWrapper.ShareImageInstagram("Share image..", "Subject", LocalizationManager.Instance.GetString("share_image_text"), filePath);
            }, m_savedWorkData.ImageInfo.Is3D); 
            AudioManager.Instance.PlayClick();
        }
    } 

    public void PlayButtonClick()
    {
    }


    void ShowMessage(string text)
    {
        m_popup.gameObject.SetActive(true);
        m_popup.Show(LocalizationManager.Instance.GetString(text));
    }

    public void ReturnToMenu()
    {
        AudioManager.Instance.PlayClick();
        //in the library just close, from a coloring scene go back to the library
        if (SceneManager.GetActiveScene().name == "LibraryScene")
        {
            WindowManager.Instance.CloseMe(this);
        }
        else
        {
            if (SceneManager.GetActiveScene().name == "2DScene")
            {
                NewWorkbookManager.Instance.SaveWork(false);
            }
            MainManager.Instance.StartLibrary(MainMenu.LastPage);
        }
    }

    public void CloseButtonClick()
    {
        WindowManager.Instance.CloseMe(this);
        AnalyticsManager.Instance.BackButtonClicked();
        AudioManager.Instance.PlayClick();
    }

    void OnDestroy()
    {
        //IAPWrapper instance = IAPWrapper.Instance;
        //instance.OnPurchase = (Action<bool, SubscriptionType>)Delegate.Remove(instance.OnPurchase, new Action<bool, SubscriptionType>(this.OnPurchaseHandler));
    } 

    IEnumerator ShowRateUsIfNeedCoroutine(RateUsReason reason)
    {
        m_waitTimeForRateUs = true;
        //wait for the popup to close, then 1 more second
        while (m_popup.isActiveAndEnabled)
        {
            yield return null;
        }
        yield return new WaitForSeconds(1f);

        if (m_waitTimeForRateUs)
        {
            m_waitTimeForRateUs = false;
            if (RateUsWindow.NeedShow(reason))
            {
                DialogToolWrapper.ShowRateUsDialog(null);
            }
        }
    }
}

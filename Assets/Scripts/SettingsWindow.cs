

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingsWindow : BaseWindow
{
    bool restoredAnything;

    [SerializeField] Popup m_popup;
    [SerializeField] GameObject m_vibroRightPart;
    [SerializeField] GameObject m_vibroWrongPart;
    [SerializeField] Switcher m_vibroRightSwitcher;
    [SerializeField] Switcher m_vibroWrongSwitcher;
    [SerializeField] Switcher m_soundsSwitcher;
    [SerializeField] Switcher m_highlightCellsSwitcher;
    [SerializeField] Switcher m_loupeSwitcher;
    [SerializeField] GameObject m_removeAdsButton;
    [SerializeField] Text m_groupText;

    protected override string WindowName
    {
        get
        {
            return "mainMenu_moreTab";
        }
    }

    void OnGUI()
    {
    }

    void Awake()
    {
        m_vibroRightSwitcher.UpdateState(AppData.VibroRightEnabled, true);
        m_vibroWrongSwitcher.UpdateState(AppData.VibroWrongEnabled, true);
        m_soundsSwitcher.UpdateState(AppData.SoundsEnabled, true);
        m_highlightCellsSwitcher.UpdateState(AppData.BulbMode, true);
        m_loupeSwitcher.UpdateState(AppData.LoupeEnabled, true);
        m_vibroRightPart.SetActive(VibroWrapper.IsVibroAvailable());
        m_vibroWrongPart.SetActive(VibroWrapper.IsVibroAvailable());
        //IAPWrapper instance = IAPWrapper.Instance;
        //instance.OnPurchase = (Action<bool, SubscriptionType>)Delegate.Combine(instance.OnPurchase, new Action<bool, SubscriptionType>(this.OnPurchaseHandlerRemoveAds));
        OnPurchaseHandlerRemoveAds(false, SubscriptionType.remove_ads);
    }

    public void VibroRightButtonClick()
    {
        AppData.VibroRightEnabled = !AppData.VibroRightEnabled;
        m_vibroRightSwitcher.UpdateState(AppData.VibroRightEnabled, false);
        AudioManager.Instance.PlayClick();
    }

    public void VibroWrongButtonClick()
    {
        AppData.VibroWrongEnabled = !AppData.VibroWrongEnabled;
        m_vibroWrongSwitcher.UpdateState(AppData.VibroWrongEnabled, false);
        AudioManager.Instance.PlayClick();
    }

    public void SoundButtonClick()
    {
        AppData.SoundsEnabled = !AppData.SoundsEnabled;
        m_soundsSwitcher.UpdateState(AppData.SoundsEnabled, false);
        AudioManager.Instance.PlayClick();
        //INPluginWrapper.Instance.UpdateSoundState();

        if (BackgroundMusic.musicMgr != null)
        {
            BackgroundMusic.musicMgr.RefreshSettings();
        }
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.RefreshSettings();
        }
    }

    public void HighlightCellsClick()
    {
        AppData.BulbMode = !AppData.BulbMode;
        m_highlightCellsSwitcher.UpdateState(AppData.BulbMode, false);
        AudioManager.Instance.PlayClick();
    }

    public void LoupeClick()
    {
        AppData.LoupeEnabled = !AppData.LoupeEnabled;
        m_loupeSwitcher.UpdateState(AppData.LoupeEnabled, false);
        AudioManager.Instance.PlayClick();
    }

    public void TutorialButtonClick()
    {
        var tutorialWindow = WindowManager.Instance.OpenTutorial();
        tutorialWindow.Init("settings", "2D");
        AudioManager.Instance.PlayClick();
    }

    public void Tutorial3DButtonClick()
    {
        var tutorialWindow = WindowManager.Instance.OpenTutorial3D();
        tutorialWindow.Init("settings", "3D");
        AudioManager.Instance.PlayClick();
    }

    public void RateUsButtonClick()
    {
        AnalyticsManager.Instance.RateUsClicked();
        RateUsTool.OpenRateUs();
        AudioManager.Instance.PlayClick();
    }

    public void HelpButtonClick()
    {
        AudioManager.Instance.PlayClick();
    }

    public void SupportButtonClick()
    {
        AudioManager.Instance.PlayClick();
    }

    public void ShareAppButtonClick()
    {
        ShareWrapper.ShareApp();
    }

    public void TermsButtonClick()
    {
        Application.OpenURL(AppPathsConfig.TermsUrl);
    }

    public void PrivacyButtonClick()
    {
        Application.OpenURL(AppPathsConfig.PrivacyUrl);
    }

    public void MoreAppsButtonClick()
    {
        AnalyticsManager.Instance.MoreAppsClicked();
        Application.OpenURL("https://play.google.com/store/apps/dev?id=6817085628932553524");
    }

    public void RemoveAdsClick()
    {
        //IAPWrapper.Instance.BuyProduct(SubscriptionType.remove_ads, "settings", null);
    }

    void OnPurchaseHandlerRemoveAds(bool res, SubscriptionType subscrType)
    {
        /*switch (INPluginWrapper.Instance.GetAbTestGroup())
        {
            case ABTestGroup.None:
            case ABTestGroup.RewardedNo_ContentEasy:
            case ABTestGroup.RewardedNo_ContentMedium_Old:
            case ABTestGroup.RewardedNo_ContentMedium_Revealed:
                //this.m_removeAdsButton.SetActive(!IAPWrapper.Instance.NoAds);
                break;
            default:
                this.m_removeAdsButton.SetActive(false);
                break;
        }*/
    }

    public void RestoreButtonClick()
    {
        //IAPWrapper instance = IAPWrapper.Instance;
        //instance.OnPurchase = (Action<bool, SubscriptionType>)Delegate.Combine(instance.OnPurchase, new Action<bool, SubscriptionType>(this.OnPurchaseHandler));
        base.StartCoroutine(WaitForRestored());
        //IAPWrapper.Instance.RestorePurchases(null);
        AnalyticsManager.Instance.RestoreButtonClicked();
        AudioManager.Instance.PlayClick();
    }

    void OnPurchaseHandler(bool res, SubscriptionType type)
    {
        restoredAnything = true;
        //IAPWrapper instance = IAPWrapper.Instance;
        //instance.OnPurchase = (Action<bool, SubscriptionType>)Delegate.Remove(instance.OnPurchase, new Action<bool, SubscriptionType>(this.OnPurchaseHandler));
    }
    public void ChangeGroupButtonClick()
    {
        /*ABTestGroup abTestGroup = INPluginWrapper.Instance.GetAbTestGroup();
        abTestGroup++;
        if (abTestGroup > ABTestGroup.RewardedNo_ContentMedium_Revealed)
        {
            abTestGroup = ABTestGroup.None;
        }
        INPluginWrapper.Instance.SetDefaultAbTestGroup(abTestGroup);*/
        UpdateGroupText();
        OnPurchaseHandlerRemoveAds(false, SubscriptionType.remove_ads);
    }

    void UpdateGroupText()
    {
        //this.m_groupText.text = INPluginWrapper.Instance.GetAbTestGroup().ToString();
    }

    public void CloseButtonClick()
    {
        WindowManager.Instance.CloseMe(this);
        AnalyticsManager.Instance.BackButtonClicked();
        AudioManager.Instance.PlayClick();
    }

    void OnEnable()
    {
        UpdateGroupText();
    }

    void OnDestroy()
    {
        //IAPWrapper instance = IAPWrapper.Instance;
        //instance.OnPurchase = (Action<bool, SubscriptionType>)Delegate.Remove(instance.OnPurchase, new Action<bool, SubscriptionType>(this.OnPurchaseHandlerRemoveAds));
    }
    IEnumerator WaitForRestored()
    {
        var timer = 2f;
        while (true)
        {
            if (timer > 0f)
            {
                timer -= Time.deltaTime;
                yield return null;
                continue;
            }
            //IAPWrapper instance = IAPWrapper.Instance;
            //instance.OnPurchase = (Action<bool, SubscriptionType>)Delegate.Remove(instance.OnPurchase, new Action<bool, SubscriptionType>(this.OnPurchaseHandler));
            /*if (this.restoredAnything)
            {
                if (IAPWrapper.Instance.Subscribed)
                {
                    AnalyticsManager.Instance.PurchasesRestored();
                    this.m_popup.Show(LocalizationManager.Instance.GetString("restored1"));
                }
                else
                {
                    this.m_popup.Show(LocalizationManager.Instance.GetString("restored2"));
                }
            }
            else
            {
                this.m_popup.Show(LocalizationManager.Instance.GetString("restored0"));
            }*/
            yield break;
        }
    }
}

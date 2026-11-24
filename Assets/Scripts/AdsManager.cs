using System;
using UnityEngine;

public class AdsManager : MonoBehaviour
{
    public static AdsManager instance;

    void Start()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void InitializeRewardedAds() { }

    public void InitializeBannerAds() { }

    public void InitializeInterstitialAds() { }

    public void ShowBanner() { }

    public void ShowInterstitial(Action onClosed)
    {
        onClosed?.Invoke();
    }

    public void ShowInterstitialTimer(Action onClosed)
    {
        ShowInterstitial(onClosed);
    }

    public void ShowReward(Action onRewarded, Action onUnavailable)
    {
        onUnavailable?.Invoke();
    }
}

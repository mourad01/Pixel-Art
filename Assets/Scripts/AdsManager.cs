using UnityEngine;

using System;
using static MaxSdkBase;

public class AdsManager : MonoBehaviour
{

    #region Singletone 
    public static AdsManager instance;
    private void Start()
    {
        if (instance == null)
            instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);
    }
    #endregion

    [Header("Ads IDs")]
    public string adRewardUnitId = "YOUR_AD_UNIT_ID";
    public string adInterstitialUnitId = "YOUR_AD_UNIT_ID";
    public string bannerAdUnitId = "YOUR_BANNER_AD_UNIT_ID"; // Retrieve the ID from your account
    int retryAttempt;
    int retryAttempt2;



    void Init()
    {
        MaxSdkCallbacks.OnSdkInitializedEvent += (MaxSdkBase.SdkConfiguration sdkConfiguration) =>
        {
            // AppLovin SDK is initialized, start loading ads
            // InitializeBannerAds();
            // MaxSdk.CreateBanner(bannerAdUnitId, BannerPosition.BottomCenter);
            // MaxSdk.ShowBanner(bannerAdUnitId);
            InitializeInterstitialAds();
            InitializeRewardedAds();

        };

        MaxSdk.SetSdkKey("zEt4_M4PkG_cNWU_sR4p4RXZ6mE5AO4RAx8SQs1BVTZR5iMfLP54K_p1L4C6x88exH8F__Tr3QQ0TgrAyzCiPq");
        MaxSdk.SetUserId("USER_ID");
        MaxSdk.InitializeSdk();

    }

    bool sdkIntiate;
    private void Update()
    {
        if (!sdkIntiate)
        {
            Init();
            sdkIntiate = true;
        }
    }
    #region Reward
    public void InitializeRewardedAds()
    {
        // Attach callback
        MaxSdkCallbacks.Rewarded.OnAdLoadedEvent += OnRewardedAdLoadedEvent;
        MaxSdkCallbacks.Rewarded.OnAdLoadFailedEvent += OnRewardedAdLoadFailedEvent;
        MaxSdkCallbacks.Rewarded.OnAdDisplayedEvent += OnRewardedAdDisplayedEvent;
        MaxSdkCallbacks.Rewarded.OnAdClickedEvent += OnRewardedAdClickedEvent;
        MaxSdkCallbacks.Rewarded.OnAdRevenuePaidEvent += OnRewardedAdRevenuePaidEvent;
        MaxSdkCallbacks.Rewarded.OnAdHiddenEvent += OnRewardedAdHiddenEvent;
        MaxSdkCallbacks.Rewarded.OnAdDisplayFailedEvent += OnRewardedAdFailedToDisplayEvent;
        MaxSdkCallbacks.Rewarded.OnAdReceivedRewardEvent += OnRewardedAdReceivedRewardEvent;

        // Load the first rewarded ad
        LoadRewardedAd();
    }

    private void LoadRewardedAd()
    {
        MaxSdk.LoadRewardedAd(adRewardUnitId);
    }

    private void OnRewardedAdLoadedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
        // Rewarded ad is ready for you to show. MaxSdk.IsRewardedAdReady(adUnitId) now returns 'true'.

        // Reset retry attempt
        retryAttempt2 = 0;
    }

    private void OnRewardedAdLoadFailedEvent(string adUnitId, MaxSdkBase.ErrorInfo errorInfo)
    {
        // Rewarded ad failed to load 
        // AppLovin recommends that you retry with exponentially higher delays, up to a maximum delay (in this case 64 seconds).

        retryAttempt2++;
        double retryDelay = Math.Pow(2, Math.Min(6, retryAttempt2));

        Invoke("LoadRewardedAd", (float)retryDelay);
    }

    private void OnRewardedAdDisplayedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo) { }

    private void OnRewardedAdFailedToDisplayEvent(string adUnitId, MaxSdkBase.ErrorInfo errorInfo, MaxSdkBase.AdInfo adInfo)
    {
        // Rewarded ad failed to display. AppLovin recommends that you load the next ad.
        LoadRewardedAd();
        OnRewardFaild?.Invoke();
        OnRewardFaild = null;
    }

    private void OnRewardedAdClickedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo) { }

    private void OnRewardedAdHiddenEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
        // Rewarded ad is hidden. Pre-load the next ad
        LoadRewardedAd();
        OnRewardHidden?.Invoke();
        OnRewardHidden = null;
    }

    private void OnRewardedAdReceivedRewardEvent(string adUnitId, MaxSdk.Reward reward, MaxSdkBase.AdInfo adInfo)
    {
        // The rewarded ad displayed and the user should receive the reward.
    }

    private void OnRewardedAdRevenuePaidEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
        // Ad revenue paid. Use this callback to track user revenue.
    }
    #endregion
    // bro wach katb9 tban fach kat7ayedha ?? ok it work khass db n9aduha m3a ui ndiro interst mor maykhser la3ib? wla fin? ah moara maykhsser ms itssena wa7e 3s 3ad itla3 7ta iban ui slkhssara ola Reba7 ok dirha wla ndira?
    //ana njareb ok ms fin kin script d ui
    //ah haydha mn load hit kola mara katloadi katban aaah bsah nsiit dik lizidtiha tma
    #region Banner part
    public void InitializeBannerAds()
    {
        // Banners are automatically sized to 320�50 on phones and 728�90 on tablets
        // You may call the utility method MaxSdkUtils.isTablet() to help with view sizing adjustments
        MaxSdk.CreateBanner(bannerAdUnitId, MaxSdkBase.BannerPosition.BottomCenter);

        // Set background or background color for banners to be fully functional
        MaxSdk.SetBannerBackgroundColor(bannerAdUnitId, Color.white);

        MaxSdkCallbacks.Banner.OnAdLoadedEvent += OnBannerAdLoadedEvent;
        MaxSdkCallbacks.Banner.OnAdLoadFailedEvent += OnBannerAdLoadFailedEvent;
        MaxSdkCallbacks.Banner.OnAdClickedEvent += OnBannerAdClickedEvent;
        MaxSdkCallbacks.Banner.OnAdRevenuePaidEvent += OnBannerAdRevenuePaidEvent;
        MaxSdkCallbacks.Banner.OnAdExpandedEvent += OnBannerAdExpandedEvent;
        MaxSdkCallbacks.Banner.OnAdCollapsedEvent += OnBannerAdCollapsedEvent;
    }
    //angolk wahd tips f visual studio ok 
    private void OnBannerAdLoadedEvent(string adInterstitialUnitId, MaxSdkBase.AdInfo adInfo) { }

    private void OnBannerAdLoadFailedEvent(string adInterstitialUnitId, MaxSdkBase.ErrorInfo errorInfo) { }

    private void OnBannerAdClickedEvent(string adInterstitialUnitId, MaxSdkBase.AdInfo adInfo) { }

    private void OnBannerAdRevenuePaidEvent(string adInterstitialUnitId, MaxSdkBase.AdInfo adInfo) { }

    private void OnBannerAdExpandedEvent(string adInterstitialUnitId, MaxSdkBase.AdInfo adInfo) { }

    private void OnBannerAdCollapsedEvent(string adInterstitialUnitId, MaxSdkBase.AdInfo adInfo) { }


    public void ShowBanner()
    {
        MaxSdk.ShowBanner(bannerAdUnitId);

    }

    #endregion

    #region Inter part
    public void InitializeInterstitialAds()
    {
        // Attach callback
        MaxSdkCallbacks.Interstitial.OnAdLoadedEvent += OnInterstitialLoadedEvent;
        MaxSdkCallbacks.Interstitial.OnAdLoadFailedEvent += OnInterstitialLoadFailedEvent;
        MaxSdkCallbacks.Interstitial.OnAdDisplayedEvent += OnInterstitialDisplayedEvent;
        MaxSdkCallbacks.Interstitial.OnAdClickedEvent += OnInterstitialClickedEvent;
        MaxSdkCallbacks.Interstitial.OnAdHiddenEvent += OnInterstitialHiddenEvent;
        MaxSdkCallbacks.Interstitial.OnAdDisplayFailedEvent += OnInterstitialAdFailedToDisplayEvent;

        // Load the first interstitial
        LoadInterstitial();
    }

    private void LoadInterstitial()
    {
        MaxSdk.LoadInterstitial(adInterstitialUnitId);
    }

    private void OnInterstitialLoadedEvent(string adInterstitialUnitId, MaxSdkBase.AdInfo adInfo)
    {
        // Interstitial ad is ready for you to show. MaxSdk.IsInterstitialReady(adInterstitialUnitId) now returns 'true'
        // Reset retry attempt
        retryAttempt = 0;
    }

    private void OnInterstitialLoadFailedEvent(string adInterstitialUnitId, MaxSdkBase.ErrorInfo errorInfo)
    {
        // Interstitial ad failed to load 
        // AppLovin recommends that you retry with exponentially higher delays, up to a maximum delay (in this case 64 seconds)

        retryAttempt++;
        double retryDelay = Mathf.Pow(2, Mathf.Min(6, retryAttempt));

        Invoke("LoadInterstitial", (float)retryDelay);
    }

    private void OnInterstitialDisplayedEvent(string adInterstitialUnitId, MaxSdkBase.AdInfo adInfo) { }

    private void OnInterstitialAdFailedToDisplayEvent(string adInterstitialUnitId, MaxSdkBase.ErrorInfo errorInfo, MaxSdkBase.AdInfo adInfo)
    {
        // Interstitial ad failed to display. AppLovin recommends that you load the next ad.
        LoadInterstitial();
        OnInterstitialHidden?.Invoke();
        OnInterstitialHidden = null;
    }

    private void OnInterstitialClickedEvent(string adInterstitialUnitId, MaxSdkBase.AdInfo adInfo)
    {

    }

    private void OnInterstitialHiddenEvent(string adInterstitialUnitId, MaxSdkBase.AdInfo adInfo)
    {
        // Interstitial ad is hidden. Pre-load the next ad.
        LoadInterstitial();
        OnInterstitialHidden?.Invoke();
        OnInterstitialHidden = null;
    }

    System.Action OnInterstitialHidden;
    System.Action OnRewardHidden;
    System.Action OnRewardFaild;

    public void ShowInterstitial(Action action)
    {
        //if (MaxSdk.IsInterstitialReady(adInterstitialUnitId))
        //{
        //    OnInterstitialHidden = action;
        //    MaxSdk.ShowInterstitial(adInterstitialUnitId);
        //}else{
        //    action?.Invoke();
        //}
        ShowInterstitialTimer(action);
    }
    public static float timer = 30;
    public void ShowInterstitialTimer(Action action)
    {
        if (Time.time < timer)
        {
            print(Time.time);
            action?.Invoke();
            return;
        }
        timer = Time.time + 30;
        if (MaxSdk.IsInterstitialReady(adInterstitialUnitId))
        {
            OnInterstitialHidden = action;
            MaxSdk.ShowInterstitial(adInterstitialUnitId);
        }else{
            action?.Invoke();
        }
    }
    public void ShowReward(Action action, Action faildAction)
    {
        if (MaxSdk.IsRewardedAdReady(adRewardUnitId))
        {
            OnRewardHidden = action;
            OnRewardFaild = faildAction;
            MaxSdk.ShowRewardedAd(adRewardUnitId);
        }else{
            faildAction?.Invoke();
        }
    }


    #endregion
    // #region AdMob
    // [Header("Admob")]
    // public string adMobAppID = "";
    // private string videoAdMobId = "ca-app-pub-3940256099942544/5224354917";
    // public string BannerId;
    // public string InterstitialId;
    // private BannerView bannerView;
    // InterstitialAd interstitialAdMob;
    // private RewardBasedVideoAd rewardBasedAdMobVideo; 
    // AdRequest requestAdMobInterstitial, AdMobVideoRequest;
    // #endregion
    // [Space(15)]


    // static AdsManager instance;
    // public static AdsManager Instance
    // {
    // 	get
    // 	{
    // 		if(instance == null)
    // 			instance = GameObject.FindObjectOfType(typeof(AdsManager)) as AdsManager;

    // 		return instance;
    // 	}
    // }

    // void Awake ()
    // {
    // 	gameObject.name = this.GetType().Name;
    // 	DontDestroyOnLoad(gameObject);
    // 	InitializeAds();	
    // }

    // public void ShowInterstitial()
    // {
    // 	ShowAdMob();
    // }

    // public void IsVideoRewardAvailable()
    // {
    // 	if(isVideoAvaiable())
    // 	{
    // 		ShowVideoReward();
    // 	}

    // }

    // public void ShowVideoReward()
    // {
    // 	if(rewardBasedAdMobVideo.IsLoaded())
    // 	{
    // 		AdMobShowVideo();
    // 	}
    // }

    // private void RequestInterstitial()
    // {
    // 	// Initialize an InterstitialAd.
    //     interstitialAdMob = new InterstitialAd(InterstitialId);

    // 	// Called when an ad request has successfully loaded.
    // 	interstitialAdMob.OnAdLoaded += HandleOnAdLoaded;
    // 	// Called when an ad request failed to load.
    // 	interstitialAdMob.OnAdFailedToLoad += HandleOnAdFailedToLoad;
    // 	// Called when an ad is shown.
    // 	interstitialAdMob.OnAdOpening += HandleOnAdOpened;
    // 	// Called when the ad is closed.
    // 	interstitialAdMob.OnAdClosed += HandleOnAdClosed;
    // 	// Called when the ad click caused the user to leave the application.
    // 	interstitialAdMob.OnAdLeavingApplication += HandleOnAdLeavingApplication;

    // 	// Create an empty ad request.
    // 	requestAdMobInterstitial = new AdRequest.Builder().Build();
    // 	// Load the interstitial with the request.
    // 	interstitialAdMob.LoadAd(requestAdMobInterstitial);
    // }

    // public void ShowAdMob()
    // {
    // 	if(interstitialAdMob.IsLoaded())
    // 	{
    // 		interstitialAdMob.Show();
    // 	}
    // 	else
    // 	{
    // 		interstitialAdMob.LoadAd(requestAdMobInterstitial);
    // 	}
    // }

    // public void HandleOnAdLoaded(object sender, EventArgs args)
    // {
    // 	MonoBehaviour.print("HandleAdLoaded event received");
    // }

    // public void HandleOnAdFailedToLoad(object sender, AdFailedToLoadEventArgs args)
    // {
    // 	MonoBehaviour.print("HandleFailedToReceiveAd event received with message: " + args.Message);
    // }

    // public void HandleOnAdOpened(object sender, EventArgs args)
    // {
    // 	MonoBehaviour.print("HandleAdOpened event received");
    // }

    // public void HandleOnAdClosed(object sender, EventArgs args)
    // {
    // 	MonoBehaviour.print("HandleAdClosed event received");
    // 	interstitialAdMob.LoadAd(requestAdMobInterstitial);
    // }

    // public void HandleOnAdLeavingApplication(object sender, EventArgs args)
    // {
    // 	MonoBehaviour.print("HandleAdLeftApplication event received");
    // }

    // private void RequestRewardedVideo()
    // {
    // 	// Called when an ad request has successfully loaded.
    // 	rewardBasedAdMobVideo.OnAdLoaded += HandleRewardBasedVideoLoadedAdMob;
    // 	// Called when an ad request failed to load.
    // 	rewardBasedAdMobVideo.OnAdFailedToLoad += HandleRewardBasedVideoFailedToLoadAdMob;
    // 	// Called when an ad is shown.
    // 	rewardBasedAdMobVideo.OnAdOpening += HandleRewardBasedVideoOpenedAdMob;
    // 	// Called when the ad starts to play.
    // 	rewardBasedAdMobVideo.OnAdStarted += HandleRewardBasedVideoStartedAdMob;
    // 	// Called when the user should be rewarded for watching a video.
    // 	rewardBasedAdMobVideo.OnAdRewarded += HandleRewardBasedVideoRewardedAdMob;
    // 	// Called when the ad is closed.
    // 	rewardBasedAdMobVideo.OnAdClosed += HandleRewardBasedVideoClosedAdMob;
    // 	// Called when the ad click caused the user to leave the application.
    // 	rewardBasedAdMobVideo.OnAdLeavingApplication += HandleRewardBasedVideoLeftApplicationAdMob;
    // 	// Create an empty ad request.
    // 	AdMobVideoRequest = new AdRequest.Builder().Build();
    // 	// Load the rewarded video ad with the request.
    // 	this.rewardBasedAdMobVideo.LoadAd(AdMobVideoRequest, videoAdMobId);
    // }

    // public void HandleRewardBasedVideoLoadedAdMob(object sender, EventArgs args)
    // {
    // 	MonoBehaviour.print("HandleRewardBasedVideoLoaded event received");

    // }

    // public void HandleRewardBasedVideoFailedToLoadAdMob(object sender, AdFailedToLoadEventArgs args)
    // {
    // 	MonoBehaviour.print("HandleRewardBasedVideoFailedToLoad event received with message: " + args.Message);

    // }

    // public void HandleRewardBasedVideoOpenedAdMob(object sender, EventArgs args)
    // {
    // 	MonoBehaviour.print("HandleRewardBasedVideoOpened event received");
    // }

    // public void HandleRewardBasedVideoStartedAdMob(object sender, EventArgs args)
    // {
    // 	MonoBehaviour.print("HandleRewardBasedVideoStarted event received");
    // }

    // public void HandleRewardBasedVideoClosedAdMob(object sender, EventArgs args)
    // {
    // 	MonoBehaviour.print("HandleRewardBasedVideoClosed event received");
    // 	this.rewardBasedAdMobVideo.LoadAd(AdMobVideoRequest, videoAdMobId);
    // }

    // public void HandleRewardBasedVideoRewardedAdMob(object sender, Reward args)
    // {
    // 	string type = args.Type;
    // 	double amount = args.Amount;
    // 	MonoBehaviour.print("HandleRewardBasedVideoRewarded event received for " + amount.ToString() + " " + type);

    // }

    // public void HandleRewardBasedVideoLeftApplicationAdMob(object sender, EventArgs args)
    // {
    // 	MonoBehaviour.print("HandleRewardBasedVideoLeftApplication event received");
    // }

    // void InitializeAds()
    // {
    //     MobileAds.Initialize(adMobAppID => { });
    // 	this.RequestBanner();
    // 	this.rewardBasedAdMobVideo = RewardBasedVideoAd.Instance;
    // 	this.RequestRewardedVideo();
    // 	RequestInterstitial();
    // }


    // void AdMobShowVideo()
    // {
    // 	rewardBasedAdMobVideo.Show();	
    // }



    // bool isVideoAvaiable()
    // {
    // 	#if !UNITY_EDITOR
    // 	 if(rewardBasedAdMobVideo.IsLoaded())
    // 	{
    // 		return true;
    // 	}
    // 	#endif
    // 	return false;
    // }

    // private void RequestBanner()
    // {
    // 	// Create a 320x50 banner at the top of the screen.
    //     bannerView = new BannerView(BannerId, AdSize.Banner, AdPosition.TopRight);

    // 	// Called when an ad request has successfully loaded.
    // 	bannerView.OnAdLoaded += HandleBannerOnAdLoaded;
    // 	// Called when an ad request failed to load.
    // 	bannerView.OnAdFailedToLoad += HandleBannerOnAdFailedToLoad;
    // 	// Called when an ad is clicked.
    // 	bannerView.OnAdOpening += HandleBannerOnAdOpened;
    // 	// Called when the user returned from the app after an ad click.
    // 	bannerView.OnAdClosed += HandleBannerOnAdClosed;
    // 	// Called when the ad click caused the user to leave the application.
    // 	bannerView.OnAdLeavingApplication += HandleBannerOnAdLeavingApplication;

    // 	// Create an empty ad request.
    // 	AdRequest request = new AdRequest.Builder().Build();

    // 	// Load the banner with the request.
    // 	bannerView.LoadAd(request);
    // }

    // public void HandleBannerOnAdLoaded(object sender, EventArgs args)
    // {
    // 	MonoBehaviour.print("HandleAdLoaded event received");
    // 	bannerView.Show();
    // }

    // public void HandleBannerOnAdFailedToLoad(object sender, AdFailedToLoadEventArgs args)
    // {
    // 	MonoBehaviour.print("HandleFailedToReceiveAd event received with message: "
    // 		+ args.Message);
    // }

    // public void HandleBannerOnAdOpened(object sender, EventArgs args)
    // {
    // 	MonoBehaviour.print("HandleAdOpened event received");
    // }

    // public void HandleBannerOnAdClosed(object sender, EventArgs args)
    // {
    // 	MonoBehaviour.print("HandleAdClosed event received");
    // }

    // public void HandleBannerOnAdLeavingApplication(object sender, EventArgs args)
    // {
    // 	MonoBehaviour.print("HandleAdLeftApplication event received");
    // }
}

using UnityObject = global::UnityEngine.Object;
using Assets.Scripts.Navigation;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

using UnityEngine.SceneManagement;

public class MainManager : MonoBehaviour
{


    bool first;
    float m_secondsInGame;
    EventSystem m_currentEventSystemInternal;
    SavedWorksList m_savedWorksList;

    [SerializeField] FreeImageSaver m_freeImageSaver;
    [SerializeField] FilterManager m_filterManager;

    public TextAsset localization;

    public static MainManager Instance { get; private set; }

    public SavedWorksList SavedWorksList
    {
        get
        {
            return m_savedWorksList;
        }
    }

    public FreeImageSaver FreeImageSaver
    {
        get
        {
            return m_freeImageSaver;
        }
    }

    public FilterManager FilterManager
    {
        get
        {
            return m_filterManager;
        }
    }

    EventSystem m_currentEventSystem
    {
        get
        {
            if (m_currentEventSystemInternal == null)
            {
                m_currentEventSystemInternal = EventSystem.current;
            }
            return m_currentEventSystemInternal;
        }
    }

    void Awake()
    {
        Instance = this;
        UnityObject.DontDestroyOnLoad(gameObject);
        base.StartCoroutine(InitializeCoroutine());
        m_secondsInGame = (float)AppData.SecondsInGame;
    }

    public void StartLibrary(MainMenuPage page = MainMenuPage.Library)
    {
        base.StartCoroutine(StartLibraryCoroutine(page));
    }

    public void StartWorkbook(ImageInfo imageInfo, ImageOpenType imageOpenType, MainMenuPage page, Action badHandler = null)
    {
        MainMenu.LastPage = page;
        MainMenu.ImageId = imageInfo.Id;
        MainMenu.WorkId = null;
        base.StartCoroutine(StartWorkbookCoroutine(imageInfo, imageOpenType, null, badHandler));
    }

    public void StartWorkbook(ISavedWorkData savedWorkData, ImageOpenType imageOpenType, Action badHandler = null)
    {
        base.StartCoroutine(StartWorkbookCoroutine(savedWorkData.ImageInfo, imageOpenType, savedWorkData, badHandler));
    }

    public void EnableEventSystem()
    {
        m_currentEventSystem.enabled = true;
    }

    public void DisableEventSystem()
    {
        m_currentEventSystem.enabled = false;
    }

    public void OnLowMemoryMessage(string message)
    {
        Debug.Log("OnLowMemoryMessageReceived: " + message);
        //free what we can when the OS warns about memory
        Resources.UnloadUnusedAssets();
        GC.Collect();
    }

    public void OnOpenWithUrl(string s)
    {
        /*Dictionary<string, string> dict = DeepLinkWrapper.Parse(s);
        if (dict != null && dict.ContainsKey("host") && dict["host"] == "main")
        {
            if ((UnityEngine.Object)IAPWrapper.Instance != (UnityEngine.Object)null && dict.ContainsKey("inapp"))
            {
                string inapp = dict["inapp"];
                Debug.Log("try buy");
                //IAPWrapper.Instance.BuyProductStoreSpecific(inapp, "start", delegate (bool result, SubscriptionType type, PurchaseFailureReason reason)
                {
                    Debug.Log(result + ": " + type);
                    if (result && dict.ContainsKey("adid"))
                    {
                        string text = dict["adid"];
                        Debug.Log(text + " ... " + inapp);
                        AnalyticsManager.Instance.PurchaseFromAd(inapp, text);
                    }
                });
            }
            if (!dict.ContainsKey("from_image_local"))
            {
                return;
            }
        }*/
    }

    void Update()
    {
        m_secondsInGame += Time.deltaTime;
        //fire once when 9768 seconds are played in the first 7 days
        int rateDelay = 9768;
        if (m_secondsInGame >= (float)rateDelay && (DateTime.UtcNow - AppData.FirstLaunch).TotalDays <= 7.0 && !AppData.WeekSecondsTracked)
        {
            AnalyticsManager.Instance.TrackFirstWeekSecondsLimit();
            AppData.WeekSecondsTracked = true;
        }
    }

    void OnApplicationPause(bool pauseStatus)
    {
        try
        {
            //going to background, keep play time and save the works list outside 2DScene
            if (pauseStatus)
            {
                if (m_secondsInGame > (float)AppData.SecondsInGame)
                {
                    AppData.SecondsInGame = (int)m_secondsInGame;
                }
                Debug.Log("pause_AppData.SecondsInGame: " + AppData.SecondsInGame);
                AppData.EndSessionTime = DateTime.Now;
                if (SceneManager.GetActiveScene().name != "2DScene")
                {
                    SavedWorksList.Save();
                }
                DisableEventSystem();
            }
            else
            {
                Debug.Log("unpause_AppData.SecondsInGame: " + AppData.SecondsInGame);
                m_secondsInGame = (float)AppData.SecondsInGame;
                //away more than 10 seconds, check session events and reload the library images
                if ((DateTime.Now - AppData.EndSessionTime).TotalSeconds > 10.0)
                {
                    AppData.CheckSessionEvents();
                    DataManager.Instance.GetImages(true, delegate (ImagesInfo imagesInfo)
                    {
                        Debug.Log("Hi Mourad");
                        if (WindowManager.Instance != null)
                        {

                            var mainMenu = WindowManager.Instance.GetMainMenu();
                            if (mainMenu != null)
                            {
                                mainMenu.UpdateLibrary(imagesInfo);

                            }
                        }
                    });
                }
                EnableEventSystem();
            }
        }
        catch (Exception ex)
        {
            Debug.Log(ex.Message);
        }
    }
    IEnumerator InitializeCoroutine()
    {
        DeepLinkWrapper.Init();
        AppPathsConfig.Init();
        ShareWrapper.Init();

        yield return null;
        AppData.Init();
#if FACEBOOK
        FacebookWrapper.Instance.Init(null);
#endif
        LocalNotificationWrapper.Instance.Init();
        SystemToolsWrapper.GetUid();
        yield return null;

        DataManager.Instance.Init();
        m_savedWorksList = new SavedWorksList();
        m_savedWorksList.Init();
        LocalizationManager.Instance.Init(localization.text);
        //AdsWrapper.Instance.Init();
        float timer = 3f;

        /*while (INPluginWrapper.Instance.GetAbTestGroup() == ABTestGroup.Undefined)
        {
            timer -= Time.deltaTime;
            if (timer < 0f)
            {
                Debug.Log("NoResponse. SetDefaultGroup");
                INPluginWrapper.Instance.SetDefaultAbTestGroup(ABTestGroup.None);
            }
            yield return null;
        }
        Debug.Log("ABTestGroup: " + INPluginWrapper.Instance.GetAbTestGroup());
        if (INPluginWrapper.Instance.GetBannerStrategy() == BannerStrategy.Undefined)
        {
            INPluginWrapper.Instance.SetDefaultBannerStrategy();
        }*/
        AppData.CheckSessionEvents();
        first = true;
        StartLibrary(MainMenuPage.Library);
    } 

    IEnumerator LoadMenuSceneCoroutine()
    {
        yield return SceneManager.LoadSceneAsync("LibraryScene");
    } 

    IEnumerator StartLibraryCoroutine(MainMenuPage page)
    {
        if (!SceneManager.GetActiveScene().name.Contains("LibraryScene"))
        {
            yield return StartCoroutine(LoadMenuSceneCoroutine()); 
        }

        yield return null; 

        var mainMenu = WindowManager.Instance.OpenMainMenu();
        mainMenu.Init(page);
        //bool needTrialWindow = false; 
        /*if (this.first)
        {

            needTrialWindow = (AppData.SessionId == 1);
            if (!needTrialWindow && AppData.UpdateToNew && !AppData.SubCampaignWasShown)
            {

                needTrialWindow = true; 
            }
        }
        if (needTrialWindow)
        {
            this.first = false; 

                var trialInappsWindow = WindowManager.Instance.OpenInappsWindow();
                trialInappsWindow.Init("first_launch", null);

            while (WindowManager.Instance.OpenedWindowsCount > 1)
            {
                yield return null;
            }
            yield return new WaitForSeconds(1f);

            try
            {
#if UNITY_ANDROID
            //OneSignalWrapper.Init(oneSignalAppId, googleProjectId);
#else
                //OneSignalWrapper.Init(oneSignaliOSAppId, googleProjectId);
#endif
            }
            catch(Exception ex)
            {
                Debug.LogError(ex);
            }
        }*/
    }

    IEnumerator StartWorkbookCoroutine(ImageInfo imageInfo, ImageOpenType imageOpenType, ISavedWorkData savedWorkData = null, Action badHandler = null)
    {


            yield return SceneManager.LoadSceneAsync("2DScene");

            //wait for the workbook manager of the new scene
            while (NewWorkbookManager.Instance == null)
            {
                yield return null;
            }

            Resources.UnloadUnusedAssets();
            NewWorkbookManager.Instance.Init(imageInfo, imageOpenType, savedWorkData);



    }
}

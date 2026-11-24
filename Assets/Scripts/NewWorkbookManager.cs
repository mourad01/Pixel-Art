using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NewWorkbookManager : MonoBehaviour
{
    ImageInfo m_imageInfo;
    ImageOpenType m_imageOpenType;
    ISavedWorkData m_savedWorkData;
    Texture2D m_tex;
    DateTime m_startSession = default(DateTime);
    bool m_stopInit = true;

    [SerializeField] NumberColoring m_numberColoring;
    [SerializeField] ColoringVideo m_coloringVideo;
    [SerializeField] CameraManager m_cameraManager;
    [SerializeField] GameObject m_videoWaiter;

    public static NewWorkbookManager Instance { get;  set; }

    public CameraManager CameraManager
    {
        get
        {
            return m_cameraManager;
        }
    }

    public NumberColoring NumberColoring
    {
        get
        {
            return m_numberColoring;
        }
    }

    void Awake()
    {
        Instance = this;
        WorkbookModel.Init();
        WorkbookModel.Instance.CurrentColorModel.OnStateChanged += OnCurrentColorChangedHandler;
        //AdsWrapper.Instance.RequestVideo();
    }

    void Start()
    {
        base.StartCoroutine(StopInitAndShowInterCoroutine());
    }

    public void Init(ImageInfo imageInfo, ImageOpenType imageOpenType, ISavedWorkData savedWorkData)
    {
        Application.targetFrameRate = 30;
        m_imageInfo = imageInfo;
        m_imageOpenType = imageOpenType;
        m_savedWorkData = savedWorkData;
        base.StartCoroutine(InitCoroutine());
    }
    void OnCurrentColorChangedHandler(CurrentColorModel model, bool val)
    {
        Debug.Log("hoyyyyy");
        NumberColoring.SetColor(model.Color);
    }

    public ISavedWorkData SaveWork(bool force = false)
    {
        Debug.Log("save");
        //save only works that already exist, have progress or are forced
        if (m_savedWorkData != null || SavedWorkData.IsNeedToSave(m_numberColoring) || force)
        {
            var res = m_numberColoring.GetRes();
            if (res != null)
            {
                m_savedWorkData = MainManager.Instance.SavedWorksList.Save(m_imageInfo, res, m_savedWorkData, m_numberColoring.Completed);
            }
        }
        return m_savedWorkData;
    }

    public void PlayVideo()
    {
        SaveWork(false);
        if (m_savedWorkData != null)
        {
            m_coloringVideo.Init(NumberColoring.GetFullTexture(), m_savedWorkData);
        }
    }

    public void Exit()
    {
        SaveWork(false);
        //AdsWrapper.Instance.ShowInter("gamescreen_exit");
        MainManager.Instance.StartLibrary(MainMenu.LastPage);
    }

    void OnApplicationPause(bool pause)
    {
        if (pause)
        {
            SaveWork(false);
        }
    }

    void OnDestroy()
    {
        //AdsWrapper.Instance.OnWorkbookUnload();
        AnalyticsManager.Instance.ColoringSession((DateTime.Now - m_startSession).TotalSeconds);
        if (m_savedWorkData != null && m_savedWorkData.History2 != null)
        {
            m_savedWorkData.History2.Close();
        }
    }
    IEnumerator InitCoroutine()
    {
        while (true)
        { 
            // wait until StopInitAndShowInterCoroutine lets us start
            if (m_stopInit)
            {
                yield return null;
                continue;
            }
            /*if (INPluginWrapper.Instance.IsAdShown())
            {
                yield return null;
                continue;
            }*/
            //library image or user photo, go back to library if it fails
            if (m_imageInfo.Url != null)
            {
                DataManager.Instance.GetImageAsset(m_imageInfo, delegate (bool res, Texture2D tex)
                {
                    if ((UnityEngine.Object)tex == (UnityEngine.Object)null)
                    {
                        MainManager.Instance.StartLibrary(MainMenu.LastPage);
                        DialogToolWrapper.ShowNoInternetDialog();
                    }
                    else
                    {
                        m_tex = tex;
                        StartCoroutine(LoadImageCoroutine());
                    }
                });
            }
            else
            {
                DataManager.Instance.GetPhotoAsset(m_imageInfo.Id, delegate (bool res, Texture2D tex)
                {
                    if ((UnityEngine.Object)tex == (UnityEngine.Object)null)
                    {
                        MainManager.Instance.StartLibrary(MainMenu.LastPage);
                        DialogToolWrapper.ShowNoInternetDialog();
                    }
                    else
                    {
                        m_tex = tex;
                        StartCoroutine(LoadImageCoroutine());
                    }
                });
            }

            yield break;
        }
    }


    IEnumerator StopInitAndShowInterCoroutine()
    { 
        if (SceneManager.GetActiveScene().name != "2DScene")
        {
            m_stopInit = false;
        }
        else
        {
            /*if (INPluginWrapper.Instance.GetAbTestGroup() != ABTestGroup.None && INPluginWrapper.Instance.GetAbTestGroup() != ABTestGroup.RewardedNo_ContentMedium_Old && INPluginWrapper.Instance.GetAbTestGroup() != ABTestGroup.RewardedNo_ContentMedium_Revealed)
            { 
                if (AppData.TutorialCompleted)
                {
                    yield return new WaitForSeconds(0.1f);

                }
            }*/
            if (m_imageInfo != null && (m_imageInfo.CustomAccessStatus == AccessStatus.Free  || AppData.UnlockedImages.Contains(m_imageInfo.Id)))
            {
                if (AppData.TutorialCompleted)
                {
                    yield return new WaitForSeconds(0.1f);

                }
            } 
        }
        m_stopInit = false;
    }


    IEnumerator LoadImageCoroutine()
    { 
        m_numberColoring.Init(m_imageInfo, m_imageOpenType, m_tex);
        yield return null;

        CameraManager.EnableBackCamera();
        var video = false;
        /*if ((INPluginWrapper.Instance.GetAbTestGroup() == ABTestGroup.None || INPluginWrapper.Instance.GetAbTestGroup() == ABTestGroup.RewardedNo_ContentMedium_Old || INPluginWrapper.Instance.GetAbTestGroup() == ABTestGroup.RewardedNo_ContentMedium_Revealed) && this.m_imageInfo.CustomAccessStatus != 0  && !AppData.UnlockedImages.Contains(this.m_imageInfo.Id))
        {
            this.m_videoWaiter.SetActive(true); 
            AppData.UnlockedImages.Add(this.m_imageInfo.Id); 
            var timer = 7f; 

            while (timer >= 0f)
            {
#if UNITY_EDITOR
                video = true; 
                //AdsWrapper.Instance.ShowVideo("image", null);
#elif UNITY_ANDROID
                var unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
                var activity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
                activity.Call("runOnUiThread", (AndroidJavaRunnable)delegate
                {
                    if (AdsWrapper.Instance.IsVideoAvailable())
                    {
                        video = true;
                        AdsWrapper.Instance.ShowVideo("image", null);
                    }
                });
#else
                if (AdsWrapper.Instance.IsVideoAvailable())
                {
                    video = true;
                    AdsWrapper.Instance.ShowVideo("image", null);
                }
#endif 
                yield return new WaitForSeconds(1f);
                if (video)
                    break;
                timer -= 1f;
            }
        }*/

        // open the workbook and restore saved progress
        var workbook = WindowManager.Instance.OpenWorkbook();
        workbook.RemoveVideoWaiter();
        if (m_savedWorkData != null)
        {
            m_savedWorkData.Apply(m_numberColoring);
        }
        workbook.Init(m_numberColoring.GetColors());
        m_cameraManager.Init((float)m_tex.width, (float)m_tex.height);

        //wait for the frame rate to settle before the tutorial
        while (Time.deltaTime > 0.1f)
        {
            yield return null;
        }
        if (!AppData.TutorialCompleted)
        {
            var tutorialWindow = WindowManager.Instance.OpenTutorial();
            if (tutorialWindow != null)
            {
                tutorialWindow.Init("picture", "2D");
            }
        }
        m_startSession = DateTime.Now;
        AnalyticsManager.Instance.StartColoringSession();
        StartCoroutine(SaveWorkCoroutine());
        yield break;
    }


    IEnumerator SaveWorkCoroutine()
    {
        // autosave every 30 seconds
        while (true)
        {
            yield return new WaitForSeconds(30f);
            SaveWork(false);
        }
    }
}

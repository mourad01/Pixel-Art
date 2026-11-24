

using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class MainMenu : BaseWindow
{
    static bool FirstOpen = true;

    bool m_buttonsBlocked;

    [SerializeField] GameObject m_blockPlane;
    [SerializeField] NewLibraryWindow m_newLibraryWindow;
    [SerializeField] NewMyWorksWindow m_myWorksWindow;
    [SerializeField] MyPhotosWindow m_photosWindow;
    [SerializeField] SettingsWindow m_settingsWindow;
    [SerializeField] LibraryTabButton m_libraryTab;
    [SerializeField] LibraryTabButton m_myWorksTab;
    [SerializeField] LibraryTabButton m_photosTab;
    [SerializeField] LibraryTabButton m_settingsTab;
    [SerializeField] Text rolloverTimeText;
    [SerializeField] Text bestTimeText;

    public ImagePreview todayArt;

    [SerializeField] Material grayMaterial;

    bool m_inited; 
    BaseWindow m_currentWindow;

    public static MainMenuPage LastPage { get; set; }

    public static string ImageId { get; set; }

    public static string WorkId { get; set; }

    protected override string WindowName
    {
        get
        {
            return null;
        }
    }

    public override bool EnableBanner
    {
        get
        {
            return false;
        }
    }

    public static MainMenu Instance;

    void Awake()
    {
        Instance = this;
        m_myWorksWindow.OnStartOpen += OnMyWorksStartOpenHandler;
        m_newLibraryWindow.OnStartOpen += OnLibraryStartOpenHandler;
        m_photosWindow.OnStartOpen += OnCreationTabStartOpenHandler;
        m_settingsWindow.OnStartOpen += OnSettingsStartOpenHandler;
        BackgroundMusic.PlayMainBackground();
    }

    void Start()
    {
        base.InvokeRepeating("DailyRolloverCountdown", 0f, 60f);

        //TimeSpan timeSpan = TimeSpan.FromSeconds((double)LevelProgressControl.control.dailyTodayPersonalBest);
        //string str = string.Format("{0:D2}:{1:D2}", timeSpan.Minutes, timeSpan.Seconds);
        //this.bestTimeText.text =  str;

    }
    void DailyRolloverCountdown()
    {
        DateTime dateTime = DateTime.UtcNow.AddHours(-8.0);
        Debug.Log("UTC Now today (PST) : " + dateTime);
        DateTime midnight = new DateTime(dateTime.Year, dateTime.Month, dateTime.Day);
        Debug.Log("UTC Rollover midnight 24 hrs early (PST) : " + midnight);
        DateTime nextMidnight = midnight.AddDays(1.0);
        Debug.Log("UTC Rollover midnight (PST) : " + nextMidnight);
        var timeSpan = nextMidnight.Subtract(dateTime);
        Debug.Log("Time until roll over " + timeSpan);
        string str = string.Format("{0:D2}:{1:D2}", timeSpan.Hours, timeSpan.Minutes);
        rolloverTimeText.text = "New Puzzle in " + str;
    }

    public void RefreshTodayArt()
    {
        try
        {
            GetNameScript.GetTodayArt(todayArt, grayMaterial);
            DailyRolloverCountdown();

            var timeSpan = TimeSpan.FromSeconds((double)LevelProgressControl.control.dailyTodayPersonalBest);
            string str = string.Format("{0:D2}:{1:D2}", timeSpan.Minutes, timeSpan.Seconds);
            bestTimeText.text = str;
        }
        catch(NullReferenceException e)
        {
            Debug.Log(e.Message);
        }
    }

    public void Init(MainMenuPage page)
    {
        Application.targetFrameRate = -1;
        if (!m_inited)
        {
            m_inited = true;
            Debug.Log("Main menu initialize");
            DataManager.Instance.GetImages(false, delegate (ImagesInfo list)
            {
                Debug.Log("Main Menu 136");
                if (m_newLibraryWindow != null)
                {
                    Debug.Log("Main Menu 139");
                    m_newLibraryWindow.FastOpen();
                    m_newLibraryWindow.Init(list);
                    base.StartCoroutine(WaitInitLibraryAndClosePlaneCoroutine());
                }
            });
        }
        gameObject.SetActive(true);
        switch (page)
        {
            case MainMenuPage.Library:
                m_currentWindow = m_newLibraryWindow;
                OnLibraryStartOpenHandler();
                AnalyticsManager.Instance.LibraryOpened();
                break;
            case MainMenuPage.MyWorks:
                m_newLibraryWindow.FastClose();
                m_myWorksWindow.FastOpen();
                m_currentWindow = m_myWorksWindow;
                m_myWorksWindow.Init();
                OnMyWorksStartOpenHandler();
                AnalyticsManager.Instance.MyWorksOpened();
                break;
            case MainMenuPage.Photos:
                m_newLibraryWindow.FastClose();
                m_photosWindow.FastOpen();
                m_currentWindow = m_photosWindow;
                m_photosWindow.Init();
                OnCreationTabStartOpenHandler();
                AnalyticsManager.Instance.PhotosOpened();
                break;
        }
    }

    public override void InitCanvas(Canvas canvas, bool setClosePosition = true)
    {
        base.InitCanvas(canvas, setClosePosition);
        m_newLibraryWindow.InitCanvas(canvas, false);
        m_myWorksWindow.InitCanvas(canvas, true);
        m_photosWindow.InitCanvas(canvas, true);
        m_settingsWindow.InitCanvas(canvas, true);
    }

    public void UpdateLibrary(ImagesInfo imagesInfo)
    {
        if (m_newLibraryWindow != null)
        {
            Debug.Log("reinit imagesInfo");
            m_newLibraryWindow.Init(imagesInfo);
        }
    }

    public override bool Close()
    {
        if (m_myWorksWindow.isActiveAndEnabled)
        {
            m_myWorksWindow.Close();
            m_newLibraryWindow.DefferedOpen(0.1f);
            AnalyticsManager.Instance.LibraryOpened();
            return false;
        }
        if (m_photosWindow.isActiveAndEnabled)
        {
            m_photosWindow.Close();
            m_newLibraryWindow.DefferedOpen(0.1f);
            AnalyticsManager.Instance.PhotosOpened();
        }
        SystemToolsWrapper.Minimize();
        return false;
    }

    public void LibraryButtonClick()
    {
        if (m_currentWindow != m_newLibraryWindow && !m_buttonsBlocked)
        {
            m_buttonsBlocked = true;
            //AdsWrapper.Instance.ShowInter("tab_change");
            WindowManager.Instance.CloseMe(m_currentWindow, WindowOpenStyle.FromRight);
            m_currentWindow = m_newLibraryWindow;
            m_newLibraryWindow.DefferedOpen(0.1f);
            SelectTab(m_libraryTab);
            AnalyticsManager.Instance.LibraryOpened();
            AudioManager.Instance.PlayClick();
        }
    }

    public void MyWorkButtonClick()
    {
        if (m_currentWindow != m_myWorksWindow && !m_buttonsBlocked)
        {
            m_buttonsBlocked = true;
            //AdsWrapper.Instance.ShowInter("tab_change");
            m_myWorksWindow.Init();
            if (m_currentWindow == m_newLibraryWindow)
            {
                WindowManager.Instance.CloseMe(m_currentWindow, WindowOpenStyle.FromLeft);
                m_myWorksWindow.DefferedOpen(0.1f);
            }
            else
            {
                WindowManager.Instance.CloseMe(m_currentWindow, WindowOpenStyle.FromRight);
                m_myWorksWindow.DefferedOpen(WindowOpenStyle.FromLeft, 0.1f);
            }
            m_currentWindow = m_myWorksWindow;
            SelectTab(m_myWorksTab);
            AnalyticsManager.Instance.MyWorksOpened();
            AudioManager.Instance.PlayClick();
        }
    }

    public void CreateButtonClick()
    {
        if (m_currentWindow != m_photosWindow && !m_buttonsBlocked)
        {
            m_buttonsBlocked = true;
            //AdsWrapper.Instance.ShowInter("tab_change");
            m_photosWindow.Init();
            if (m_currentWindow == m_newLibraryWindow || m_currentWindow == m_myWorksWindow)
            {
                WindowManager.Instance.CloseMe(m_currentWindow, WindowOpenStyle.FromLeft);
                m_photosWindow.DefferedOpen(0.1f);
            }
            else
            {
                WindowManager.Instance.CloseMe(m_currentWindow, WindowOpenStyle.FromRight);
                m_photosWindow.DefferedOpen(WindowOpenStyle.FromLeft, 0.1f);
            }
            m_currentWindow = m_photosWindow;
            SelectTab(m_photosTab);
            AnalyticsManager.Instance.PhotosOpened();
            AudioManager.Instance.PlayClick();
        }
    }

    public void QuestionButtonClick()
    {
        if (m_currentWindow != m_settingsWindow && !m_buttonsBlocked)
        {
            m_buttonsBlocked = true;
            WindowManager.Instance.CloseMe(m_currentWindow, WindowOpenStyle.FromLeft);
            m_currentWindow = m_settingsWindow;
            AnalyticsManager.Instance.MyWorksOpened();
            AudioManager.Instance.PlayClick();
            m_settingsWindow.DefferedOpen(0.1f);
            SelectTab(m_settingsTab);
        }
    }

    void OnMyWorksStartOpenHandler()
    {
        m_buttonsBlocked = false;
        SelectTab(m_myWorksTab);
    }

    void OnCreationTabStartOpenHandler()
    {
        m_buttonsBlocked = false;
        SelectTab(m_photosTab);
    }

    void OnLibraryStartOpenHandler()
    {
        m_buttonsBlocked = false;
        SelectTab(m_libraryTab);
    }

    void OnSettingsStartOpenHandler()
    {
        m_buttonsBlocked = false;
        SelectTab(m_settingsTab);
    }

    void SelectTab(LibraryTabButton tab)
    {
        m_libraryTab.SetHighlighted(m_libraryTab == tab);
        m_myWorksTab.SetHighlighted(m_myWorksTab == tab);
        m_photosTab.SetHighlighted(m_photosTab == tab);
        m_settingsTab.SetHighlighted(m_settingsTab == tab);
    }

    public override void SendActiveScreenEvent()
    {
        if (m_inited)
        {
            m_currentWindow.SendActiveScreenEvent();
        }
    }
    IEnumerator WaitInitLibraryAndClosePlaneCoroutine()
    {
        if (FirstOpen)
        {
            FirstOpen = false;
            float timer = 0.5f;
            yield return null;

            while (true)
            {
                timer -= Time.deltaTime;
                if (!(timer < 0f) && (!m_newLibraryWindow.isActiveAndEnabled || !m_newLibraryWindow.Inited) && (!m_myWorksWindow.isActiveAndEnabled || !m_myWorksWindow.Inited))
                {
                    if (!m_photosWindow.isActiveAndEnabled)
                    {
                        yield return null;
                        continue;
                    }
                    if (!m_photosWindow.Inited)
                    {
                        yield return null;
                        continue;
                    }
                }
                break;
            }
        }
        m_blockPlane.SetActive(false);
    }

    public void OpenDailyArt()
    {
        AudioManager.Instance.PlayClick();
        var w = WindowManager.Instance.OpenDailyArtWindow();

        w.Open();
        if (GetNameScript.Instance != null)
        {
            GetNameScript.Instance.Init();
        }
    }
}

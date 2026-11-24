

using System.Collections.Generic;
using System.Linq;

using UnityEngine;

public class WindowManager : MonoBehaviour
{
    [SerializeField] Transform m_uiParent;
    [SerializeField] Canvas m_canvas;

    List<BaseWindow> m_openedWindows;
    MainMenu m_mainMenu;
    CreationWindow m_creationWindow;
    DailyArtWindow m_dailyArtWindow;
    NewWorkbook m_workbook;
    NewWorkbook3D m_workbook3D;
    ShareWindow m_shareWindow;
    SettingsWindow m_settingsWindow;
    HelpWindow m_helpWindow;
    DetailsWindow m_detailsWindow;
    NewInappsWindow m_inappsWindow;
    TrialInappsWindow m_trialInappsWindow;
    IapPopup m_abTestWindow;
    TutorialWindow m_tutorialWindow;
    TutorialWindow m_tutorialWindow3D;
    FilterWindow m_filterWindow;

    public static WindowManager Instance { get;  set; }

    public int OpenedWindowsCount
    {
        get
        {
            return (m_openedWindows != null) ? m_openedWindows.Count : 0;
        }
    }

    public bool BannerEnabled
    {
        get
        {
            var baseWindow = m_openedWindows.LastOrDefault();
            return baseWindow != null && baseWindow.EnableBanner;
        }
    }

    void Awake()
    {
        Instance = this;
        m_openedWindows = new List<BaseWindow>();
    }

    public MainMenu OpenMainMenu()
    {
        if (m_mainMenu == null)
        {

            m_mainMenu = ((Component)m_uiParent.Find("main_menu")).GetComponent<MainMenu>();

        }
        m_openedWindows.Add(m_mainMenu);

        //AdsWrapper.Instance.OpenedWindowsCountChangedHandler();
        m_mainMenu.InitCanvas(m_canvas, true);
        m_mainMenu.Open();

        return m_mainMenu;
    }

    public MainMenu GetMainMenu()
    {
        return m_mainMenu;
    }

    public CreationWindow OpenCreationWindow()
    {
        if (m_creationWindow == null)
        {
            m_creationWindow = Object.Instantiate(PrefabLoader.Load<CreationWindow>("creation_window"));
            m_creationWindow.transform.SetParent(m_uiParent);
            m_creationWindow.transform.localScale = Vector2.one;
            ((RectTransform)m_creationWindow.transform).anchoredPosition = Vector3.zero;
            ((RectTransform)m_creationWindow.transform).sizeDelta = Vector2.zero;
        }
        m_openedWindows.Add(m_creationWindow);
        m_creationWindow.transform.SetAsLastSibling();

        //AdsWrapper.Instance.OpenedWindowsCountChangedHandler();
        m_creationWindow.InitCanvas(m_canvas, true);
        m_creationWindow.Open();
        return m_creationWindow;
    }

    public DailyArtWindow OpenDailyArtWindow()
    {
        if (m_dailyArtWindow == null)
        {
            m_dailyArtWindow = Object.Instantiate(PrefabLoader.Load<DailyArtWindow>("daily_art"));
            m_dailyArtWindow.transform.SetParent(m_uiParent);
            m_dailyArtWindow.transform.localScale = Vector2.one;
            ((RectTransform)m_dailyArtWindow.transform).anchoredPosition = Vector3.zero;
            ((RectTransform)m_dailyArtWindow.transform).sizeDelta = Vector2.zero;
        }
        m_openedWindows.Add(m_dailyArtWindow);
        m_dailyArtWindow.transform.SetAsLastSibling();

        //AdsWrapper.Instance.OpenedWindowsCountChangedHandler();
        m_dailyArtWindow.InitCanvas(m_canvas, true);
        m_dailyArtWindow.Open();
        return m_dailyArtWindow;
    }

    public NewWorkbook OpenWorkbook()
    {
        if (m_workbook == null)
        {
            m_workbook = ((Component)m_uiParent.Find("workbook")).GetComponent<NewWorkbook>();
        }
        m_openedWindows.Add(m_workbook);
        //AdsWrapper.Instance.OpenedWindowsCountChangedHandler();
        m_workbook.InitCanvas(m_canvas, true);
        m_workbook.Open();
        return m_workbook;
    }

    public NewWorkbook3D OpenWorkbook3D()
    {
        if (m_workbook3D == null)
        {
            m_workbook3D = ((Component)m_uiParent.Find("workbook")).GetComponent<NewWorkbook3D>();
        }
        m_openedWindows.Add(m_workbook3D);
        //AdsWrapper.Instance.OpenedWindowsCountChangedHandler();
        m_workbook3D.InitCanvas(m_canvas, true);
        m_workbook3D.Open();
        return m_workbook3D;
    }

    public TutorialWindow OpenTutorial()
    {
        if (m_tutorialWindow == null)
        {
            var r = PrefabLoader.Load<TutorialWindow>("tutorial_window_3d");
            if (r == null)
                return null;

            m_tutorialWindow = Object.Instantiate(r);
            m_tutorialWindow.transform.SetParent(m_uiParent);
            m_tutorialWindow.transform.localScale = Vector2.one;
            ((RectTransform)m_tutorialWindow.transform).anchoredPosition = Vector3.zero;
            ((RectTransform)m_tutorialWindow.transform).sizeDelta = Vector2.zero;
        }
        m_openedWindows.Add(m_tutorialWindow);
        m_tutorialWindow.transform.SetAsLastSibling();
        //AdsWrapper.Instance.OpenedWindowsCountChangedHandler();
        m_tutorialWindow.InitCanvas(m_canvas, true);
        m_tutorialWindow.Open();
        return m_tutorialWindow;
    }

    public TutorialWindow OpenTutorial3D()
    {
        if (m_tutorialWindow3D == null)
        {
            var r = PrefabLoader.Load<TutorialWindow>("tutorial_window_3d");
            if (r == null)
                return null;
            m_tutorialWindow3D = Object.Instantiate(r);
            if (m_tutorialWindow3D == null)
                return null;

            m_tutorialWindow3D.transform.SetParent(m_uiParent);
            m_tutorialWindow3D.transform.localScale = Vector2.one;
            ((RectTransform)m_tutorialWindow3D.transform).anchoredPosition = Vector3.zero;
            ((RectTransform)m_tutorialWindow3D.transform).sizeDelta = Vector2.zero;
        }
        m_openedWindows.Add(m_tutorialWindow3D);
        m_tutorialWindow3D.transform.SetAsLastSibling();
        //AdsWrapper.Instance.OpenedWindowsCountChangedHandler();
        m_tutorialWindow3D.InitCanvas(m_canvas, true);
        m_tutorialWindow3D.Open();
        return m_tutorialWindow3D;
    }

    public FilterWindow OpenFilterWindow()
    {
        try
        {
            if (m_filterWindow == null)
            {

                m_filterWindow = Object.Instantiate(PrefabLoader.Load<FilterWindow>("filter_window"));
                m_filterWindow.transform.SetParent(m_uiParent);
                m_filterWindow.transform.localScale = Vector2.one;
                ((RectTransform)m_filterWindow.transform).anchoredPosition = Vector3.zero;
                ((RectTransform)m_filterWindow.transform).sizeDelta = Vector2.zero;


            }
            m_openedWindows.Add(m_filterWindow);
            m_filterWindow.transform.SetAsLastSibling();
            //AdsWrapper.Instance.OpenedWindowsCountChangedHandler();
            m_filterWindow.InitCanvas(m_canvas, true);
            m_filterWindow.Open();
        }
        catch (System.Exception e)
        {
            Debug.Log(e.Message);
        }
        return m_filterWindow;
    }

    public ShareWindow OpenShareWindow()
    {
        if (m_shareWindow == null)
        {
            m_shareWindow = Object.Instantiate(PrefabLoader.Load<ShareWindow>("share_window"));
            m_shareWindow.transform.SetParent(m_uiParent);
            m_shareWindow.transform.localScale = Vector2.one;
            ((RectTransform)m_shareWindow.transform).anchoredPosition = Vector3.zero;
            ((RectTransform)m_shareWindow.transform).sizeDelta = Vector2.zero;
            m_shareWindow.OnOpen += () =>
            {
                if (GameScene.Instance != null)
                {
                    GameScene.Instance.gameObject.SetActive(false);
                }
            };
            m_shareWindow.OnClose += () =>
            {
                if (GameScene.Instance != null)
                {
                    GameScene.Instance.gameObject.SetActive(true);
                }
            };
        }
        m_openedWindows.Add(m_shareWindow);
        m_shareWindow.transform.SetAsLastSibling();
        //AdsWrapper.Instance.OpenedWindowsCountChangedHandler();
        m_shareWindow.InitCanvas(m_canvas, true);
        m_shareWindow.Open();
        AnalyticsManager.Instance.ShareWindowOpened();
        return m_shareWindow;
    }

    public SettingsWindow OpenSettingsWindow()
    {
        if (m_settingsWindow == null)
        {
            m_settingsWindow = Object.Instantiate(PrefabLoader.Load<SettingsWindow>("settings_window"));
            m_settingsWindow.transform.SetParent(m_uiParent);
            m_settingsWindow.transform.localScale = Vector2.one;
            ((RectTransform)m_settingsWindow.transform).anchoredPosition = Vector3.zero;
            ((RectTransform)m_settingsWindow.transform).sizeDelta = Vector2.zero;
        }
        m_openedWindows.Add(m_settingsWindow);
        m_settingsWindow.transform.SetAsLastSibling();
        //AdsWrapper.Instance.OpenedWindowsCountChangedHandler();
        m_settingsWindow.InitCanvas(m_canvas, true);
        m_settingsWindow.Open();
        AnalyticsManager.Instance.SettingsWindowOpened();
        return m_settingsWindow;
    }

    public HelpWindow OpenHelpWindow()
    {
        if (m_helpWindow == null)
        {
            m_helpWindow = Object.Instantiate(PrefabLoader.Load<HelpWindow>("help_window"));
            m_helpWindow.transform.SetParent(m_uiParent);
            m_helpWindow.transform.localScale = Vector2.one;
            ((RectTransform)m_helpWindow.transform).anchoredPosition = Vector3.zero;
            ((RectTransform)m_helpWindow.transform).sizeDelta = Vector2.zero;
        }
        m_openedWindows.Add(m_helpWindow);
        m_helpWindow.transform.SetAsLastSibling();
        //AdsWrapper.Instance.OpenedWindowsCountChangedHandler();
        m_helpWindow.InitCanvas(m_canvas, true);
        m_helpWindow.Open();
        AnalyticsManager.Instance.HelpWindowOpened();
        return m_helpWindow;
    }

    public DetailsWindow OpenDetailsWindow()
    {
        if (m_detailsWindow == null)
        {
            m_detailsWindow = Object.Instantiate(PrefabLoader.Load<DetailsWindow>("details_window"));
            m_detailsWindow.transform.SetParent(m_uiParent);
            m_detailsWindow.transform.localScale = Vector2.one;
            ((RectTransform)m_detailsWindow.transform).anchoredPosition = Vector3.zero;
            ((RectTransform)m_detailsWindow.transform).sizeDelta = Vector2.zero;
        }
        m_openedWindows.Add(m_detailsWindow);
        m_detailsWindow.transform.SetAsLastSibling();
        //AdsWrapper.Instance.OpenedWindowsCountChangedHandler();
        m_detailsWindow.InitCanvas(m_canvas, true);
        m_detailsWindow.Open();
        return m_detailsWindow;
    }

    public NewInappsWindow OpenInappsWindow()
    {
        if (m_inappsWindow == null)
        {
            m_inappsWindow = Object.Instantiate(PrefabLoader.Load<NewInappsWindow>("inapps_window_android"));
            m_inappsWindow.transform.SetParent(m_uiParent);
            m_inappsWindow.transform.localScale = Vector2.one;
            ((RectTransform)m_inappsWindow.transform).anchoredPosition = Vector3.zero;
            ((RectTransform)m_inappsWindow.transform).sizeDelta = Vector2.zero;
        }
        m_openedWindows.Add(m_inappsWindow);
        m_inappsWindow.transform.SetAsLastSibling();
        //AdsWrapper.Instance.OpenedWindowsCountChangedHandler();
        m_inappsWindow.InitCanvas(m_canvas, true);
        m_inappsWindow.Open();
        return m_inappsWindow;
    }

    //public TrialInappsWindow OpenTrialInappsWindow()
    //{
    //	if (this.m_trialInappsWindow == null)
    //	{
    //		this.m_trialInappsWindow = Object.Instantiate(PrefabLoader.Load<TrialInappsWindow>("trial_inapps_window"));
    //		this.m_trialInappsWindow.transform.SetParent(this.m_uiParent);
    //		this.m_trialInappsWindow.transform.localScale = Vector2.one;
    //		((RectTransform)this.m_trialInappsWindow.transform).anchoredPosition = Vector3.zero;
    //		((RectTransform)this.m_trialInappsWindow.transform).sizeDelta = Vector2.zero;
    //	}
    //	this.m_openedWindows.Add(this.m_trialInappsWindow);
    //	this.m_trialInappsWindow.transform.SetAsLastSibling();
    //	AdsWrapper.Instance.OpenedWindowsCountChangedHandler();
    //	this.m_trialInappsWindow.InitCanvas(this.m_canvas, true);
    //	this.m_trialInappsWindow.Open();
    //	return this.m_trialInappsWindow;
    //}

    public IapPopup OpenAbTestWindow()
    {
        if (m_abTestWindow == null)
        {
            m_abTestWindow = Object.Instantiate(PrefabLoader.Load<IapPopup>("ab_test_inapps_window"));
            m_abTestWindow.transform.SetParent(m_uiParent);
            m_abTestWindow.transform.localScale = Vector2.one;
            ((RectTransform)m_abTestWindow.transform).anchoredPosition = Vector3.zero;
            ((RectTransform)m_abTestWindow.transform).sizeDelta = Vector2.zero;
        }
        m_openedWindows.Add(m_abTestWindow);
        m_abTestWindow.transform.SetAsLastSibling();
        //AdsWrapper.Instance.OpenedWindowsCountChangedHandler();
        m_abTestWindow.InitCanvas(m_canvas, true);
        m_abTestWindow.Open();
        return m_abTestWindow;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && m_openedWindows.Count > 0)
        {
            var window = m_openedWindows.Last();
            CloseMe(window);
        }
    }

    public void CloseMe(BaseWindow window)
    {
        if (window.Close())
        {
            m_openedWindows.Remove(window);

            //AdsWrapper.Instance.OpenedWindowsCountChangedHandler();
            var baseWindow = m_openedWindows.Last();
            baseWindow.SendActiveScreenEvent();
        }
    }

    public void CloseMe(BaseWindow window, BaseWindow.WindowOpenStyle style)
    {
        if (window.Close(style))
        {
            m_openedWindows.Remove(window);
            //AdsWrapper.Instance.OpenedWindowsCountChangedHandler();
            var baseWindow = m_openedWindows.Last();
            baseWindow.SendActiveScreenEvent();
        }
    }
}



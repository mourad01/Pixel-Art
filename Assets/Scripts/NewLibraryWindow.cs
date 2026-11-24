using UnityObject = global::UnityEngine.Object;


using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class NewLibraryWindow : BaseWindow
{
    ImagesInfo m_imagesInfo;
    List<ImageInfo> m_images;
    List<GroupOfPreviews> m_imageGroups;
    int m_currentGroupIndex = -1;
    bool m_lock;
    bool m_needShowGroupIndex;
    float m_delta = -1f;
    float m_previewSize = -1f;
    int m_pairsCount = -1;

    [SerializeField] ScrollRect m_scrollRect;
    [SerializeField] GroupOfPreviews m_groupOfPreviewsPrefab;
    [SerializeField] RectTransform m_groupOfPreviewsParent;
    [SerializeField] BigPreview m_bigPreview;
    [SerializeField] GameObject m_blockPlane;



    public bool Inited
    {
        get
        {
            return m_imageGroups[0].Loaded;
        }
    }

    protected override string WindowName
    {
        get
        {
            return "mainMenu_libraryTab";
        }
    }

    NewLibraryWindow()
    {
        m_imageGroups = new List<GroupOfPreviews>();
    }

    static NewLibraryWindow instance;
    void Awake()
    {
        instance = this;

        //IAPWrapper instance = IAPWrapper.Instance;
        //instance.OnPurchase = (Action<bool, SubscriptionType>)Delegate.Combine(instance.OnPurchase, new Action<bool, SubscriptionType>(this.OnPurchaseHandler));
        //DataManager instance2 = DataManager.Instance;
        //instance2.OnInternetAppeared = (Action)Delegate.Combine(instance2.OnInternetAppeared, new Action(this.OnInternetAppearedHandler));
    }
    public void Start()
    {

        MainMenu.Instance.RefreshTodayArt();
    }
    public void Init(ImagesInfo imagesInfo)
    {
        Debug.Log("New library Images count = " + imagesInfo.Count);
        if (m_imagesInfo != null && m_imagesInfo.Images.Count == imagesInfo.Images.Count)
        {
            return;
        }
        Debug.Log("New library Create categories");
        m_imagesInfo = imagesInfo;
        m_currentGroupIndex = -1;
        m_images = m_imagesInfo.Images;
        //two previews per row
        m_pairsCount = (m_images.Count + 1) / 2;
        m_delta = 20f;
        float size = ((RectTransform)transform).rect.width - m_delta * 2f;
        m_previewSize = (size - m_delta) / 2f;
        m_scrollRect.content.sizeDelta = new Vector2(size, (float)m_pairsCount * (m_previewSize + m_delta) + m_delta);
        // pool of 10 rows reused while scrolling
        if (m_imageGroups.Count == 0)
        {
            for (int i = 0; i < 10; i++)
            {
                var groupOfPreviews = UnityObject.Instantiate(m_groupOfPreviewsPrefab);
                groupOfPreviews.transform.SetParent(m_groupOfPreviewsParent);
                groupOfPreviews.transform.localPosition = Vector3.zero;
                groupOfPreviews.transform.localScale = Vector3.one;
                ((RectTransform)groupOfPreviews.transform).sizeDelta = new Vector2(size, m_previewSize);
                groupOfPreviews.gameObject.SetActive(false);
                groupOfPreviews.OnImageClick += OnImageClickHandler;
                groupOfPreviews.Index = -1;
                m_imageGroups.Add(groupOfPreviews);
            }
        }
        //jump to the row of the last opened image
        int index = m_images.FindIndex((ImageInfo a) => a.Id == MainMenu.ImageId) / 2;
        if (index < 0)
        {
            index = 0;
        }
        ShowGroupIndex(index, true);

    }

    public static void UpdateDailyClick(bool hasClick)
    {
        if (instance != null)
        {
            MainMenu.Instance.todayArt.OnClick = null;
            if (hasClick)
                MainMenu.Instance.todayArt.OnClick = instance.OpenImage;
        }
    }

    void Update()
    {
        var blockPlane = m_blockPlane;
        Vector2 velocity = m_scrollRect.velocity;
        int active;
        // block taps while the list scrolls fast
        if (!(velocity.y > 500f))
        {
            active = ((m_scrollRect.velocity.y < -500f) ? 1 : 0);
        }
        else
        {
            active = 1;
        }
        blockPlane.SetActive((byte)active != 0);
        if (m_needShowGroupIndex)
        {
            ShowGroupIndex(m_currentGroupIndex, true);
            m_needShowGroupIndex = false;
        }
    }

    public void ShowGroupIndex(int index, bool force = false)
    {
        if (isActiveAndEnabled)
        {
            base.StartCoroutine(ShowGroupIndexCoroutine(index, force));
        }
        else
        {
            m_needShowGroupIndex = true;
        }
    }

    public void OnScrollRectScrolled(Vector2 pos)
    {
        if (!m_lock && m_imageGroups.Count > 0)
        {
            Vector2 sizeDelta = ((Component)m_imageGroups[0]).GetComponent<RectTransform>().sizeDelta;
            float y = sizeDelta.y;
            int spacing = 20;
            //row index under the current scroll position
            float scrolled = m_scrollRect.content.sizeDelta.y * (1f - pos.y);
            int index = Mathf.RoundToInt((scrolled - (float)spacing) / (y + (float)spacing));
            OnSelectIndexHandler(index, false);
        }
    }

    void OnImageClickHandler(ImageInfo imageInfo, ImagePreview imagePreview, GroupOfPreviews groupOfPreviews)
    {
        m_currentGroupIndex = m_imageGroups.IndexOf(groupOfPreviews);
        OpenImage(imageInfo, imagePreview);
    }

    public void OpenImage(ImageInfo imageInfo, ImagePreview imagePreview)
    {
        DailyGame.ResetState(false);

        AnalyticsManager.Instance.ImageSelected(imageInfo);

        DrawAction(imageInfo, imagePreview);
    }

    void SelectPreview(string saveId, ImageInfo imageInfo, ImagePreview preview)
    {
        ISavedWorkData swd = null;

        if (saveId != null)
        {
            swd = MainManager.Instance.SavedWorksList.LoadById(saveId);

        }
        m_bigPreview.Init((RectTransform)preview.transform, preview.Texture, preview.Locked, swd);

    }
    public void ExecutePuzzle()
    {
        string strIdent = GetNameScript.GetDailyIdentity(0);
        if (LevelProgressControl.control != null)
        {
            LevelProgressControl.control.puzzleSrcType = LevelProgressControl.PuzzleSourceType.PUZZLE_DAILY;
            LevelProgressControl.control.postcardOrigin = SceneManager.GetActiveScene().name;
            LevelProgressControl.control.puzzleBasename = strIdent;
        }
        //datafile key holds the daily puzzle name
        PlayerPrefs.SetString("datafile", strIdent); 
    }
    public static void ExecutePuzzle(ImageInfo imageInfo)
    {
        if (instance != null)
        {
            DailyGame.ResetState(true);
            AnalyticsManager.Instance.ImageSelected(imageInfo);
            instance.DrawAction(imageInfo, null);
        }
    }
    public void DrawAction(ImageInfo imageInfo, ImagePreview preview)
    {
        // image is not cached and there is no internet
        if (!InternetConnection.IsAvailable && !DataManager.Instance.CheckLocalAsset(imageInfo))
        {
            DialogToolWrapper.ShowNoInternetDialog();
        }
        else if (!m_lock)
        {
            m_lock = true;
            string text = MainManager.Instance.SavedWorksList.LastSaveOfImageId(imageInfo.Id);

            if (preview != null)
            {
                DailyGame.ResetState(preview.TodayArt);
                if (preview.TodayArt)
                {
                    ExecutePuzzle();
                }
            }

            //no saved work for this image, start a new one
            if (text == null)
            { 
                NewImageButtonClick(imageInfo, ImageOpenType.New, preview);
            }
            else
            {
                if (preview != null)
                {
                    SelectPreview(text, imageInfo, preview);
                }
                ActionSheetWrapper.ShowImagePreviewActionSheet(delegate (ActionSheetResult a)
                {
                    switch (a)
                    {
                        case ActionSheetResult.Continue:
                            LoadButtonClick(imageInfo);

                            m_lock = false;
                            break;
                        case ActionSheetResult.New:
                            NewImageButtonClick(imageInfo, ImageOpenType.Reopened, preview);
                            m_lock = false;
                            break;
                        case ActionSheetResult.Cancel:
                            m_bigPreview.Close();
                            m_lock = false;
                            break;
                    }
                });

            }
        }
    }

    public void NewImageButtonClick(ImageInfo imageInfo, ImageOpenType imageOpenType, ImagePreview imagePreview)
    {

        if ((imageInfo.CustomAccessStatus == AccessStatus.Free && imagePreview != null && !imagePreview.AdsAvailable) || AppData.UnlockedImages.Contains(imageInfo.Id))
        {

            base.StartCoroutine(NewImageButtonClickCoroutine(imageInfo, imageOpenType));
        }
        else
        {


            base.StartCoroutine(NewImageButtonClickCoroutine(imageInfo, imageOpenType));

            m_lock = false;
        }
    }

    public void LoadButtonClick(ImageInfo imageInfo)
    {

        m_lock = true;

            ClearPreviews();
            string id = MainManager.Instance.SavedWorksList.LastSaveOfImageId(imageInfo.Id);
            var savedWorkData = MainManager.Instance.SavedWorksList.Load(SavedWorksList.GetPathToSave(id));
            MainMenu.LastPage = MainMenuPage.Library;
            MainMenu.ImageId = savedWorkData.ImageInfo.Id;
            MainMenu.WorkId = savedWorkData.Id;

            //rd.OnRewardedAdLoadedEvent(adUnitId, adInfo);

            MainManager.Instance.StartWorkbook(savedWorkData, ImageOpenType.Continued, delegate
            {

                m_bigPreview.Close();
                m_lock = false;
            });

    }

    void ClearPreviews()
    {
    }

    void OnSelectIndexHandler(int index, bool force = false)
    {
        try
        {
            if (index >= 0 && index < m_pairsCount && !m_lock || force)
            {
                if (index != m_currentGroupIndex || force)
                {
                    bool forward = index > m_currentGroupIndex;
                    var list = (from a in m_imageGroups
                                                  orderby a.Index
                                                  select a).ToList();
                    m_currentGroupIndex = index;
                    // fill rows from 4 above to 5 below the current one
                    for (int i = -4; i < 6; i++)
                    {
                        int localIndex = m_currentGroupIndex + i;
                        if (localIndex >= 0 && localIndex < m_pairsCount)
                        {
                            var groupOfPreviews = m_imageGroups.FirstOrDefault((GroupOfPreviews a) => a.Index == localIndex);
                            if (groupOfPreviews == null)
                            {
                                //recycle the row left behind by the scroll
                                groupOfPreviews = ((!forward) ? list[m_imageGroups.Count - 1] : list[0]);
                                groupOfPreviews.Index = localIndex;
                                groupOfPreviews.Clear();
                                groupOfPreviews.AddPreview(m_images[localIndex * 2]);
                                if (localIndex * 2 + 1 < m_images.Count)
                                {
                                    groupOfPreviews.AddPreview(m_images[localIndex * 2 + 1]);
                                }
                                groupOfPreviews.LoadIcons();
                                ((RectTransform)groupOfPreviews.transform).anchoredPosition = new Vector2(0f, 0f - ((float)localIndex * (m_previewSize + m_delta) + m_delta));
                                list.Remove(groupOfPreviews);
                            }
                            else if (force)
                            {
                                groupOfPreviews.Index = localIndex;
                                groupOfPreviews.Clear();
                                groupOfPreviews.AddPreview(m_images[localIndex * 2]);
                                if (localIndex * 2 + 1 < m_images.Count)
                                {
                                    groupOfPreviews.AddPreview(m_images[localIndex * 2 + 1]);
                                }
                                groupOfPreviews.LoadIcons();
                            }
                        }
                    }
                }
            } 
        }
        catch (Exception ex)
        {
            Debug.Log(ex.Message);
        }
    }

    void ActivateWindowElement(MonoBehaviour element)
    {
        ((GroupOfPreviews)element).LoadIcons();
    }

    void DesactivateWindowElement(MonoBehaviour element)
    {
        ((GroupOfPreviews)element).UnloadIcons();
    }

    void OnPurchaseHandler(bool res, SubscriptionType subscription)
    {
        foreach (GroupOfPreviews imageGroup in m_imageGroups)
        {
            imageGroup.Reinit();
        }
    }

    void OnInternetAppearedHandler()
    {
        OnSelectIndexHandler(m_currentGroupIndex, true);
    }

    void OnDestroy()
    {
        //IAPWrapper instance = IAPWrapper.Instance;
        //instance.OnPurchase = (Action<bool, SubscriptionType>)Delegate.Remove(instance.OnPurchase, new Action<bool, SubscriptionType>(this.OnPurchaseHandler));
        //DataManager instance2 = DataManager.Instance;
        //instance2.OnInternetAppeared = (Action)Delegate.Remove(instance2.OnInternetAppeared, new Action(this.OnInternetAppearedHandler));
    }
    IEnumerator ShowGroupIndexCoroutine(int index, bool force = false)
    {
        yield return null;

        var y = (float)index * (m_previewSize + m_delta);
        var p = m_scrollRect.content.anchoredPosition;
        p.y = y;
        m_scrollRect.content.anchoredPosition = p;
        OnSelectIndexHandler(index, force);
    }
    IEnumerator NewImageButtonClickCoroutine(ImageInfo imageInfo, ImageOpenType imageOpenType)
    { 
        m_lock = true; 
        ClearPreviews();
        MainManager.Instance.StartWorkbook(imageInfo, imageOpenType, MainMenuPage.Library, delegate
        {
            m_bigPreview.Close();
            m_lock = false;
        });
        yield return null;
    }
}


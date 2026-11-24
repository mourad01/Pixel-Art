using UnityObject = global::UnityEngine.Object;


using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class MyPhotosWindow : BaseWindow
{
    List<PhotoInfo> m_photoInfos;
    List<GroupOfMyPhotos> m_imageGroups;
    int m_currentGroupIndex = -1;
    bool m_inited;
    bool m_lock;
    bool m_needReinit;
    float m_delta = -1f;
    float m_previewSize = -1f;
    int m_pairsCount = -1;

    [SerializeField] ScrollRect m_scrollRect;
    [SerializeField] GroupOfMyPhotos m_groupOfMyPhotosPrefab;
    [SerializeField] RectTransform m_groupOfPreviewsParent;
    [SerializeField] BigPreview m_bigPreview;

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
            return "mainMenu_createTab";
        }
    }

    MyPhotosWindow()
    {
        m_imageGroups = new List<GroupOfMyPhotos>();
    }

    public void Init()
    {
        m_currentGroupIndex = -1;
        if (!m_inited)
        {
            m_needReinit = true;
            m_inited = true;
        }
        else
        {
            OnSelectIndexHandler(0, false);
        }
    }

    void Reinit()
    {
        m_photoInfos = new List<PhotoInfo>();
        m_photoInfos.AddRange(MainManager.Instance.SavedWorksList.GetPhotos());
        m_photoInfos.Insert(0, new PhotoInfo(string.Empty, PhotoSource.Unknown));
        m_pairsCount = (m_photoInfos.Count + 1) / 2;
        m_delta = 20f;
        float contentWidth = ((RectTransform)transform).rect.width - m_delta * 2f;
        m_previewSize = (contentWidth - m_delta) / 2f;
        m_scrollRect.content.sizeDelta = new Vector2(contentWidth, (float)m_pairsCount * (m_previewSize + m_delta) + m_delta);
        if (m_imageGroups.Count == 0)
        {
            for (int i = 0; i < 10; i++)
            {
                var groupOfMyPhotos = UnityObject.Instantiate(m_groupOfMyPhotosPrefab);
                groupOfMyPhotos.transform.SetParent(m_groupOfPreviewsParent);
                groupOfMyPhotos.transform.localPosition = Vector3.zero;
                groupOfMyPhotos.transform.localScale = Vector3.one;
                ((RectTransform)groupOfMyPhotos.transform).sizeDelta = new Vector2(contentWidth, m_previewSize);
                groupOfMyPhotos.gameObject.SetActive(false);
                groupOfMyPhotos.OnImageClick += OnImageClickHandler;
                groupOfMyPhotos.Index = -1;
                m_imageGroups.Add(groupOfMyPhotos);
            }
        }
        OnSelectIndexHandler(0, true);
        m_scrollRect.normalizedPosition = new Vector2(0f, 1f);
    }

    void Update()
    {
        if (m_needReinit)
        {
            Reinit();
            m_needReinit = false;
        }
    }

    public void OnScrollRectScrolled(Vector2 pos)
    {
        if (!m_lock && m_imageGroups.Count > 0)
        {
            Vector2 sizeDelta = m_scrollRect.content.sizeDelta;
            float scrolled = sizeDelta.y * (1f - pos.y);
            int value2 = Mathf.RoundToInt((scrolled - m_delta) / (m_previewSize + m_delta));
            if (value2 < m_pairsCount)
            {
                OnSelectIndexHandler(value2, false);
            }
            else
            {
                OnSelectIndexHandler(m_pairsCount - 1, false);
            }
        }
    }

    void OnSelectIndexHandler(int index, bool force = false)
    {
        if (index >= 0 && index < m_pairsCount && !m_lock)
        {
            if (index != m_currentGroupIndex)
            {
                bool forward = index > m_currentGroupIndex;
                var list = (from a in m_imageGroups
                                              orderby a.Index
                                              select a).ToList();
                m_currentGroupIndex = index;
                for (int i = -4; i < 6; i++)
                {
                    int localIndex = m_currentGroupIndex + i;
                    if (localIndex >= 0 && localIndex < m_pairsCount)
                    {
                        var groupOfMyPhotos = m_imageGroups.FirstOrDefault((GroupOfMyPhotos a) => a.Index == localIndex);
                        if (groupOfMyPhotos == null)
                        {
                            groupOfMyPhotos = ((!forward) ? list[list.Count - 1] : list[0]);
                            groupOfMyPhotos.Clear();
                            groupOfMyPhotos.Index = localIndex;
                            groupOfMyPhotos.AddPreview(m_photoInfos[localIndex * 2]);
                            if (localIndex * 2 + 1 < m_photoInfos.Count)
                            {
                                groupOfMyPhotos.AddPreview(m_photoInfos[localIndex * 2 + 1]);
                            }
                            groupOfMyPhotos.LoadIcons();
                            ((RectTransform)groupOfMyPhotos.transform).anchoredPosition = new Vector2(0f, 0f - ((float)localIndex * (m_previewSize + m_delta) + m_delta));
                            list.Remove(groupOfMyPhotos);
                        }
                        else
                        {
                            if (force)
                            {
                                groupOfMyPhotos.CheckPreview(0, m_photoInfos[localIndex * 2]);
                                if (localIndex * 2 + 1 < m_photoInfos.Count)
                                {
                                    groupOfMyPhotos.CheckPreview(1, m_photoInfos[localIndex * 2 + 1]);
                                }
                                else
                                {
                                    groupOfMyPhotos.ClearPreview(1);
                                }
                                groupOfMyPhotos.LoadIcons();
                            }
                            list.Remove(groupOfMyPhotos);
                        }
                    }
                    else if (localIndex == m_pairsCount)
                    {
                        var groupOfMyPhotos = m_imageGroups.FirstOrDefault((GroupOfMyPhotos a) => a.Index == localIndex);
                        if (groupOfMyPhotos != null)
                        {
                            groupOfMyPhotos.Clear();
                        }
                    }
                }
            }
        }
        else if (m_pairsCount == 0)
        {
            foreach (GroupOfMyPhotos imageGroup in m_imageGroups)
            {
                imageGroup.Clear();
            }
        }
    }

    public void OnImageClickHandler(PhotoInfo photoInfo, PhotoPreview preview, GroupOfMyPhotos groupOfMyPhotos)
    {
        /*DailyGame.ResetState(false);
        if (string.IsNullOrEmpty(photoInfo.Id))
        {
            if (AppData.FreePhotos > 0)
            {
                CreationWindow creationWindow = WindowManager.Instance.OpenCreationWindow();
                creationWindow.Init(delegate (PhotoInfo info)
                {
                    this.NewImageButtonClick(info, ImageOpenType.New);
                    AppData.FreePhotos--;
                });
            }
            else if (INPluginWrapper.Instance.GetAbTestGroup() == ABTestGroup.RewardedNo_ContentEasy)
            {



                CreationWindow creationWindow3 = WindowManager.Instance.OpenCreationWindow();
                creationWindow3.Init(delegate (PhotoInfo info)
                {
                    this.NewImageButtonClick(info, ImageOpenType.New);
                });



            }

            IapPopup abWindow = WindowManager.Instance.OpenAbTestWindow();
            abWindow.Init(delegate (bool res)
            {
                if (res)
                {
                    CreationWindow creationWindow2 = WindowManager.Instance.OpenCreationWindow();
                    creationWindow2.Init(delegate (PhotoInfo info)
                    {
                        this.NewImageButtonClick(info, ImageOpenType.New);
                        WindowManager.Instance.CloseMe(abWindow);
                    });
                }
            }

        else
        {
            ABTestGroup abTestGroup = INPluginWrapper.Instance.GetAbTestGroup();
            if (abTestGroup == ABTestGroup.RewardedNo_ContentMedium_Old || abTestGroup == ABTestGroup.None)
            {
                NewInappsWindow newInappsWindow = WindowManager.Instance.OpenInappsWindow();
                newInappsWindow.Init("photo", null);
            }
            else
            {
                var trialInappsWindow = WindowManager.Instance.OpenInappsWindow();
                trialInappsWindow.Init("photo", null);//, false, false);
            }
        }
        }
        else
        {
            this.m_currentGroupIndex = this.m_imageGroups.IndexOf(groupOfMyPhotos);
            string text = MainManager.Instance.SavedWorksList.LastSaveOfImageId(photoInfo.Id);
            ISavedWorkData swd = null;
            if (text != null)
            {
                swd = MainManager.Instance.SavedWorksList.LoadById(text);
            }
            this.SelectPreview(preview, swd);
            this.DrawButtonClick(photoInfo, swd);
        }*/
    }

    void SelectPreview(PhotoPreview preview, ISavedWorkData swd)
    {
        m_bigPreview.Init((RectTransform)preview.transform, preview.Texture, false, swd);
    }

    public void DrawButtonClick(PhotoInfo photoInfo, ISavedWorkData swd)
    {
        AnalyticsManager.Instance.ImageSelected(new ImageInfo(photoInfo.Id, photoInfo.Source.ToString().ToLower()));
        if (!m_lock && m_currentGroupIndex >= 0)
        {
            m_lock = true;
            if (swd != null)
            {
                ActionSheetWrapper.ShowSavedWorkActionSheet(delegate (ActionSheetResult a)
                {
                    switch (a)
                    {
                        case ActionSheetResult.Continue:
                            LoadButtonClick(swd.Id);
                            break;
                        case ActionSheetResult.New:
                            NewImageButtonClick(photoInfo, ImageOpenType.Reopened);
                            break;
                        case ActionSheetResult.Share:
                            ShareButtonClick(swd.Id);
                            break;
                        case ActionSheetResult.Delete:
                            DeleteButtonClick(photoInfo.Id);
                            break;
                        case ActionSheetResult.Cancel:
                            m_bigPreview.Close();
                            break;
                    }
                    m_lock = false;
                });
            }
            else
            {
                ActionSheetWrapper.ShowEmptyPhotoActionSheet(delegate (ActionSheetResult a)
                {
                    switch (a)
                    {
                        case ActionSheetResult.New:
                            NewImageButtonClick(photoInfo, ImageOpenType.New);
                            break;
                        case ActionSheetResult.Delete:
                            DeleteButtonClick(photoInfo.Id);
                            break;
                        case ActionSheetResult.Cancel:
                            m_bigPreview.Close();
                            break;
                    }
                    m_lock = false;
                });
            }
        }
    }

    void LoadButtonClick(string saveId)
    {
        m_lock = true;
        var savedWorkData = MainManager.Instance.SavedWorksList.LoadById(saveId);
        MainMenu.LastPage = MainMenuPage.MyWorks;
        MainMenu.ImageId = savedWorkData.ImageInfo.Id;
        MainMenu.WorkId = savedWorkData.Id;
        MainManager.Instance.StartWorkbook(savedWorkData, ImageOpenType.Continued, null);
    }

    void NewImageButtonClick(PhotoInfo photoInfo, ImageOpenType imageOpenType)
    {
        m_lock = true;
        MainManager.Instance.StartWorkbook(new ImageInfo(photoInfo.Id, photoInfo.Source.ToString().ToLower()), imageOpenType, MainMenuPage.Photos, null);
    }

    public void DeleteButtonClick(string photoId)
    {
        m_bigPreview.Close();
        AnalyticsManager.Instance.DeleteButtonClick();
        DialogToolWrapper.ShowDeleteDialog(delegate (bool res)
        {
            if (res)
            {
                AnalyticsManager.Instance.DeleteSuccess();
                MainManager.Instance.SavedWorksList.DeletePhoto(photoId);
                m_currentGroupIndex = -1;
                m_lock = false;
                Reinit();
            }
        });
    }

    public void ShareButtonClick(string saveId)
    {
        m_bigPreview.Close();
        var shareWindow = WindowManager.Instance.OpenShareWindow();
        shareWindow.InitFromLibrary(saveId);
    }
}



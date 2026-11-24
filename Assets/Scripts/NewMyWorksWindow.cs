using UnityObject = global::UnityEngine.Object;


using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class NewMyWorksWindow : BaseWindow
{
    List<string> m_saveIds;
    List<GroupOfMyWorks> m_imageGroups;
    int m_currentGroupIndex = -1;
    bool m_inited;
    bool m_lock;
    bool m_needReinit;
    int m_needSelectIndex = -1;
    float m_delta = -1f;
    float m_previewSize = -1f;
    int m_pairsCount = -1;

    [SerializeField] ScrollRect m_scrollRect;
    [SerializeField] GroupOfMyWorks m_groupOfMyWorksPrefab;
    [SerializeField] RectTransform m_groupOfPreviewsParent;
    [SerializeField] GameObject m_noWorksBlock;
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
            return "mainMenu_worksTab";
        }
    }

    NewMyWorksWindow()
    {
        m_imageGroups = new List<GroupOfMyWorks>();
    }

    public void Init()
    {
        m_currentGroupIndex = -1;
        //first open builds the list in Update, later opens only refresh rows
        if (!m_inited)
        {
            m_needReinit = true;
            m_inited = true;
        }
        else if (m_saveIds.Count > 0)
        {
            m_needSelectIndex = 0;
        }
    }

    void Reinit()
    {
        var saves = MainManager.Instance.SavedWorksList.GetSaves();
        m_saveIds = new List<string>();
        m_saveIds.AddRange(saves.Distinct());
        // two saved works per row
        m_pairsCount = (m_saveIds.Count + 1) / 2;
        m_delta = 20f;
        float contentWidth = ((RectTransform)transform).rect.width - m_delta * 2f;
        m_previewSize = (contentWidth - m_delta) / 2f;
        m_scrollRect.content.sizeDelta = new Vector2(contentWidth, (float)m_pairsCount * (m_previewSize + m_delta) + m_delta);
        if (m_imageGroups.Count == 0)
        {
            for (int i = 0; i < 10; i++)
            {
                var groupOfMyWorks = UnityObject.Instantiate(m_groupOfMyWorksPrefab);
                groupOfMyWorks.transform.SetParent(m_groupOfPreviewsParent);
                groupOfMyWorks.transform.localPosition = Vector3.zero;
                groupOfMyWorks.transform.localScale = Vector3.one;
                ((RectTransform)groupOfMyWorks.transform).sizeDelta = new Vector2(contentWidth, m_previewSize);
                groupOfMyWorks.gameObject.SetActive(false);
                groupOfMyWorks.OnImageClick += OnImageClickHandler;
                groupOfMyWorks.Index = -1;
                m_imageGroups.Add(groupOfMyWorks);
            }
        }
        OnSelectIndexHandler(0, true);
        m_scrollRect.normalizedPosition = new Vector2(0f, 1f);
        m_noWorksBlock.SetActive(m_saveIds.Count == 0);
    }

    void Update()
    {
        if (m_needReinit)
        {
            Reinit();
            m_needReinit = false;
        }
        if (m_needSelectIndex >= 0)
        {
            OnSelectIndexHandler(m_needSelectIndex, true);
            m_needSelectIndex = -1;
        }
        var blockPlane = m_blockPlane;
        Vector2 velocity = m_scrollRect.velocity;
        int active;
        //block taps while the list scrolls fast
        if (!(velocity.y > 500f))
        {
            active = ((m_scrollRect.velocity.y < -500f) ? 1 : 0);
        }
        else
        {
            active = 1;
        }
        blockPlane.SetActive((byte)active != 0);
    }

    public void OnScrollRectScrolled(Vector2 pos)
    {
        if (!m_lock && m_imageGroups.Count > 0)
        {
            float scrolled = m_scrollRect.content.sizeDelta.y * (1f - pos.y);
            int index = Mathf.RoundToInt((scrolled - m_delta) / (m_previewSize + m_delta));
            if (index < m_pairsCount)
            {
                OnSelectIndexHandler(index, false);
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
                // only rows near the current one get a pooled group
                for (int i = -4; i < 6; i++)
                {
                    int localIndex = m_currentGroupIndex + i;
                    if (localIndex >= 0 && localIndex < m_pairsCount)
                    {
                        var groupOfMyWorks = m_imageGroups.FirstOrDefault((GroupOfMyWorks a) => a.Index == localIndex);
                        if (groupOfMyWorks == null)
                        {
                            groupOfMyWorks = ((!forward) ? list[list.Count - 1] : list[0]);
                            groupOfMyWorks.Clear();
                            groupOfMyWorks.Index = localIndex;
                            groupOfMyWorks.AddPreview(m_saveIds[localIndex * 2]);
                            if (localIndex * 2 + 1 < m_saveIds.Count)
                            {
                                groupOfMyWorks.AddPreview(m_saveIds[localIndex * 2 + 1]);
                            }
                            groupOfMyWorks.LoadIcons();
                            ((RectTransform)groupOfMyWorks.transform).anchoredPosition = new Vector2(0f, 0f - ((float)localIndex * (m_previewSize + m_delta) + m_delta));
                            list.Remove(groupOfMyWorks);
                        }
                        else
                        {
                            if (force)
                            {
                                groupOfMyWorks.CheckPreview(0, m_saveIds[localIndex * 2]);
                                if (localIndex * 2 + 1 < m_saveIds.Count)
                                {
                                    groupOfMyWorks.CheckPreview(1, m_saveIds[localIndex * 2 + 1]);
                                }
                                else
                                {
                                    groupOfMyWorks.ClearPreview(1);
                                }
                                groupOfMyWorks.LoadIcons();
                            }
                            list.Remove(groupOfMyWorks);
                        }
                    }
                    //clear the extra row left after a delete
                    else if (localIndex == m_pairsCount)
                    {
                        var groupOfMyWorks = m_imageGroups.FirstOrDefault((GroupOfMyWorks a) => a.Index == localIndex);
                        if (groupOfMyWorks != null)
                        {
                            groupOfMyWorks.Clear();
                        }
                    }
                }
            }
        }
        else if (m_pairsCount == 0)
        {
            foreach (GroupOfMyWorks imageGroup in m_imageGroups)
            {
                imageGroup.Clear();
            }
        }
    }

    public void OnImageClickHandler(string saveId, MyWorkPreview preview, GroupOfMyWorks groupOfMyWorks)
    {
        DailyGame.ResetState(false);
        m_currentGroupIndex = m_imageGroups.IndexOf(groupOfMyWorks);
        SelectPreview(saveId, preview);
        DrawButtonClick(saveId);
    }

    void SelectPreview(string saveId, MyWorkPreview preview)
    {
        var savedWorkData = MainManager.Instance.SavedWorksList.LoadById(saveId);
        AnalyticsManager.Instance.ImageSelected(savedWorkData.ImageInfo);
        m_bigPreview.Init((RectTransform)preview.transform, preview.Texture, false, savedWorkData);
    }

    public void DrawButtonClick(string saveId)
    {
        if (!m_lock && m_currentGroupIndex >= 0)
        {
            m_lock = true;
            ActionSheetWrapper.ShowSavedWorkActionSheet(delegate (ActionSheetResult a)
            {
                switch (a)
                {
                    case ActionSheetResult.Continue:
                        LoadButtonClick(saveId);
                        break;
                    case ActionSheetResult.New:
                        NewImageButtonClick(saveId);
                        break;
                    case ActionSheetResult.Delete:
                        DeleteButtonClick(saveId);
                        break;
                    case ActionSheetResult.Cancel:
                        m_bigPreview.Close();
                        break;
                }
                m_lock = false;
            });
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

    void NewImageButtonClick(string saveId)
    {
        var savedWorkData = MainManager.Instance.SavedWorksList.LoadById(saveId);
        var imageInfo = DataManager.Instance.GetImageInfo(savedWorkData.ImageInfo.Id);
        // image may be gone from the list, use the saved info then
        if (imageInfo == null)
        {
            imageInfo = savedWorkData.ImageInfo;
        }
        if (imageInfo.CustomAccessStatus == AccessStatus.Free)
        {
            m_lock = true;
            var savedWork = MainManager.Instance.SavedWorksList.LoadById(saveId);
            MainManager.Instance.StartWorkbook(savedWork.ImageInfo, ImageOpenType.Reopened, MainMenuPage.MyWorks, null);
        }
        else
        {
            m_lock = true;
            var savedWork = MainManager.Instance.SavedWorksList.LoadById(saveId);
            MainManager.Instance.StartWorkbook(savedWork.ImageInfo, ImageOpenType.Reopened, MainMenuPage.MyWorks, null);
        }
    }

    public void DeleteButtonClick(string saveId)
    {
        m_bigPreview.Close();
        AnalyticsManager.Instance.DeleteButtonClick();
        DialogToolWrapper.ShowDeleteDialog(delegate (bool res)
        {
            if (res)
            {
                AnalyticsManager.Instance.DeleteSuccess();
                MainManager.Instance.SavedWorksList.Delete(saveId);
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

    public bool GetStateBigmPreview()
    {
        return m_bigPreview.isActiveAndEnabled;
    }
}



using UnityObject = global::UnityEngine.Object;


using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VerticalMyWorksWindow : MonoBehaviour
{
    bool m_inited;
    List<MyWorkPreview> m_imageElements;
    List<string> m_saveIds;
    int m_currentImageIndex;
    bool m_lock;

    [SerializeField] ScrollRect m_scrollRect;
    [SerializeField] MyWorkPreview m_myWorkPreviewPrefab;
    [SerializeField] RectTransform m_imagePreviewElementsParent;
    [SerializeField] ShareWindow m_shareWindow;
    [SerializeField] GameObject m_noWorksBlock;

    public bool ShareWindowShowed
    {
        get
        {
            return m_shareWindow.isActiveAndEnabled;
        }
    }

    VerticalMyWorksWindow()
    {
        m_imageElements = new List<MyWorkPreview>();
        m_saveIds = new List<string>();
    }

    public void Init()
    {
        m_currentImageIndex = -1;
        if (!m_inited)
        {
            Reinit();
            m_inited = true;
        }
        else if (m_saveIds.Count > 0)
        {
            OnSelectIndexHandler(0);
        }
    }

    void Reinit()
    {
        m_saveIds = new List<string>();
        int i = 0;
        var saves = MainManager.Instance.SavedWorksList.GetSaves();
        for (i = 0; i < saves.Count; i++)
        {
            string save = saves[i];
            m_saveIds.Add(save);
            if (i >= m_imageElements.Count)
            {
                var myWorkPreview = UnityObject.Instantiate(m_myWorkPreviewPrefab);
                myWorkPreview.transform.SetParent(m_imagePreviewElementsParent);
                myWorkPreview.transform.localPosition = Vector3.zero;
                myWorkPreview.transform.localScale = Vector3.one;
                m_imageElements.Add(myWorkPreview);
            }
            m_imageElements[i].Init(save);
            m_imageElements[i].gameObject.SetActive(true);
        }
        for (; i < m_imageElements.Count; i++)
        {
            m_imageElements[i].gameObject.SetActive(false);
        }
        m_noWorksBlock.SetActive(saves.Count == 0);
        SetId(0);
        OnSelectIndexHandler(0);
    }

    public void InitAfterSetActive()
    {
        base.StartCoroutine(NormalizePosition());
    }
    public void ShowId(string id)
    {
        if (!string.IsNullOrEmpty(id))
        {
            base.StartCoroutine(ShowIdCoroutine(id));
        }
    }

    public void ImageClick(string saveId)
    {
        m_currentImageIndex = m_saveIds.IndexOf(saveId);
        SetId(m_currentImageIndex);
        DrawButtonClick();
    }

    public void OnScrollRectScrolled(Vector2 pos)
    {
        if (!m_lock && m_imageElements.Count > 0)
        {
            Vector2 sizeDelta = ((Component)m_imageElements[0]).GetComponent<RectTransform>().sizeDelta;
            float y = sizeDelta.y;
            float spacing = ((Component)m_scrollRect.content).GetComponent<VerticalLayoutGroup>().spacing;
            Vector2 sizeDelta2 = m_scrollRect.content.sizeDelta;
            float scrolled = sizeDelta2.y * (1f - pos.y);
            int index = Mathf.RoundToInt((scrolled - spacing) / (y + spacing));
            OnSelectIndexHandler(index);
        }
    }

    void SetId(int index)
    {
        if (m_imageElements.Count > 0)
        {
            Vector2 sizeDelta = ((Component)m_imageElements[0]).GetComponent<RectTransform>().sizeDelta;
            float y = sizeDelta.y;
            float spacing = ((Component)m_scrollRect.content).GetComponent<VerticalLayoutGroup>().spacing;
            float y2 = (float)index * (y + spacing);
            Vector2 anchoredPosition = m_scrollRect.content.anchoredPosition;
            anchoredPosition.y = y2;
            m_scrollRect.content.anchoredPosition = anchoredPosition;
        }
    }

    public void DrawButtonClick()
    {
        if (!m_lock && m_currentImageIndex >= 0)
        {
            m_lock = true;
            ActionSheetWrapper.ShowSavedWorkActionSheet(delegate (ActionSheetResult a)
            {
                switch (a)
                {
                    case ActionSheetResult.Continue:
                        LoadButtonClick();
                        break;
                    case ActionSheetResult.New:
                        NewImageButtonClick();
                        break;
                    case ActionSheetResult.Share:
                        ShareButtonClick();
                        break;
                    case ActionSheetResult.Delete:
                        DeleteButtonClick();
                        break;
                }
                m_lock = false;
            });
        }
    }

    void LoadButtonClick()
    {
        m_lock = true;
        ClearPreviews();
    }

    void NewImageButtonClick()
    {
        var savedWorkData = MainManager.Instance.SavedWorksList.LoadById(m_saveIds[m_currentImageIndex]);
        var imageInfo = DataManager.Instance.GetImageInfo(savedWorkData.ImageInfo.Id);
        if (imageInfo == null)
        {
            imageInfo = savedWorkData.ImageInfo;
        }
        if (imageInfo.CustomAccessStatus == AccessStatus.Free )
        {
            m_lock = true;
            ClearPreviews();
            var savedWorkData2 = MainManager.Instance.SavedWorksList.LoadById(m_saveIds[m_currentImageIndex]);
            MainManager.Instance.StartWorkbook(savedWorkData2.ImageInfo, ImageOpenType.Reopened, MainMenuPage.MyWorks, null);
        }
        else
        {
            var newInappsWindow = WindowManager.Instance.OpenInappsWindow();
            newInappsWindow.Init("image", null);
        }
    }

    public void DeleteButtonClick()
    {
        if (m_currentImageIndex >= 0)
        {
            DialogToolWrapper.ShowDeleteDialog(delegate (bool res)
            {
                if (res)
                {
                    MainManager.Instance.SavedWorksList.Delete(m_saveIds[m_currentImageIndex]);
                    m_currentImageIndex = -1;
                    m_lock = false;
                    Reinit();
                }
            });
        }
    }

    public void ShareButtonClick()
    {
        if (m_currentImageIndex >= 0)
        {
            m_shareWindow.InitFromLibrary(m_saveIds[m_currentImageIndex]);
        }
    }

    public void ShareWindowClose()
    {
        m_shareWindow.Close();
    }

    void OnSelectIndexHandler(int index)
    {
        if (index >= 0 && index < m_imageElements.Count && !m_lock && index != m_currentImageIndex)
        {
            m_currentImageIndex = index;
            if (index >= 0)
            {
                m_imageElements[index].LoadIcon();
                if (index + 1 < m_imageElements.Count)
                {
                    m_imageElements[index + 1].LoadIcon();
                    if (index + 2 < m_imageElements.Count)
                    {
                        m_imageElements[index + 2].LoadIcon();
                    }
                }
                if (index - 1 >= 0)
                {
                    m_imageElements[index - 1].LoadIcon();
                    if (index - 2 >= 0)
                    {
                        m_imageElements[index - 2].LoadIcon();
                    }
                }
                if (index - 5 >= 0)
                {
                    m_imageElements[index - 5].UnloadIcon();
                }
                if (index + 5 < m_imageElements.Count)
                {
                    m_imageElements[index + 5].UnloadIcon();
                }
            }
        }
    }

    public void UnloadAll()
    {
        for (int i = 0; i < m_imageElements.Count; i++)
        {
            m_imageElements[i].UnloadIcon();
        }
    }

    void ClearPreviews()
    {
        for (int i = 0; i < m_imageElements.Count; i++)
        {
            if (i != m_currentImageIndex)
            {
                m_imageElements[i].UnloadIcon();
            }
        }
    }

    public void OnDisable()
    {
        if (m_shareWindow.isActiveAndEnabled)
        {
            m_shareWindow.Close();
        }
    }
    IEnumerator NormalizePosition()
    {
        yield return null;

        for (int i = 0; i < m_imageElements.Count; i++)
        {
            Vector3 localPosition = ((RectTransform)m_imageElements[i].transform).localPosition;
            localPosition.z = -0.1f;
            ((RectTransform)m_imageElements[i].transform).localPosition = localPosition;
        }
        if (m_scrollRect != null)
        {
            m_scrollRect.normalizedPosition = new Vector2(0f, 1f);
        }
        m_scrollRect.normalizedPosition = new Vector2(0f, 1f);
        OnScrollRectScrolled(m_scrollRect.normalizedPosition);
    }
    IEnumerator ShowIdCoroutine(string id)
    {
        yield return null;
        SetId(m_saveIds.IndexOf(id));
    }
}

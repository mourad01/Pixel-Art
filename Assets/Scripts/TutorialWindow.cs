using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TutorialWindow : BaseWindow
{
    int m_currentIndex;
    string m_placement = string.Empty;
    string m_type = string.Empty;

    [SerializeField] PositScrollRect m_positScrollRect;
    [SerializeField] List<TutorialPage> m_pages;
    [SerializeField] List<Image> m_points;

    protected override string WindowName
    {
        get
        {
            return "tutorialWindow";
        }
    }

    void Awake()
    {
        m_positScrollRect.OnSelectIndex += OnSelectIndexHandler;
    }

    public void Init(string placement, string type)
    {
        m_currentIndex = 0;
        m_placement = placement;
        m_type = type;
        foreach (TutorialPage page in m_pages)
        {
            ((RectTransform)page.transform).sizeDelta = new Vector2(((RectTransform)transform).rect.width, ((RectTransform)transform).rect.height);
        }
        m_positScrollRect.Reinit(0f, false);
        AnalyticsManager.Instance.TutorOpened(placement, type);
        UpdatePoint();

        AppData.TutorialCompleted = true; 
    }

    void OnSelectIndexHandler(int index)
    {
        if (m_currentIndex != index)
        {
            if (m_currentIndex < m_pages.Count && m_currentIndex >= 0)
            {
                m_pages[m_currentIndex].Stop();
            }
            m_currentIndex = index;
            if (m_currentIndex < m_pages.Count && m_currentIndex >= 0)
            {
                m_pages[m_currentIndex].Play();
            }
            UpdatePoint();
        }
    }

    void UpdatePoint()
    {
        if (m_currentIndex < m_pages.Count && m_currentIndex >= 0)
        {
            for (int i = 0; i < m_pages.Count; i++)
            {
                if (i == m_currentIndex)
                {
                    m_points[i].SetAlpha(1f);
                }
                else
                {
                    m_points[i].SetAlpha(0.3f);
                }
            }
        }
    }

    public void CloseButtonClick()
    {
        AnalyticsManager.Instance.TutorClosed(m_placement, m_type, m_currentIndex + 1);
        WindowManager.Instance.CloseMe(this);
        AudioManager.Instance.PlayClick();
    }

    public void StartButtonClick()
    {
        AnalyticsManager.Instance.TutorClosed(m_placement, m_type, m_currentIndex + 1);
        WindowManager.Instance.CloseMe(this);
        AudioManager.Instance.PlayClick();
    }

    void OnDestroy()
    {
        m_positScrollRect.OnSelectIndex -= OnSelectIndexHandler;
    }
}



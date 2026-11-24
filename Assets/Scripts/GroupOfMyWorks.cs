

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GroupOfMyWorks : MonoBehaviour
{
    public Action<string, MyWorkPreview, GroupOfMyWorks> OnImageClick;

    int m_index = -1;

    [SerializeField] List<MyWorkPreview> m_previews;

    int m_emptyIndex;
    bool m_subscribed;

    public int Index
    {
        get
        {
            return m_index;
        }
        set
        {
            m_index = value;
            gameObject.SetActive(m_index >= 0);
        }
    }

    public bool Loaded
    {
        get
        {
            return m_previews.All((MyWorkPreview a) => a.Loaded);
        }
    }

    public void AddPreview(string saveId)
    {
        if (!m_subscribed)
        {
            Subscribe();
        }
        m_previews[m_emptyIndex].Init(saveId);
        m_previews[m_emptyIndex].gameObject.SetActive(true);
        m_emptyIndex++;
    }

    public void CheckPreview(int index, string saveId)
    {
        if (m_previews[index].SaveId != saveId)
        {
            m_previews[index].Init(saveId);
            m_previews[index].gameObject.SetActive(true);
        }
    }

    public void Subscribe()
    {
        m_subscribed = true;
        for (int i = 0; i < m_previews.Count; i++)
        {
            m_previews[i].OnClick += OnImageClickHandler;
        }
    }

    public void LoadIcons()
    {
        base.StartCoroutine(LoadIconsCoroutine());
    }

    public void UnloadIcons()
    {
    }

    public void Reinit()
    {
    }

    void OnImageClickHandler(string saveId, MyWorkPreview imagePreview)
    {
        OnImageClick.SafeInvoke(saveId, imagePreview, this);
    }

    public void Clear()
    {
        for (int i = 0; i < m_previews.Count; i++)
        {
            m_previews[i].gameObject.SetActive(false);
        }
        m_emptyIndex = 0;
        Index = -1;
    }

    public void ClearPreview(int index)
    {
        m_previews[index].gameObject.SetActive(false);
        if (m_previews.All((MyWorkPreview a) => !a.gameObject.activeSelf))
        {
            Index = -1;
        }
    }
    IEnumerator LoadIconsCoroutine()
    {
        for (int i = 0; i < m_previews.Count; i++)
        {
            if (m_previews[i].IsInited)
            {
                m_previews[i].LoadIcon();
            }
            yield return null;
        }
    }
}



using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GroupOfMyPhotos : MonoBehaviour
{
    public Action<PhotoInfo, PhotoPreview, GroupOfMyPhotos> OnImageClick;

    int m_index = -1;

    [SerializeField] List<PhotoPreview> m_previews;

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
            return m_previews.All((PhotoPreview a) => a.Loaded);
        }
    }

    public void AddPreview(PhotoInfo photoInfo)
    {
        if (!m_subscribed)
        {
            Subscribe();
        }
        m_previews[m_emptyIndex].Init(photoInfo);
        m_previews[m_emptyIndex].gameObject.SetActive(true);
        m_emptyIndex++;
    }

    public void CheckPreview(int index, PhotoInfo photoInfo)
    {
        if (m_previews[index].PhotoId != photoInfo.Id)
        {
            m_previews[index].Init(photoInfo);
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
        for (int i = 0; i < m_previews.Count; i++)
        {
            if (m_previews[i].isActiveAndEnabled)
            {
                m_previews[i].UnloadIcon();
            }
        }
    }

    public void Reinit()
    {
        for (int i = 0; i < m_previews.Count; i++)
        {
            if (m_previews[i].Inited)
            {
                m_previews[i].Reinit();
            }
        }
    }

    void OnImageClickHandler(PhotoInfo photoInfo, PhotoPreview imagePreview)
    {
        OnImageClick.SafeInvoke(photoInfo, imagePreview, this);
    }

    public void Clear()
    {
        for (int i = 0; i < m_previews.Count; i++)
        {
            m_previews[i].gameObject.SetActive(false);
        }
        m_emptyIndex = 0;
    }

    public void ClearPreview(int index)
    {
        m_previews[index].gameObject.SetActive(false);
        //whole row empty, hide it
        if (m_previews.All((PhotoPreview a) => !a.gameObject.activeSelf))
        {
            Index = -1;
        }
    }
    IEnumerator LoadIconsCoroutine()
    {
        // one icon per frame to avoid a hitch
        for(int i = 0; i < m_previews.Count; i++)
        {
            if (m_previews[i].Inited)
            {
                m_previews[i].LoadIcon();
            }
            yield return null;
        }
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GroupOfPreviews : MonoBehaviour
{
    public Action<ImageInfo, ImagePreview, GroupOfPreviews> OnImageClick;

    int m_index = -1;

    [SerializeField] List<ImagePreview> m_previews;

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
            return m_previews.All((ImagePreview a) => a.Loaded);
        }
    }

    public void AddPreview(ImageInfo imageInfo)
    {

        if (!m_subscribed)
        {

            Subscribe();
        }
        if (!m_previews[m_emptyIndex].CheckTheSame(imageInfo))
        {

            m_previews[m_emptyIndex].Init(imageInfo);
        }
        m_previews[m_emptyIndex].gameObject.SetActive(true);
        m_emptyIndex++;
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

    void OnImageClickHandler(ImageInfo imageInfo, ImagePreview imagePreview)
    {
        OnImageClick.SafeInvoke(imageInfo, imagePreview, this);
    }

    public void Clear()
    {

        for (int i = 0; i < m_previews.Count; i++)
        {
            m_previews[i].gameObject.SetActive(false);
        }
        m_emptyIndex = 0;
    }
    IEnumerator LoadIconsCoroutine()
    {
        //one icon per frame
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

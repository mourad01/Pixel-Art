using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BigPreview : MonoBehaviour
{
    bool m_materialInited;
    RectTransform m_sampleTransform;

    [SerializeField] RectTransform m_content;
    [SerializeField] RawImage m_image;
    [SerializeField] RawImage m_filterImage;
    [SerializeField] GameObject m_videoLock;

    public void Init(RectTransform sampleTransform, Texture2D tex, bool videoLocked, ISavedWorkData swd = null)
    {
        if (!m_materialInited)
        {
            m_image.material = new Material(Shader.Find("Custom/GreyTextureShader"));
            m_filterImage.material = new Material(Shader.Find("Custom/TilingShader"));
            m_materialInited = true;
        }
        m_sampleTransform = sampleTransform;
        m_image.texture = tex;
        if (swd != null)
        {
            var texture2D = new Texture2D(1, 1, TextureFormat.ARGB32, false);
            texture2D.filterMode = (FilterMode)(swd.ImageInfo.Is3D ? 1 : 0);
            texture2D.LoadImage(swd.Preview);
            m_image.material.SetTexture("_ResTex", texture2D);
            m_image.enabled = true;
            var filter = MainManager.Instance.FilterManager.GetFilter(swd.FilterId);
            if (filter != null)
            {
                m_filterImage.material.mainTextureScale = new Vector2((float)texture2D.width, (float)texture2D.height);
                m_filterImage.texture = filter;
                m_filterImage.enabled = true;
                m_filterImage.material.SetFloat("RepeatX", texture2D.width);
                m_filterImage.material.SetFloat("RepeatY", texture2D.height);
            }
            else
            {
                m_filterImage.enabled = false;
            }
        }
        else
        {
            m_filterImage.enabled = false;
            m_image.uvRect = new Rect(0f, 0f, 1f, 1f);
            m_image.material.SetTexture("_ResTex", null);
            m_filterImage.uvRect = m_image.uvRect;
        }
        m_videoLock.SetActive(false);
        gameObject.SetActive(true);
        base.StartCoroutine(FlyCoroutine());
    }

    public void Close()
    {
        gameObject.SetActive(false);
    }


    IEnumerator FlyCoroutine()
    {
        m_content.sizeDelta = m_sampleTransform.rect.max - m_sampleTransform.rect.min;
        m_content.pivot = m_sampleTransform.pivot;
        m_content.position = m_sampleTransform.position;
        var resPivot = new Vector2(0.5f, 1f);
        m_content.pivot = resPivot;
        var deltaPivot = resPivot - m_sampleTransform.pivot; 
        m_content.anchoredPosition += new Vector2(deltaPivot.x * m_sampleTransform.rect.width, deltaPivot.y * m_sampleTransform.rect.height);
        var resPos = Vector2.zero;
        var resSizeDelta = m_content.sizeDelta * 2f;
        var time = 0.2f;
        var speed = (resPos - m_content.anchoredPosition) / time;
        var sizeDeltaSpeed = (resSizeDelta - m_content.sizeDelta) / time;
        yield return null;

        while (true)
        {
            var deltaTime = Mathf.Min(Time.deltaTime, 0.05f);
            time -= deltaTime;
            if (time < 0f)
            {
                m_content.anchoredPosition = resPos;
                m_content.sizeDelta = resSizeDelta;
                yield break;
            } 
            m_content.anchoredPosition += speed * deltaTime; 
            m_content.sizeDelta += sizeDeltaSpeed * deltaTime;
            yield return null;
        }
    }

}

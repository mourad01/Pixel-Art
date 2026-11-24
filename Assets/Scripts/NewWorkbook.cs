

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class NewWorkbook : BaseWindow
{
    [SerializeField] RectTransform m_content;
    [SerializeField] List<RectTransform> m_topPanels;
    [SerializeField] List<RectTransform> m_bottomPanels;
    [SerializeField] Animator m_colorPanelAnimator;
    [SerializeField] ColorsPanel m_colorsPanel;
    [SerializeField] GameObject m_blockPlane;
    [SerializeField] GameObject m_videoWaiter;
    [SerializeField] GameObject m_completedPlane;
    [SerializeField] List<MaskableGraphic> m_completePlateElements;

    protected override string WindowName
    {
        get
        {
            return "gameScreen";
        }
    }

    public override bool EnableBanner
    {
        get
        {
            return true;
        }
    }

    public override void Open()
    {
        base.StartCoroutine(OpenCoroutine());
    }

    public void RemoveVideoWaiter()
    {
        m_videoWaiter.SetActive(false);
    }

    public void Init(List<Color> colors)
    {
        m_colorsPanel.Init(colors);
        if (NewWorkbookManager.Instance.NumberColoring.Completed)
        {
            base.StartCoroutine(CompletePlateAppearCoroutine());
        }
        NewWorkbookManager.Instance.NumberColoring.OnComplete += delegate
        {
            base.StartCoroutine(CompletePlateAppearCoroutine());

            if (DailyGame.IsDailyArt())
            {
                DailyGame.Instance.Complete();
            }
        };
    }
    public void OkButtonClick()
    {
        try
        {
            var filterWindow = WindowManager.Instance.OpenFilterWindow();
            filterWindow.OnOpen = () =>
            {
                NewWorkbookManager.Instance.NumberColoring.gameObject.SetActive(false);
            };
            filterWindow.OnClose = () =>
            {
                NewWorkbookManager.Instance.NumberColoring.gameObject.SetActive(true);
                filterWindow.OnClose = null;
                filterWindow.OnStartOpen = null;
            };
            float zoom = NewWorkbookManager.Instance.CameraManager.Zoom;
            float maxZoom = NewWorkbookManager.Instance.CameraManager.MaxZoom;
            Vector2 cameraPos = NewWorkbookManager.Instance.CameraManager.CameraPos;
            filterWindow.Init(NewWorkbookManager.Instance.SaveWork(true), maxZoom, default(Rect));
            AudioManager.Instance.PlayClick();
            //AdsWrapper.Instance.ShowInterOrRate("end_level");
        }
        catch (System.Exception e)
        {
            Debug.Log(e.Message);
        }
    }

    public void BackButtonClick()
    {
        AnalyticsManager.Instance.BackButtonClicked();
        NewWorkbookManager.Instance.Exit();
        AudioManager.Instance.PlayClick();
    }

    public void CloseCompletedPlateButtonClick()
    {
        m_completedPlane.SetActive(false);
        //AdsWrapper.Instance.ShowInterOrRate("end_level");
        AudioManager.Instance.PlayClick();
        AnalyticsManager.Instance.BackButtonClicked();
    }

    public override bool Close()
    {
        NewWorkbookManager.Instance.Exit();
        return false;
    }
    IEnumerator OpenCoroutine()
    {
        SendActiveScreenEvent();
        yield return new WaitForSeconds(0.1f);

        m_blockPlane.SetActive(false);
        var topPanelsHeight = m_topPanels.Sum((RectTransform a) => a.rect.height);
        var bottomPanelsHeight = m_bottomPanels.Sum((RectTransform a) => a.rect.height);
        m_content.anchoredPosition = new Vector2(0f, (topPanelsHeight - bottomPanelsHeight) / 2f);
        m_content.sizeDelta = new Vector2(0f, topPanelsHeight + bottomPanelsHeight); 
        var sign = (m_content.sizeDelta.y > 0f);
        var time = 0.5f;
        var sizeSpeed = -m_content.sizeDelta / time;
        var posSpeed = -m_content.anchoredPosition / time;
        yield return null;

        while (true)
        {
            var deltaTime = Mathf.Min(0.05f, Time.deltaTime);
            var sizeDelta = sizeSpeed * deltaTime;
            var posDelta = posSpeed * deltaTime; 
            var newSign = (m_content.sizeDelta.y + sizeDelta.y > 0f);
            if (sign != newSign)
            {
                m_content.sizeDelta = Vector2.zero;
                m_content.anchoredPosition = Vector2.zero;
                if (!AppData.SpecPageOpened && AppData.TutorialCompleted)
                {
                    m_colorPanelAnimator.cullingMode = AnimatorCullingMode.AlwaysAnimate;
                }
                yield break;
            } 
            m_content.sizeDelta += sizeDelta; 
            m_content.anchoredPosition += posDelta;

            yield return null;
        }
    }
    IEnumerator CompletePlateAppearCoroutine()
    {
        while (Input.touchCount != 0)
        {
            yield return null;
        }
        m_completedPlane.SetActive(true);
        var alphas = m_completePlateElements.Select(delegate (MaskableGraphic a)
        {
            return a.color.a;
        }).ToList();

        foreach(var completePlateElement in m_completePlateElements)
        {
            completePlateElement.SetAlpha(0);
        } 
        var time = 0.1f;
        var timer = time;

        yield return null;

        while (true)
        {
            timer -= Mathf.Min(Time.deltaTime, 0.05f);
            if (timer < 0f)
            {
                for (int i = 0; i < m_completePlateElements.Count; i++)
                {
                    m_completePlateElements[i].SetAlpha(alphas[i]);
                }
                yield break;
            }
            for (int j = 0; j < m_completePlateElements.Count; j++)
            {
                m_completePlateElements[j].SetAlpha(alphas[j] * (time - timer));
            }

            yield return null;
        }
    }
}

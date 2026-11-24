


using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class NewWorkbook3D : BaseWindow
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
        UnitySingleton<GameController>.Instance.OnComplete += delegate
        {
            base.StartCoroutine(CompletePlateAppearCoroutine());

            if (DailyGame.IsDailyArt())
            {
                DailyGame.Instance.Complete();
            }
        };
        m_colorsPanel.Init(colors);
    }

    public void OkButtonClick()
    {
        UnitySingleton<ProgressManager>.Instance.SaveWork(delegate (SavedWorkData3D swd)
        {
            var shareWindow = WindowManager.Instance.OpenShareWindow();
            shareWindow.Init(swd);
            AudioManager.Instance.PlayClick();
            //AdsWrapper.Instance.ShowInterOrRate("end_level");
        });
    }

    public void BackButtonClick()
    {
        UnitySingleton<ProgressManager>.Instance.SaveWork(delegate
        {
            Exit();
        });
        AnalyticsManager.Instance.BackButtonClicked();
        AudioManager.Instance.PlayClick();
    }

    public void CheatButtonClick()
    {
    }

    public void CloseCompletedPlateButtonClick()
    {
        m_completedPlane.SetActive(false);
        //AdsWrapper.Instance.ShowInterOrRate("end_level");
        AudioManager.Instance.PlayClick();
        AnalyticsManager.Instance.BackButtonClicked();
    }

    public void ShareButtonClick()
    {
        UnitySingleton<ProgressManager>.Instance.SaveWork(delegate (SavedWorkData3D swd)
        {
            var shareWindow = WindowManager.Instance.OpenShareWindow();
            shareWindow.Init(swd);
        });
    }

    public override bool Close()
    {
        UnitySingleton<ProgressManager>.Instance.SaveWork(delegate
        {
            Exit();
        });
        return false;
    }

    public void Exit()
    {
        //AdsWrapper.Instance.ShowInter("gamescreen_exit");
        MainManager.Instance.StartLibrary(MainMenu.LastPage);
    }
    IEnumerator OpenCoroutine()
    {
        SendActiveScreenEvent();
        yield return new WaitForSeconds(0.1f);

        m_blockPlane.SetActive(false);
        //start the content squeezed by the top and bottom panels, then grow it to full size in 0.5s
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
            // overshoot, snap to the final size
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
        //wait for the player to lift all fingers
        while (Input.touchCount != 0)
        {
            yield return null;
        }
        m_completedPlane.SetActive(true);
        //keep the original alphas and fade everything in from 0
        var alphas = m_completePlateElements.Select(delegate (MaskableGraphic a)
        {
            return a.color.a;
        }).ToList();

        foreach (var completePlateElement in m_completePlateElements)
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


using UnityObject = global::UnityEngine.Object;


using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;

public class HelpWindow : BaseWindow
{
    bool m_inited;

    [SerializeField] ScrollRect m_scrollRect;
    [SerializeField] Text m_helpText;
    [SerializeField] Text m_urlPrefab;
    [SerializeField] Text m_textPrefab;
    [SerializeField] HorizontalLayoutGroup m_horizontalGroupPrefab;
    [SerializeField] Transform m_logo;
    [SerializeField] SwipeReceiver m_swipeReceiver;

    protected override string WindowName
    {
        get
        {
            return "helpWindow";
        }
    }

    void Awake()
    {
        m_swipeReceiver.OnSwipeRight += CloseButtonClick;
    }

    public void Init()
    {
        if (!m_inited)
        {
            m_inited = true;
            base.StartCoroutine(SetContentSizeCoroutine());
            m_scrollRect.verticalNormalizedPosition = 1f;
            string text = LocalizationManager.Instance.GetString("help_text");
            var matchCollection = Regex.Matches(text, "(http|ftp|https)://([\\w_-]+(?:(?:\\.[\\w_-]+)+))([\\w.,@?^=%&:/~+#-]*[\\w@?^=%&/~+#-])?");
            for (int i = matchCollection.Count - 1; i >= 0; i--)
            {
                int index = matchCollection[i].Index;
                text = text.Insert(index + matchCollection[i].Length, "</a>");
                text = ((text[index - 1] == '\n') ? text.Insert(index, "<a>") : text.Insert(index, "\n<a>"));
            }
            var parts = text.Split(new string[1] {
                "\n"
            }, StringSplitOptions.RemoveEmptyEntries);
            for (int value2 = 0; value2 < parts.Length; value2++)
            {
                string helpText = string.Empty;

                if (value2 < parts.Length && !parts[value2].Contains("<a>"))
                {
                    helpText = helpText + parts[value2] + "\n";
                }
                if (!string.IsNullOrEmpty(helpText))
                {
                    var lineText = UnityObject.Instantiate(m_helpText);
                    lineText.transform.SetParent(m_helpText.transform.parent);
                    lineText.transform.localScale = Vector2.one;
                    lineText.gameObject.SetActive(true);
                    if (helpText.Contains("<center>"))
                    {
                        lineText.alignment = TextAnchor.UpperCenter;
                        helpText = helpText.Replace("<center>", string.Empty).Replace("</center>", string.Empty);
                    }
                    string line = helpText;
                    var trimChars = new char[1] {
                        '\n'
                    };
                    helpText = (lineText.text = line.Trim(trimChars).Replace("<br>", "\n"));
                    if (LocalizationManager.Instance.CurrentLanguage == SystemLanguage.Arabic)
                    {
                        lineText.alignment = TextAnchor.MiddleRight;
                    }
                }
                if (parts[value2].Contains("<a>"))
                {
                    var match = Regex.Match(parts[value2], "(?<text>[^~]*)<a>(?<url>[^~]+)</a>");
                    var urlText = UnityObject.Instantiate(m_urlPrefab);
                    urlText.transform.SetParent(m_helpText.transform.parent);
                    urlText.transform.localScale = Vector2.one;
                    urlText.gameObject.SetActive(true);
                    string url = match.Groups["url"].Value;
                    if (url.Contains("<center>"))
                    {
                        urlText.alignment = TextAnchor.UpperCenter;
                        url = url.Replace("<center>", string.Empty).Replace("</center>", string.Empty);
                    }
                    urlText.text = url;
                    ((Component)urlText).GetComponent<Button>().onClick.AddListener(delegate
                    {
                        Application.OpenURL(url);
                    });
                }
            }
            m_logo.SetAsLastSibling();
        }
    }

    public void CloseButtonClick()
    {
        WindowManager.Instance.CloseMe(this);
        AnalyticsManager.Instance.BackButtonClicked();
        AudioManager.Instance.PlayClick();
    }

    public void TermsAndConditionsButtonClick()
    {
    }

    void OnDestroy()
    {
        m_swipeReceiver.OnSwipeRight -= CloseButtonClick;
    }
    IEnumerator SetContentSizeCoroutine()
    {
        yield return null;
        m_scrollRect.gameObject.SetActive(false);
        m_scrollRect.gameObject.SetActive(true);
        m_scrollRect.verticalNormalizedPosition = 1f;
    }
}

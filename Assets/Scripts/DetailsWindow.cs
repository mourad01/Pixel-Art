using UnityObject = global::UnityEngine.Object;


using System;
using System.Collections;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;

using UnityEngine.UI;

public class DetailsWindow : BaseWindow
{
    bool m_inited;

    [SerializeField] ScrollRect m_scrollRect;
    [SerializeField] Text m_detailsText;
    [SerializeField] Text m_urlPrefab;
    [SerializeField] Text m_textPrefab;
    [SerializeField] HorizontalLayoutGroup m_horizontalGroupPrefab;
    [SerializeField] Transform m_logo;

    string _price1;
    string _price2;
    string _price3;

    protected override string WindowName
    {
        get
        {
            return "detailsWindow";
        }
    }

    /*public void Init(Product[] _products)
    {
        if (_products.Count() > 0)
        {
            this._price1 = _products[0].metadata.localizedPriceString;
        }
        if (_products.Count() >= 1)
        {
            this._price2 = _products[1].metadata.localizedPriceString;
        }
        if (_products.Count() >= 2)
        {
            this._price3 = _products[2].metadata.localizedPriceString;
        }
        this.Init();
    }*/

    public void Init()
    {
        if (!m_inited)
        {
            m_inited = true;
            base.StartCoroutine(SetContentSizeCoroutine());
            m_scrollRect.verticalNormalizedPosition = 1f;
            string @string = LocalizationManager.Instance.GetString("details_text_android");
            @string = string.Format(@string, _price1, _price2, _price3);
            var matchCollection = Regex.Matches(@string, "(http|ftp|https)://([\\w_-]+(?:(?:\\.[\\w_-]+)+))([\\w.,@?^=%&:/~+#-]*[\\w@?^=%&/~+#-])?");
            for (int i = matchCollection.Count - 1; i >= 0; i--)
            {
                int index = matchCollection[i].Index;
                @string = @string.Insert(index + matchCollection[i].Length, "</a>");
                @string = ((@string[index - 1] == '\n') ? @string.Insert(index, "<a>") : @string.Insert(index, "\n<a>"));
            }
            var parts = @string.Split(new string[1] {
                "\n"
            }, StringSplitOptions.RemoveEmptyEntries);
            for (int index = 0; index < parts.Length; index++)
            {
                string text = string.Empty;

                if (index < parts.Length && !parts[index].Contains("<a>"))
                {
                    text = text + parts[index] + "\n";
                }
                if (!string.IsNullOrEmpty(text))
                {
                    var lineText = UnityObject.Instantiate(m_detailsText);
                    lineText.transform.SetParent(m_detailsText.transform.parent);
                    lineText.transform.localScale = Vector2.one;
                    lineText.gameObject.SetActive(true);
                    if (text.Contains("<center>"))
                    {
                        lineText.alignment = TextAnchor.UpperCenter;
                        text = text.Replace("<center>", string.Empty).Replace("</center>", string.Empty);
                    }
                    string line = text;
                    var trimChars = new char[1] {
                        '\n'
                    };
                    text = (lineText.text = line.Trim(trimChars).Replace("<br>", "\n"));
                    if (LocalizationManager.Instance.CurrentLanguage == SystemLanguage.Arabic)
                    {
                        lineText.alignment = TextAnchor.MiddleRight;
                    }
                }
                if (parts[index].Contains("<a>"))
                {
                    var match = Regex.Match(parts[index], "(?<text>[^~]*)<a>(?<url>[^~]+)</a>");
                    var urlText = UnityObject.Instantiate(m_urlPrefab);
                    urlText.transform.SetParent(m_detailsText.transform.parent);
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

    IEnumerator SetContentSizeCoroutine()
    {
        yield return null;
        m_scrollRect.gameObject.SetActive(false);
        m_scrollRect.gameObject.SetActive(true);
        m_scrollRect.verticalNormalizedPosition = 1f;
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
}

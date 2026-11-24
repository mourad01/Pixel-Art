using UnityObject = global::UnityEngine.Object;


using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

using UnityEngine.UI;

public class NewInappsWindow : BaseWindow
{
    public Action OnPurchase;

    bool restoredAnything;

    [SerializeField] GameObject m_mainPart;
    [SerializeField] NewInappsWindowElement m_prefab;
    [SerializeField] NewInappsWindowElement m_trialInappsElement;
    [SerializeField] Transform m_elementsParent;
    [SerializeField] List<NewInappsWindowElement> m_elements;
    [SerializeField] GameObject m_resultPart;
    [SerializeField] Text m_resultTitle;
    [SerializeField] Text m_resultText;
    [SerializeField] Image m_resultIcon;
    [SerializeField] Sprite m_goodResultSprite;
    [SerializeField] Sprite m_badResultSprite;
    [SerializeField] GameObject m_closeButton;
    [SerializeField] Popup m_popup;
    [SerializeField] GameObject m_waiter;
    [SerializeField] ScrollRect m_textScrollRect;
    [SerializeField] Text m_detailsText;
    [SerializeField] Text m_urlPrefab;

    bool m_textInited;
    bool m_waitPurchases;
    Action m_closeHandler;
    string m_place;



    protected override string WindowName
    {
        get
        {
            return "inappsWindow";
        }
    }

    public void Init(string place, Action closeHandler = null)
    {
        m_place = place;
        m_closeHandler = closeHandler;
        /*Product[] products = IAPWrapper.Instance.GetProducts();
        if (products != null)
        {
            this.InitProducts(products);
        }
        else
        {
            this.m_trialInappsElement.gameObject.SetActive(false);
            this.m_waiter.SetActive(true);
            this.m_waitPurchases = true;
        }
        this.m_mainPart.SetActive(true);
        this.m_resultPart.SetActive(false);
        this.Open();
        this.InitTexts();
        AnalyticsManager.Instance.InappsWindowOpened(this.m_place);*/
    }

    void InitTexts()
    {
        if (!m_textInited)
        {
            m_textInited = true;
            base.StartCoroutine(SetContentSizeCoroutine());
            m_textScrollRect.verticalNormalizedPosition = 1f;
            string text = LocalizationManager.Instance.GetString("details_text_android");
            //wrap every url in <a></a> on its own line, from the end so indexes stay valid
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
            for (int index = 0; index < parts.Length; index++)
            {
                string detailsText = string.Empty;

                if (index < parts.Length && !parts[index].Contains("<a>"))
                {
                    detailsText = detailsText + parts[index] + "\n";
                }
                //plain line, clone the details text for it
                if (!string.IsNullOrEmpty(detailsText))
                {
                    var lineText = UnityObject.Instantiate(m_detailsText);
                    lineText.transform.SetParent(m_detailsText.transform.parent);
                    lineText.transform.localScale = Vector2.one;
                    lineText.gameObject.SetActive(true);
                    if (detailsText.Contains("<center>"))
                    {
                        lineText.alignment = TextAnchor.UpperCenter;
                        detailsText = detailsText.Replace("<center>", string.Empty).Replace("</center>", string.Empty);
                    }
                    string line = detailsText;
                    var trimChars = new char[1] {
                        '\n'
                    };
                    detailsText = (lineText.text = line.Trim(trimChars).Replace("<br>", "\n"));
                    if (LocalizationManager.Instance.CurrentLanguage == SystemLanguage.Arabic)
                    {
                        lineText.alignment = TextAnchor.MiddleRight;
                    }
                }
                // link line, make a clickable url text
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
        }
    }

    void Update()
    {
        /*if (this.m_waitPurchases && IAPWrapper.Instance.GetProducts() != null)
        {
            this.m_waitPurchases = false;
            this.InitProducts(IAPWrapper.Instance.GetProducts());
        }*/
    }

    /*private void InitProducts(Product[] products)
    {
        this._products = products;
        this.m_waitPurchases = false;
        this.m_waiter.SetActive(false);
        int i = 0;
        for (int j = 0; j < products.Length; j++)
        {
            if (products[j].availableToPurchase && products[j].definition.type == ProductType.Subscription)
            {
                Product product = products[j];
                if (i >= this.m_elements.Count)
                {
                    NewInappsWindowElement newInappsWindowElement = null; 
                    newInappsWindowElement = ((!(products[j].definition.id != SubscriptionType.one_week_subscription.ToString())) ? this.m_trialInappsElement : UnityObject.Instantiate(this.m_prefab));
                    newInappsWindowElement.transform.SetParent(this.m_elementsParent);
                    newInappsWindowElement.transform.localScale = Vector2.one;
                    newInappsWindowElement.transform.localRotation = Quaternion.identity;
                    NewInappsWindowElement newInappsWindowElement2 = newInappsWindowElement;
                    newInappsWindowElement2.OnSubscribe = (Action<Product>)Delegate.Combine(newInappsWindowElement2.OnSubscribe, new Action<Product>(this.OnSubscribeHandler));
                    this.m_elements.Add(newInappsWindowElement);
                }
                this.m_elements[i].gameObject.SetActive(true);
                if (products[j].definition.id == SubscriptionType.one_week_subscription.ToString())
                {
                    this.m_elements[i++].InitAsTrial(product);
                }
                else
                {
                    this.m_elements[i++].Init(product);
                }
            }
        }
        for (; i < this.m_elements.Count; i++)
        {
            this.m_elements[i].gameObject.SetActive(false);
        }
    }
    */
    /*private void OnSubscribeHandler(Product product)
    {
        SubscriptionType subscriptionType = (SubscriptionType)Enum.Parse(typeof(SubscriptionType), product.definition.id);
        AnalyticsManager.Instance.SubscriptionClick(subscriptionType);
        IAPWrapper.Instance.BuyProduct(subscriptionType, this.m_place, delegate (bool res, SubscriptionType type, PurchaseFailureReason reason)
        {
            if (!res && reason == PurchaseFailureReason.UserCancelled)
            {
                return;
            }
            if (res)
            {
                this.Init(this.m_place, this.m_closeHandler);
                this.OnPurchase.SafeInvoke();
                this.m_closeButton.SetActive(true);
                this.CloseButtonClick();
            }
            else
            {
                this.m_mainPart.SetActive(false);
                this.m_resultPart.SetActive(true);
                this.m_resultTitle.text = LocalizationManager.Instance.GetString((!res) ? "oops" : "thanks");
                this.m_resultText.text = LocalizationManager.Instance.GetString((!res) ? "BuyError" : "BuyConfirmation");
                this.m_resultIcon.sprite = ((!res) ? this.m_badResultSprite : this.m_goodResultSprite);
            }
        });
        AudioManager.Instance.PlayClick();
        switch (subscriptionType)
        {
            case SubscriptionType.one_month_subscription:
                AnalyticsManager.Instance.InappsWindowAction("month", this.m_place);
                break;
            case SubscriptionType.one_week_subscription:
                AnalyticsManager.Instance.InappsWindowAction("week", this.m_place);
                break;
            case SubscriptionType.one_year_subscription:
                AnalyticsManager.Instance.InappsWindowAction("year", this.m_place);
                break;
        }
    }*/

    public void TermsButtonClick()
    {
        Application.OpenURL(AppPathsConfig.TermsUrl);
    }

    public void PrivacyButtonClick()
    {
        Application.OpenURL(AppPathsConfig.PrivacyUrl);
    }

    public void CloseButtonClick()
    {
        WindowManager.Instance.CloseMe(this);
        AnalyticsManager.Instance.BackButtonClicked();
        AudioManager.Instance.PlayClick();
        AnalyticsManager.Instance.InappsWindowAction("close", m_place);
    }

    public void DetailsButtonClick()
    {
        var detailsWindow = WindowManager.Instance.OpenDetailsWindow();
        //detailsWindow.Init(this._products);
        AudioManager.Instance.PlayClick();
    }

    public override bool Close()
    {
        //can't close while the close button is hidden
        if (m_closeButton.activeSelf)
        {
            AnalyticsManager.Instance.InappsWindowClosed();
            m_closeHandler.SafeInvoke();
            return base.Close();
        }
        return false;
    }

    public void RestoreButtonClick()
    {
        //IAPWrapper instance = IAPWrapper.Instance;
        //instance.OnPurchase = (Action<bool, SubscriptionType>)Delegate.Combine(instance.OnPurchase, new Action<bool, SubscriptionType>(this.OnPurchaseHandler));
        base.StartCoroutine(WaitForRestored());
        //IAPWrapper.Instance.RestorePurchases(null);
        AnalyticsManager.Instance.RestoreButtonClicked();
        AudioManager.Instance.PlayClick();
        AnalyticsManager.Instance.InappsWindowAction("restore", m_place);
    }

    void OnPurchaseHandler(bool res, SubscriptionType type)
    {
        restoredAnything = true;
        //IAPWrapper instance = IAPWrapper.Instance;
        //instance.OnPurchase = (Action<bool, SubscriptionType>)Delegate.Remove(instance.OnPurchase, new Action<bool, SubscriptionType>(this.OnPurchaseHandler));
    }
    IEnumerator SetContentSizeCoroutine()
    {
        yield return null;
        m_textScrollRect.gameObject.SetActive(false);
        m_textScrollRect.verticalNormalizedPosition = 1f;
    }
    IEnumerator WaitForProductsCoroutine()
    {
        //while (IAPWrapper.Instance.GetProducts() == null)
        {
            yield return new WaitForSeconds(2f); 
        }
        Init(m_place, m_closeHandler);
    }
    IEnumerator WaitForRestored()
    {
        //give the store 2 seconds to report restored purchases
        float timer = 2f;
        while (timer > 0f)
        {
            timer -= Time.deltaTime;
            yield return null;
        }
        //IAPWrapper instance = IAPWrapper.Instance;
        //instance.OnPurchase = (Action<bool, SubscriptionType>)Delegate.Remove(instance.OnPurchase, new Action<bool, SubscriptionType>(this.OnPurchaseHandler));
        if (restoredAnything)
        {
            /*if (IAPWrapper.Instance.Subscribed)
            {
                AnalyticsManager.Instance.PurchasesRestored();
                this.m_popup.Show(LocalizationManager.Instance.GetString("restored1"));
            }
            else
            {
                this.m_popup.Show(LocalizationManager.Instance.GetString("restored2"));
            }*/
        }
        else
        {
            m_popup.Show(LocalizationManager.Instance.GetString("restored0"));
        }
    }
}


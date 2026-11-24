

using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class TranslatableText : MonoBehaviour
{
    [SerializeField] bool m_autoTranslate = true;

    Text m_text;
    BestFit m_bestFit;

    void Start()
    {
        if (m_autoTranslate)
        {
            Translate();
        }
    }

    public void Translate()
    {
        m_text = base.GetComponent<Text>();
        m_bestFit = base.GetComponent<BestFit>();
        m_text.enabled = false;
        if (m_text.text != string.Empty)
        {
            string str = LocalizationManager.Instance.GetString(m_text.text);
            m_text.text = str;
        }
        if (m_bestFit != null)
        {
            m_bestFit.Refit();
        }
        m_text.enabled = true;
        var contentSizeFitter = base.GetComponent<ContentSizeFitter>();
        if (contentSizeFitter != null)
        {
            contentSizeFitter.SetLayoutHorizontal();
        }
    }
}



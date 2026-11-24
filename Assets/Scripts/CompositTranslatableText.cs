

using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class CompositTranslatableText : MonoBehaviour
{
    [SerializeField] bool m_autoTranslate = true;
    [SerializeField] List<string> m_list;
    [SerializeField] string m_format;
    [SerializeField] bool forceUpper;

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
            var args = (from a in m_list
                             select LocalizationManager.Instance.GetString(a)).ToArray();
            string text = string.Format(m_format, args);
            m_text.text = ((!forceUpper) ? text : text.ToUpper());
        }
        if (m_bestFit != null)
        {
            m_bestFit.Refit();
        }
        m_text.enabled = true;
    }
}



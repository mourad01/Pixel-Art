using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
[DisallowMultipleComponent]
public class FontSwitcher : MonoBehaviour
{
    Text m_text;

    void Awake()
    {
        m_text = base.GetComponent<Text>();
        switch (LocalizationManager.Instance.CurrentLanguage)
        {
            case SystemLanguage.Japanese:
            case SystemLanguage.Korean:
            case SystemLanguage.Thai:
            case SystemLanguage.Vietnamese:
            case SystemLanguage.ChineseSimplified:
            case SystemLanguage.ChineseTraditional:
                m_text.font = (Resources.Load("MyriadPro-Regular-dynamic") as Font);
                break;
            case SystemLanguage.Arabic:
                m_text.font = (Resources.Load("MyriadPro-Regular-dynamic") as Font);
                break;
        }
    }

    [ContextMenu("ChangeFont")]
    void ChangeFont()
    {
        m_text = base.GetComponent<Text>();
        if (!m_text.font.name.Contains("helvetica") && !m_text.font.name.Contains("HELVETICA"))
        {
            return;
        }
        m_text.font = (Resources.Load("RobotoCondensed-Regular") as Font);
    }
}



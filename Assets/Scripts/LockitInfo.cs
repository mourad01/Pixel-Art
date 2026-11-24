using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class LockitInfo
{
    Dictionary<string, Dictionary<SystemLanguage, string>> m_dict;

    public int Version { get; set; }

    public string this[string key]
    {
        get
        {
            if (m_dict.ContainsKey(key))
            {
                if (m_dict[key].ContainsKey(LocalizationManager.Instance.CurrentLanguage))
                {
                    string text = m_dict[key][LocalizationManager.Instance.CurrentLanguage];
                    if (!string.IsNullOrEmpty(text))
                    {
                        return text;
                    }
                }
                return m_dict[key][LocalizationManager.Instance.DefaultLanguage];
            }
            return key;
        }
    }

    public LockitInfo()
    {
        m_dict = new Dictionary<string, Dictionary<SystemLanguage, string>>();
    }

    public void Init(Dictionary<string, object> dict)
    {
        if (dict != null)
        {
            foreach (object value2 in dict.Values)
            {
                var dictionary = value2 as Dictionary<string, object>;
                string key = string.Empty;
                foreach (string key6 in dictionary.Keys)
                {
                    switch (key6)
                    {
                        case "key":
                            key = (string)dictionary[key6];
                            m_dict.Add(key, new Dictionary<SystemLanguage, string>());
                            break;
                        default:
                            {
                                var systemLanguage = (SystemLanguage)Enum.Parse(typeof(SystemLanguage), key6);
                                string value = (string)dictionary[key6];
                                m_dict[key].Add(systemLanguage, value);
                                break;
                            }
                        case "id":
                            break;
                    }
                }
            }
            string key3 = "help_text";
            string key4 = "how_to_cancel_android";
            if (m_dict.ContainsKey(key3))
            {
                var keys = m_dict[key3].Keys.ToList();
                for (int i = 0; i < keys.Count; i++)
                {
                    var key5 = keys[i];
                    m_dict[key3][key5] = m_dict[key3][key5].Replace("<how_to_cancel/>", m_dict[key4][key5]);
                }
            }
        }
    }
}



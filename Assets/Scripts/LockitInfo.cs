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
            foreach (object entry in dict.Values)
            {
                var dictionary = entry as Dictionary<string, object>;
                string key = string.Empty;
                // each row has a key, an id and one field per SystemLanguage name
                foreach (string field in dictionary.Keys)
                {
                    switch (field)
                    {
                        case "key":
                            key = (string)dictionary[field];
                            m_dict.Add(key, new Dictionary<SystemLanguage, string>());
                            break;
                        default:
                            {
                                var systemLanguage = (SystemLanguage)Enum.Parse(typeof(SystemLanguage), field);
                                string value = (string)dictionary[field];
                                m_dict[key].Add(systemLanguage, value);
                                break;
                            }
                        case "id":
                            break;
                    }
                }
            }
            //put the how to cancel text inside the help text
            string helpKey = "help_text";
            string cancelKey = "how_to_cancel_android";
            if (m_dict.ContainsKey(helpKey))
            {
                var keys = m_dict[helpKey].Keys.ToList();
                for (int i = 0; i < keys.Count; i++)
                {
                    var locale = keys[i];
                    m_dict[helpKey][locale] = m_dict[helpKey][locale].Replace("<how_to_cancel/>", m_dict[cancelKey][locale]);
                }
            }
        }
    }
}



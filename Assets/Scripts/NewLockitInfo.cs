
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;

[Serializable]
public class NewLockitInfo
{
    Dictionary<string, Dictionary<string, string>> m_dict;

    public string this[string key]
    {
        get
        {
            if (m_dict.ContainsKey(key))
            {
                string locale = LocalizationManager.Instance.CurrentLocale;
                locale = locale.Replace("_", "-");
                if (m_dict[key].ContainsKey(locale))
                {
                    string text = m_dict[key][locale];
                    if (!string.IsNullOrEmpty(text))
                    {
                        //arabic texts come with reversed tags and urls, fix them back
                        if (locale == "ar")
                        {
                            text = text.Replace("[[[", string.Empty);
                            text = text.Replace("]]]", string.Empty);
                            //text = ArabicFixer.Fix(text, false, false);
                            text = text.Replace(">/b<", "<b>");
                            text = text.Replace(">b<", "</b>");
                            text = text.Replace("//:https", "https://");
                        }
                        return text;
                    }
                }
                // no exact match, try shorter locales like pt-BR then pt
                while (locale.Length > 3)
                {
                    if (!locale.Contains("-"))
                    {
                        locale = locale.Substring(0, 2);
                    }
                    else
                    {
                        locale = locale.Substring(0, locale.LastIndexOf("-"));
                    }
                    string localeKey = m_dict[key].Keys.FirstOrDefault((string a) => a.StartsWith(locale));
                    if (localeKey == null)
                    {
                        continue;
                    }
                    string translation = m_dict[key][localeKey];
                    if (string.IsNullOrEmpty(translation))
                    {
                        continue;
                    }
                    if (!(locale == "ar"))
                    {
                        return translation;
                    }
                    translation = translation.Replace("[[[", string.Empty);
                    translation = translation.Replace("]]]", string.Empty);
                    //text3 = ArabicFixer.Fix(text3, false, false);
                    translation = translation.Replace(">/b<", "<b>");
                    translation = translation.Replace(">b<", "</b>");
                    translation = translation.Replace("//:https", "https://");
                    break;
                }
                //last try is the default locale, else return the key itself
                locale = LocalizationManager.Instance.DefaultLocale;
                if (m_dict[key].ContainsKey(locale))
                {
                    string translation = m_dict[key][locale];
                    if (!string.IsNullOrEmpty(translation))
                    {
                        return translation;
                    }
                }
            }
            return key;
        }
    }

    //public NewLockitInfo(Dictionary<string, Dictionary<string, string>> dict)
    //{
    //    this.m_dict = dict;
    //}

    public NewLockitInfo(string text)
    {
        m_dict = new Dictionary<string, Dictionary<string, string>>();

        using (StringReader sw = new StringReader(text))
        {
            var doc = new XmlDocument();
            doc.LoadXml(text);

            // each TextPair holds a key and one Text per locale
            var pairs = doc.GetElementsByTagName("TextPair");
            foreach (XmlElement pair in pairs)
            {
                var key = (pair.GetElementsByTagName("Key")[0] as XmlElement).InnerText;
                var values = pair.GetElementsByTagName("Value")[0] as XmlElement;

                var t = new Dictionary<string, string>();
                foreach (XmlElement n in values.GetElementsByTagName("Text"))
                {
                    var textKey = (n.GetElementsByTagName("Key")[0] as XmlElement).InnerText;
                    var textValue = (n.GetElementsByTagName("Value")[0] as XmlElement).InnerText;

                    t[textKey] = textValue;
                }

                m_dict[key] = t;
            }
        }
    }
}



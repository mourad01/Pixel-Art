using UnityEngine;

public class LocalizationManager
{
    static LocalizationManager s_instance;

    LockitInfo m_lockitInfo;
    NewLockitInfo m_newLockitInfo;
    string m_fileName = string.Empty;

    public static LocalizationManager Instance
    {
        get
        {
            if (s_instance == null)
            {
                s_instance = new LocalizationManager();
            }
            return s_instance;
        }
    }

    public SystemLanguage CurrentLanguage { get; private set; }

    public SystemLanguage DefaultLanguage
    {
        get
        {
            return SystemLanguage.English;
        }
    }

    public string CurrentLocale { get; private set; }

    public string DefaultLocale
    {
        get
        {
            return "en-US";
        }
    }

    public void Init(string text)
    {
        CurrentLanguage = Application.systemLanguage;
        CurrentLocale = SystemToolsWrapper.GetLocale();
        m_newLockitInfo = new NewLockitInfo(text);


        //DataManager.Instance.GetNewLockit(delegate (NewLockitInfo lockitInfo)
        //{
        //	this.m_newLockitInfo = lockitInfo;
        //	var s = new SerializableDictionary<string, Dictionary<string, string>>();
        //	foreach(var kv in lockitInfo.m_dict)
        //	{
        //		s[kv.Key] = kv.Value;
        //	}

        //	//DataContractSerializer serializer = new DataContractSerializer(lockitInfo.m_dict.GetType());

        //	using (StringWriter sw = new StringWriter())
        //	{
        //		using (XmlTextWriter writer = new XmlTextWriter(sw))
        //		{
        //			// add formatting so the XML is easy to read in the log
        //			writer.Formatting = Formatting.Indented;

        //			s.WriteXml(writer);
        //			//serializer.WriteObject(writer, lockitInfo.m_dict);

        //			writer.Flush();

        //		}
        //	}
        //});
    }

    public string GetString(string key)
    {
        try
        {
            key = key.Trim('\r', '\n');
            return m_newLockitInfo[key].Trim('\r', '\n');
        }
        catch
        {
            return key;
        }
    }
}



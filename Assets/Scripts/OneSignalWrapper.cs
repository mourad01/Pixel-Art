using UnityDebug = global::UnityEngine.Debug;


using System.Collections.Generic;

public class OneSignalWrapper
{
    static Dictionary<string, object> startParameters;
    static bool m_request;
    static string m_userId = string.Empty;
    static bool s_inited; 

    public static string UserId
    {
        get
        {
            if (m_userId != string.Empty)
            {
                return m_userId;
            }
            if (s_inited && !m_request)
            {
                m_request = true;
                OneSignal.IdsAvailable(delegate (string userId, string pushToken)
                {
                    m_request = false;
                    m_userId = userId;
                    //DataManager.Instance.SendPushToken(null);
                });
            }
            return string.Empty;
        }
    }

    public static void Init(string appId, string googleProjectId)
    {
        if (!s_inited)
        {
            OneSignal.StartInit(appId, googleProjectId).HandleNotificationOpened(new OneSignal.NotificationOpened(NotificationHandler)).EndInit();
            string userId = UserId;
        }
    }

    static void NotificationHandler(OSNotificationOpenedResult res)
    {
        try
        {
            if (!res.notification.isAppInFocus)
            {
                startParameters = res.notification.payload.additionalData;
                string text = string.Empty;
                foreach (KeyValuePair<string, object> additionalDatum in res.notification.payload.additionalData)
                {
                    text = text + "[" + additionalDatum.Key + "] = " + additionalDatum.Value + "\r\n";
                }
                UnityDebug.Log("OneSignalNotification: \r\n" + text);
            }
        }
        catch
        {
        }
    }
}



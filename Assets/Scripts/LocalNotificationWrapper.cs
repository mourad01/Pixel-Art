using UnityObject = global::UnityEngine.Object;


using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

internal class LocalNotificationWrapper : MonoBehaviour
{
    static LocalNotificationWrapper s_instance;

    bool notificationsCreated;
    List<int> m_currentNotificationsIds = new List<int>();

    public static LocalNotificationWrapper Instance
    {
        get
        {
            if (s_instance == null)
            {
                var gameObject = new GameObject("LocalNotificationWrapper");
                s_instance = gameObject.AddComponent<LocalNotificationWrapper>();
                UnityObject.DontDestroyOnLoad(gameObject);
            }
            return s_instance;
        }
    }

    public void Init()
    {
        CancelAll();
    }

    public void Create(int id, DateTime dt, string title, string message, bool repeat = false, IDictionary dict = null)
    {
        Create(id, dt, (dt - DateTime.Now).TotalSeconds, title, message, repeat, dict);
    }

    public void Create(int id, double seconds, string title, string message, bool repeat = false, IDictionary dict = null)
    {
        DateTime dt = DateTime.Now + new TimeSpan((long)seconds * 10000000);
        Create(id, dt, seconds, title, message, repeat, dict);
    }

    public void Create(int id, DateTime dt, double seconds, string title, string message, bool repeat = false, IDictionary dict = null)
    {
        if (repeat)
        {
            LocalNotification.SendRepeatingNotification(id, TimeSpan.FromSeconds(seconds), TimeSpan.FromDays(1), title, message, Color.white);
        }
        else
        {
            LocalNotification.SendNotification(id, TimeSpan.FromSeconds(seconds), title, message, Color.white);
        } 
    }

    public void CancelAll()
    {
        if (m_currentNotificationsIds.Count == 0)
        {
            var localNotifications = AppData.LocalNotifications;
            if (localNotifications != null)
            {
                m_currentNotificationsIds = localNotifications;
            }
            AppData.LocalNotifications = null;
        }
        foreach (int currentNotificationsId in m_currentNotificationsIds)
        {
            LocalNotification.CancelNotification(currentNotificationsId); 
        }
        m_currentNotificationsIds.Clear();
    }

    public void OnApplicationPause(bool val)
    {
        if (!val)
        {
            CancelAll();
            notificationsCreated = false;
        }
        else
        {
            ApplicationQuitHandler();
            notificationsCreated = true;
        }
    }

    void ApplicationQuitHandler()
    {
        if (!notificationsCreated)
        {
            try
            {
                var random = new System.Random();
                int notificationId = 0;

                    DateTime dateTime = DateTime.Now;
                    dateTime += new TimeSpan(3, 0, 0);
                    if (dateTime.Hour >= 9 && dateTime.Hour < 21)
                    {
                        notificationId = random.Next(0, 2147483647);
                        m_currentNotificationsIds.Add(notificationId);
                        Create(notificationId, dateTime, "Pixel Art", LocalizationManager.Instance.GetString("ntfy_title") + '\n' + LocalizationManager.Instance.GetString("ntfy_body"), false, null);
                    }

                AppData.LocalNotifications = m_currentNotificationsIds;
            }
            catch (Exception ex)
            {
                Debug.Log(ex.Message);
            }
        }
    }
}



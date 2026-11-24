using UnityDebug = global::UnityEngine.Debug;


using System;
using System.Collections.Generic;

public static class AppData
{
    static string s_uidKey = "uid"; 
    static string s_appRatedKey = "app_rated";
    static string s_sessionIdKey = "session_id";
    static string s_oneSignalIdKey = "onesignal_id";
    static string s_rateUsShowsKey = "last_rateus_shows";
    static string s_localNotificationsKey = "local_notifs";
    static string s_purhasesKey = "purchases";
    static string s_freeImagesCounterKey = "free_images_counter";
    static string s_videoImagesKey = "video_images";
    static string s_vibroKey = "vibro";
    static string s_vibroRightKey = "vibro_right";
    static string s_vibroWrongKey = "vibro_wrong";

    public static string s_soundsKey = "sounds";

    static string s_tutorialKey = "tutorial";
    static string s_tutorialKey3D = "tutorial3D";
    static string s_lassoCountKey = "lassos";
    static string s_bombCountKey = "bombs";
    static string s_prevHostKey = "prev_host";
    static string s_isOldUserKey = "is_old_user";
    static string s_bulbModeKey = "bulb_mode";
    static string s_loupeKey = "loupe";
    static string s_noAdsKey = "no_ads";
    static string s_specPageOpenedKey = "spec_page";
    static string s_unlockedImages = "unlocked_images";
    static string s_pictureDoneKey = "picture_done";
    static string s_pictureDoneCountKey = "picture_done_count";
    static string s_launchNextDayKey = "launch_next_day";
    static string s_launchNext2DaysKey = "launch_next2_days";
    static string s_endSessionTime = "end_session_time";
    static string s_lastDayTracked = "session_tracked";
    static string s_freePhotos = "free_photos";
    static string s_firstLaunch = "first_launch";
    static string s_intersCount = "inters_count";
    static string s_secondsInGame = "seconds_count";
    static string s_weekSecondsTracked = "week_sec_tracked";
    static string s_subCampaign = "sub_campaign";
    static string s_updateToNew = "udpate_to_new";

    public static bool Inited { get; private set; }

    public static string Uid
    {
        get
        {
            string text = PlayerPrefsWrapper.GetString(s_uidKey);
            if (string.IsNullOrEmpty(text))
            {
                text = Guid.NewGuid().ToString();
                PlayerPrefsWrapper.SetString(s_uidKey, text);
            }
            return text;
        }
    } 

    public static bool AppRated
    {
        get
        {
            return PlayerPrefsWrapper.GetBool(s_appRatedKey);
        }
        set
        {
            PlayerPrefsWrapper.SetBool(s_appRatedKey, value);
        }
    }

    public static int SessionId
    {
        get
        {
            return PlayerPrefsWrapper.GetInt(s_sessionIdKey);
        }
        private set
        {
            PlayerPrefsWrapper.SetInt(s_sessionIdKey, value);
        }
    }

    public static string OneSignalId
    {
        get
        {
            return PlayerPrefsWrapper.GetString(s_oneSignalIdKey);
        }
        set
        {
            PlayerPrefsWrapper.SetString(s_oneSignalIdKey, value);
        }
    }

    public static List<DateTime> LastRateUsShows
    {
        get
        {
            var stringList = PlayerPrefsWrapper.GetStringList(s_rateUsShowsKey);
            var list = new List<DateTime>();
            foreach (string item in stringList)
            {
                list.Add(DateTime.Parse(item));
            }
            return list;
        }
        private set
        {
            if (value != null)
            {
                var list = new List<string>();
                foreach (DateTime item in value)
                {
                    list.Add(item.ToString());
                }
                PlayerPrefsWrapper.SetStringList(s_rateUsShowsKey, list);
            }
        }
    }

    public static List<int> LocalNotifications
    {
        get
        {
            var stringList = PlayerPrefsWrapper.GetStringList(s_localNotificationsKey);
            var list = new List<int>();
            foreach (string item in stringList)
            {
                list.Add(int.Parse(item));
            }
            return list;
        }
        set
        {
            if (value != null)
            {
                var list = new List<string>();
                foreach (int item in value)
                {
                    list.Add(item.ToString());
                }
                PlayerPrefsWrapper.SetStringList(s_localNotificationsKey, list);
            }
        }
    }

    public static List<string> Purchases
    {
        get
        {
            return PlayerPrefsWrapper.GetStringList(s_purhasesKey);
        }
        set
        {
            PlayerPrefsWrapper.SetStringList(s_purhasesKey, value);
        }
    }

    public static int FreeImagesCounter
    {
        get
        {
            return PlayerPrefsWrapper.GetInt(s_freeImagesCounterKey);
        }
        set
        {
            PlayerPrefsWrapper.SetInt(s_freeImagesCounterKey, value);
        }
    }

    public static List<string> VideoImages
    {
        get
        {
            return PlayerPrefsWrapper.GetStringList(s_videoImagesKey);
        }
        set
        {
            PlayerPrefsWrapper.SetStringList(s_videoImagesKey, value);
        }
    }

    static bool VibroEnabled
    {
        get
        {
            return PlayerPrefsWrapper.GetBool(s_vibroKey);
        }
        set
        {
            PlayerPrefsWrapper.SetBool(s_vibroKey, value);
        }
    }

    public static bool VibroRightEnabled
    {
        get
        {
            return PlayerPrefsWrapper.GetBool(s_vibroRightKey);
        }
        set
        {
            PlayerPrefsWrapper.SetBool(s_vibroRightKey, value);
        }
    }

    public static bool VibroWrongEnabled
    {
        get
        {
            return PlayerPrefsWrapper.GetBool(s_vibroWrongKey);
        }
        set
        {
            PlayerPrefsWrapper.SetBool(s_vibroWrongKey, value);
        }
    }

    public static bool SoundsEnabled
    {
        get
        {
            return PlayerPrefsWrapper.GetBool(s_soundsKey);
        }
        set
        {
            PlayerPrefsWrapper.SetBool(s_soundsKey, value);
        }
    }

    public static bool TutorialCompleted
    {
        get
        {
            return PlayerPrefsWrapper.GetBool(s_tutorialKey);
        }
        set
        {
            PlayerPrefsWrapper.SetBool(s_tutorialKey, value);
        }
    }

    //public static bool TutorialCompleted3D
    //{
    //	get
    //	{
    //		return PlayerPrefsWrapper.GetBool(AppData.s_tutorialKey3D);
    //	}
    //	set
    //	{
    //		PlayerPrefsWrapper.SetBool(AppData.s_tutorialKey3D, value);
    //	}
    //}

    public static int LassoCount
    {
        get
        {
            return PlayerPrefsWrapper.GetInt(s_lassoCountKey);
        }
        set
        {
            PlayerPrefsWrapper.SetInt(s_lassoCountKey, value);
        }
    }

    public static int BombCount
    {
        get
        {
            return PlayerPrefsWrapper.GetInt(s_bombCountKey);
        }
        set
        {
            PlayerPrefsWrapper.SetInt(s_bombCountKey, value);
        }
    } 

    public static bool IsOldUser
    {
        get
        {
            return PlayerPrefsWrapper.GetBool(s_isOldUserKey);
        }
        set
        {
            PlayerPrefsWrapper.SetBool(s_isOldUserKey, value);
        }
    }

    public static bool BulbMode
    {
        get
        {
            return PlayerPrefsWrapper.GetBool(s_bulbModeKey);
        }
        set
        {
            PlayerPrefsWrapper.SetBool(s_bulbModeKey, value);
        }
    }

    public static bool LoupeEnabled
    {
        get
        {
            return PlayerPrefsWrapper.GetBool(s_loupeKey);
        }
        set
        {
            PlayerPrefsWrapper.SetBool(s_loupeKey, value);
        }
    }

    public static bool NoAds
    {
        get
        {
            return PlayerPrefsWrapper.GetBool(s_noAdsKey);
        }
        set
        {
            PlayerPrefsWrapper.SetBool(s_noAdsKey, value);
        }
    }

    public static bool SpecPageOpened
    {
        get
        {
            return PlayerPrefsWrapper.GetBool(s_specPageOpenedKey);
        }
        set
        {
            PlayerPrefsWrapper.SetBool(s_specPageOpenedKey, value);
        }
    }

    public static List<string> UnlockedImages
    {
        get
        {
            return PlayerPrefsWrapper.GetStringList(s_unlockedImages);
        }
        set
        {
            PlayerPrefsWrapper.SetStringList(s_unlockedImages, value);
        }
    }

    public static bool PictureCompleted
    {
        get
        {
            return PlayerPrefsWrapper.GetBool(s_pictureDoneKey);
        }
        set
        {
            PlayerPrefsWrapper.SetBool(s_pictureDoneKey, value);
        }
    }

    public static int PictureCompletedCount
    {
        get
        {
            return PlayerPrefsWrapper.GetInt(s_pictureDoneCountKey);
        }
        set
        {
            PlayerPrefsWrapper.SetInt(s_pictureDoneCountKey, value);
        }
    }

    public static bool LaunchNextDay
    {
        get
        {
            return PlayerPrefsWrapper.GetBool(s_launchNextDayKey);
        }
        set
        {
            PlayerPrefsWrapper.SetBool(s_launchNextDayKey, value);
        }
    }

    public static bool LaunchNext2Days
    {
        get
        {
            return PlayerPrefsWrapper.GetBool(s_launchNext2DaysKey);
        }
        set
        {
            PlayerPrefsWrapper.SetBool(s_launchNext2DaysKey, value);
        }
    }

    public static DateTime EndSessionTime
    {
        get
        {
            string str = PlayerPrefsWrapper.GetString(s_endSessionTime);
            if (!string.IsNullOrEmpty(str))
            {
                return DateTime.Parse(str);
            }
            return DateTime.MinValue;
        }
        set
        {
            PlayerPrefsWrapper.SetString(s_endSessionTime, value.ToString());
        }
    }

    public static DateTime LastDayTracked
    {
        get
        {
            string @string = PlayerPrefsWrapper.GetString(s_lastDayTracked);
            if (!string.IsNullOrEmpty(@string))
            {
                return DateTime.Parse(@string);
            }
            return DateTime.MinValue;
        }
        set
        {
            PlayerPrefsWrapper.SetString(s_lastDayTracked, value.ToString());
        }
    }

    public static int FreePhotos
    {
        get
        {
            return 2;
        }
        set
        {
            PlayerPrefsWrapper.SetInt(s_freePhotos, value);
        }
    }

    public static DateTime FirstLaunch
    {
        get
        {
            string @string = PlayerPrefsWrapper.GetString(s_firstLaunch);
            if (!string.IsNullOrEmpty(@string))
            {
                return DateTime.Parse(@string);
            }
            return DateTime.MinValue;
        }
        set
        {
            PlayerPrefsWrapper.SetString(s_firstLaunch, value.ToString());
        }
    }

    public static int IntersCount
    {
        get
        {
            return PlayerPrefsWrapper.GetInt(s_intersCount);
        }
        set
        {
            PlayerPrefsWrapper.SetInt(s_intersCount, value);
        }
    }

    public static int SecondsInGame
    {
        get
        {
            return PlayerPrefsWrapper.GetInt(s_secondsInGame);
        }
        set
        {
            PlayerPrefsWrapper.SetInt(s_secondsInGame, value);
        }
    }

    public static bool WeekSecondsTracked
    {
        get
        {
            return PlayerPrefsWrapper.GetBool(s_weekSecondsTracked);
        }
        set
        {
            PlayerPrefsWrapper.SetBool(s_weekSecondsTracked, value);
        }
    }

    public static bool SubCampaignWasShown
    {
        get
        {
            return PlayerPrefsWrapper.GetBool(s_subCampaign);
        }
        set
        {
            PlayerPrefsWrapper.SetBool(s_subCampaign, value);
        }
    }

    public static bool UpdateToNew
    {
        get
        {
            return PlayerPrefsWrapper.GetBool(s_updateToNew);
        }
        set
        {
            PlayerPrefsWrapper.SetBool(s_updateToNew, value);
        }
    }

    public static void Init()
    {
        Inited = true;
        string uid = Uid; 
        bool isOldUser = false;
        if (!PlayerPrefsWrapper.Exists(s_isOldUserKey))
        {
            IsOldUser = (SessionId > 0);
            UnityDebug.Log("IsOldUser: " + IsOldUser);
            isOldUser = true;
        }
        SessionId++;
        if (!PlayerPrefsWrapper.Exists(s_vibroRightKey))
        {
            if (isOldUser)
            {
                if (IsOldUser)
                {
                    VibroRightEnabled = VibroEnabled;
                    VibroWrongEnabled = false;
                }
                else
                {
                    VibroRightEnabled = false;
                    VibroWrongEnabled = true;
                }
            }
            else
            {
                VibroRightEnabled = false;
                VibroWrongEnabled = VibroEnabled;
            }
        }
        if (!PlayerPrefsWrapper.Exists(s_soundsKey))
        {
            SoundsEnabled = true;
        }
        if (!PlayerPrefsWrapper.Exists(s_lassoCountKey))
        {
            LassoCount = 5;
        }
        if (!PlayerPrefsWrapper.Exists(s_bombCountKey))
        {
            BombCount = 5;
        }
        if (!PlayerPrefsWrapper.Exists(s_bulbModeKey))
        {
            BulbMode = true;
        }
        if (!PlayerPrefsWrapper.Exists(s_loupeKey))
        {
            LoupeEnabled = false;
        }
        if (SessionId == 1)
        {
            FirstLaunch = DateTime.UtcNow;
            SecondsInGame = 0;
        }
        if (!PlayerPrefsWrapper.Exists(s_freePhotos))
        {
            FreePhotos = 2;
        }
        if (!PlayerPrefsWrapper.Exists(s_updateToNew))
        {
            UpdateToNew = (SessionId > 1);
        }
        CheckSessionEvents();
    }

    public static void CheckSessionEvents()
    {
        if (!PlayerPrefsWrapper.Exists(s_launchNextDayKey))
        {
            // AnalyticsManager.Instance.Test();
            try
            {
                DateTime installDate = SystemToolsWrapper.GetInstallDate();
                DateTime today = DateTime.Today;
                var t = today - installDate;
                if (t == new TimeSpan(1, 0, 0, 0))
                {
                    AnalyticsManager.Instance.LaunchNextDay();
                    AnalyticsManager.Instance.LaunchNext2Days();
                    LaunchNextDay = true;
                    LaunchNext2Days = true;
                }
                else if (t == new TimeSpan(2, 0, 0, 0))
                {
                    AnalyticsManager.Instance.LaunchNext2Days();
                    LaunchNextDay = true;
                    LaunchNext2Days = true;
                }
                if (t > new TimeSpan(2, 0, 0, 0))
                {
                    LaunchNextDay = true;
                    LaunchNext2Days = true;
                }
            }
            catch
            {
            }
        }
        if (PlayerPrefsWrapper.Exists(s_lastDayTracked) && !(LastDayTracked < DateTime.Today))
        {
            return;
        }

            AnalyticsManager.Instance.TrackFirstSessionOfTheDay();
            LastDayTracked = DateTime.Today;

    }

    public static void AddNewRateUsView()
    {
        var lastRateUsShows = LastRateUsShows;
        if (lastRateUsShows == null)
        {
            lastRateUsShows = new List<DateTime>();
        }
        lastRateUsShows.Add(DateTime.Now);
        while (lastRateUsShows.Count > 2)
        {
            lastRateUsShows.RemoveAt(0);
        }
        LastRateUsShows = lastRateUsShows;
    }
}



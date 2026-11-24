using UnityObject = global::UnityEngine.Object;
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public class LevelProgressControl : MonoBehaviour
{
    public enum PuzzleSourceType
    {
        PUZZLE_STORY,
        PUZZLE_DAILY,
        PUZZLE_PACK
    }

    public static LevelProgressControl control;
    public static int LevelsPerPack = 12;
    public static int NumberOfPacks = 60;
    public static int NumberOfActivePacks = 18;
    public static int LevelsToUnlock = 6;
    //xor key used to hide values saved in PlayerPrefs
    public static long CipherKey = 1273017494670060202L;
    public static int HintPotMax = 999;
    public static int adPeriodInSeconds = 300;

    [HideInInspector]
    public int[] levelOffsetArray;

    [HideInInspector]
    public int[] completeArray;

    [HideInInspector]
    public int[] movesPerfectArray;

    [HideInInspector]
    public int[] movesTakenArray;

    [HideInInspector]
    public int[] starsArray;

    [HideInInspector]
    public int[] timeTakenArray;

    [HideInInspector]
    public int[] attemptsTakenArray;

    [HideInInspector]
    public int[] hintsRemainingArray;

    [HideInInspector]
    public long[] hintsLastTimeArray;

    [HideInInspector]
    public int[] postcardCompleteStateArray;

    [HideInInspector]
    public int currentLevelId;

    [HideInInspector]
    public bool isReturningFromLevel;

    [HideInInspector]
    public int puzzleReturnId = -1;

    [HideInInspector]
    public int postcardReturnId;

    [HideInInspector]
    public int dailyDayOffset;

    [HideInInspector]
    public string postcardOrigin;

    [HideInInspector]
    public string puzzleBasename;

    [HideInInspector]
    public PuzzleSourceType puzzleSrcType;

    [HideInInspector]
    public float dailyTodayPersonalBest = 600f;

    [HideInInspector]
    public bool dailyTodayPersonalBestBeaten
    {
        get { return true; }
    }

    [HideInInspector]
    public int hintPotCount;

    [HideInInspector]
    public long hintPotLastPeriod;

    [HideInInspector]
    public bool iapUnlockLevels;

    [HideInInspector]
    public bool iapDisableAds;

    //[HideInInspector]
    //public int androidSdkLevel;

    void Awake()
    {
        if (Debug.isDebugBuild)
        {
            Debug.Log("LevelProgressControl : Awake()");
        }
        if (control == null)
        {
            UnityObject.DontDestroyOnLoad(gameObject);
            control = this;
            //one slot per level of every pack
            levelOffsetArray = new int[LevelsPerPack * NumberOfPacks];
            completeArray = new int[LevelsPerPack * NumberOfPacks];
            movesPerfectArray = new int[LevelsPerPack * NumberOfPacks];
            movesTakenArray = new int[LevelsPerPack * NumberOfPacks];
            starsArray = new int[LevelsPerPack * NumberOfPacks];
            timeTakenArray = new int[LevelsPerPack * NumberOfPacks];
            attemptsTakenArray = new int[LevelsPerPack * NumberOfPacks];
            hintsRemainingArray = new int[LevelsPerPack * NumberOfPacks];
            hintsLastTimeArray = new long[LevelsPerPack * NumberOfPacks];
            postcardCompleteStateArray = new int[NumberOfPacks];
            LoadProgressData();


            // Google Play Games setup
            #if (UNITY_ANDROID || (UNITY_IPHONE && !NO_GPGS))
            PlayGamesClientConfiguration config = new PlayGamesClientConfiguration.Builder()
                // enables saving game progress.
                //.EnableSavedGames()

                // require access to a player's Google+ social graph (usually not needed)
                //.RequireGooglePlus()
                .Build();
            PlayGamesPlatform.InitializeInstance(config);

            PlayGamesPlatform.DebugLogEnabled = true;
            PlayGamesPlatform.Activate();
            #endif
            //this.androidSdkLevel = this.GetAndroidSDKLevel();
        }
        else if (control != this)
        {
            UnityObject.Destroy(gameObject);
        }
    }

    //private int GetAndroidSDKLevel()
    //{
    //    IntPtr clazz = AndroidJNI.FindClass("android.os.Build$VERSION");
    //    IntPtr staticFieldID = AndroidJNI.GetStaticFieldID(clazz, "SDK_INT", "I");
    //    return AndroidJNI.GetStaticIntField(clazz, staticFieldID);
    //}

    public void SaveProgressData()
    {
        var binaryFormatter = new BinaryFormatter();
        //lpC10.dat is the 10 chapter save, lp.dat is the legacy one
        var fileStream = File.Create(Application.persistentDataPath + "/lpC10.dat");
        var playerData = new PlayerData(LevelsPerPack, NumberOfPacks);
        Array.Copy(levelOffsetArray, playerData.levelOffsetArray, levelOffsetArray.Length);
        Array.Copy(completeArray, playerData.completeArray, completeArray.Length);
        Array.Copy(movesPerfectArray, playerData.movesPerfectArray, movesPerfectArray.Length);
        Array.Copy(movesTakenArray, playerData.movesTakenArray, movesTakenArray.Length);
        Array.Copy(starsArray, playerData.starsArray, starsArray.Length);
        Array.Copy(timeTakenArray, playerData.timeTakenArray, timeTakenArray.Length);
        Array.Copy(attemptsTakenArray, playerData.attemptsTakenArray, attemptsTakenArray.Length);
        Array.Copy(hintsRemainingArray, playerData.hintsRemainingArray, hintsRemainingArray.Length);
        Array.Copy(hintsLastTimeArray, playerData.hintsLastTimeArray, hintsLastTimeArray.Length);
        Array.Copy(postcardCompleteStateArray, playerData.postcardCompleteStateArray, postcardCompleteStateArray.Length);
        binaryFormatter.Serialize(fileStream, playerData);
        fileStream.Close();
        SaveProgressDataPrefsOnly();
    }

    public void SaveProgressDataPrefsOnly()
    {
        //hint pot time and count, then the two iap flags as bool + offset, all xored
        PlayerPrefs.SetString("PuzzleColorId", (hintPotLastPeriod ^ CipherKey).ToString());
        PlayerPrefs.SetString("PuzzleColorKey", (hintPotCount ^ CipherKey).ToString());
        PlayerPrefs.SetString("PuzzleShapeSeed", (Convert.ToInt64(iapUnlockLevels) + 2927 ^ CipherKey).ToString());
        PlayerPrefs.SetString("PuzzleDiffTrackSeed", (Convert.ToInt64(iapDisableAds) + 1066 ^ CipherKey).ToString());
        if (dailyTodayPersonalBestBeaten)
        {
            PlayerPrefs.SetInt("DailyPersonalBest", (int)dailyTodayPersonalBest);
        }
        PlayerPrefs.Save();
    }

    public void _LoadProgressData()
    {
        if (File.Exists(Application.persistentDataPath + "/lp.dat"))
        {
            if (Debug.isDebugBuild)
            {
                Debug.Log("LoadProgressData() : Data file found. Loading..." + Application.persistentDataPath + "/lp.dat");
            }
            var binaryFormatter = new BinaryFormatter();
            var fileStream = File.Open(Application.persistentDataPath + "/lp.dat", FileMode.Open);
            var playerData = (PlayerData)binaryFormatter.Deserialize(fileStream);
            fileStream.Close();
            Array.Copy(playerData.levelOffsetArray, levelOffsetArray, playerData.levelOffsetArray.Length);
            Array.Copy(playerData.completeArray, completeArray, playerData.completeArray.Length);
            Array.Copy(playerData.movesPerfectArray, movesPerfectArray, playerData.movesPerfectArray.Length);
            Array.Copy(playerData.movesTakenArray, movesTakenArray, playerData.movesTakenArray.Length);
            Array.Copy(playerData.starsArray, starsArray, playerData.starsArray.Length);
            Array.Copy(playerData.timeTakenArray, timeTakenArray, playerData.timeTakenArray.Length);
            Array.Copy(playerData.attemptsTakenArray, attemptsTakenArray, playerData.attemptsTakenArray.Length);
            Array.Copy(playerData.hintsRemainingArray, hintsRemainingArray, playerData.hintsRemainingArray.Length);
            Array.Copy(playerData.hintsLastTimeArray, hintsLastTimeArray, playerData.hintsLastTimeArray.Length);
            Array.Copy(playerData.postcardCompleteStateArray, postcardCompleteStateArray, playerData.postcardCompleteStateArray.Length);
        }
        else if (Debug.isDebugBuild)
        {
            Debug.Log("LoadProgressData() : No data file, so assuming a fresh start");
        }
        LoadPlayerPrefs();
    }

    void LoadProgressDataLegacy()
    {
        if (Debug.isDebugBuild)
        {
            Debug.Log("LoadProgressDataLegacy()");
        }
        var binaryFormatter = new BinaryFormatter();
        var fileStream = File.Open(Application.persistentDataPath + "/lp.dat", FileMode.Open);
        var playerData = (PlayerData)binaryFormatter.Deserialize(fileStream);
        fileStream.Close();
        Array.Copy(playerData.levelOffsetArray, levelOffsetArray, playerData.levelOffsetArray.Length);
        Array.Copy(playerData.completeArray, completeArray, playerData.completeArray.Length);
        Array.Copy(playerData.movesPerfectArray, movesPerfectArray, playerData.movesPerfectArray.Length);
        Array.Copy(playerData.movesTakenArray, movesTakenArray, playerData.movesTakenArray.Length);
        Array.Copy(playerData.starsArray, starsArray, playerData.starsArray.Length);
        Array.Copy(playerData.timeTakenArray, timeTakenArray, playerData.timeTakenArray.Length);
        Array.Copy(playerData.attemptsTakenArray, attemptsTakenArray, playerData.attemptsTakenArray.Length);
        Array.Copy(playerData.hintsRemainingArray, hintsRemainingArray, playerData.hintsRemainingArray.Length);
        Array.Copy(playerData.hintsLastTimeArray, hintsLastTimeArray, playerData.hintsLastTimeArray.Length);
        Array.Copy(playerData.postcardCompleteStateArray, postcardCompleteStateArray, playerData.postcardCompleteStateArray.Length);
        if (Debug.isDebugBuild)
        {
            Debug.Log("Legacy array length : " + playerData.completeArray.Length);
            Debug.Log("C10 array length : " + completeArray.Length);
        }
    }

    public void LoadProgressData()
    {
        LoadPlayerPrefs();
        //new save first, then the legacy file, otherwise fresh start
        if (File.Exists(Application.persistentDataPath + "/lpC10.dat"))
        {
            Debug.Log("10-CHAPTER FILE EXISTS");
            LoadDataStandardStart();
        }
        else if (File.Exists(Application.persistentDataPath + "/lp.dat"))
        {
            Debug.Log("LEGACY DATA EXISTS");
            PlayerPrefs.SetInt("SuggestHint", 0);
            PlayerPrefs.Save();
            LoadDataUpdateStart();
        }
        else
        {
            Debug.Log("LEGACY DATA DOES NOT EXIST");
            LoadDataFreshStart();
        }
    }

    public void LoadDataStandardStart()
    {
        if (Debug.isDebugBuild)
        {
            Debug.Log("LoadDataStandardStart()");
        }
        var binaryFormatter = new BinaryFormatter();
        var fileStream = File.Open(Application.persistentDataPath + "/lpC10.dat", FileMode.Open);
        var playerData = (PlayerData)binaryFormatter.Deserialize(fileStream);
        fileStream.Close();
        Array.Copy(playerData.levelOffsetArray, levelOffsetArray, playerData.levelOffsetArray.Length);
        Array.Copy(playerData.completeArray, completeArray, playerData.completeArray.Length);
        Array.Copy(playerData.movesPerfectArray, movesPerfectArray, playerData.movesPerfectArray.Length);
        Array.Copy(playerData.movesTakenArray, movesTakenArray, playerData.movesTakenArray.Length);
        Array.Copy(playerData.starsArray, starsArray, playerData.starsArray.Length);
        Array.Copy(playerData.timeTakenArray, timeTakenArray, playerData.timeTakenArray.Length);
        Array.Copy(playerData.attemptsTakenArray, attemptsTakenArray, playerData.attemptsTakenArray.Length);
        Array.Copy(playerData.hintsRemainingArray, hintsRemainingArray, playerData.hintsRemainingArray.Length);
        Array.Copy(playerData.hintsLastTimeArray, hintsLastTimeArray, playerData.hintsLastTimeArray.Length);
        Array.Copy(playerData.postcardCompleteStateArray, postcardCompleteStateArray, playerData.postcardCompleteStateArray.Length);
    }

    public void LoadDataUpdateStart()
    {
        if (Debug.isDebugBuild)
        {
            Debug.Log("LoadDataUpdateStart()");
        }
        var binaryFormatter = new BinaryFormatter();
        var fileStream = File.Open(Application.persistentDataPath + "/lp.dat", FileMode.Open);
        var playerData = (PlayerData)binaryFormatter.Deserialize(fileStream);
        fileStream.Close();
        Array.Copy(playerData.levelOffsetArray, levelOffsetArray, playerData.levelOffsetArray.Length);
        Array.Copy(playerData.completeArray, completeArray, playerData.completeArray.Length);
        Array.Copy(playerData.movesPerfectArray, movesPerfectArray, playerData.movesPerfectArray.Length);
        Array.Copy(playerData.movesTakenArray, movesTakenArray, playerData.movesTakenArray.Length);
        Array.Copy(playerData.starsArray, starsArray, playerData.starsArray.Length);
        Array.Copy(playerData.timeTakenArray, timeTakenArray, playerData.timeTakenArray.Length);
        Array.Copy(playerData.attemptsTakenArray, attemptsTakenArray, playerData.attemptsTakenArray.Length);
        Array.Copy(playerData.hintsRemainingArray, hintsRemainingArray, playerData.hintsRemainingArray.Length);
        Array.Copy(playerData.hintsLastTimeArray, hintsLastTimeArray, playerData.hintsLastTimeArray.Length);
        Array.Copy(playerData.postcardCompleteStateArray, postcardCompleteStateArray, playerData.postcardCompleteStateArray.Length);
    }

    public void LoadDataFreshStart()
    {
        if (Debug.isDebugBuild)
        {
            Debug.Log("LoadDataFreshStart()");
        }
    }

    public void ResetProgressData()
    {
        Array.Clear(levelOffsetArray, 0, levelOffsetArray.Length);
        Array.Clear(completeArray, 0, completeArray.Length);
        Array.Clear(movesPerfectArray, 0, movesPerfectArray.Length);
        Array.Clear(movesTakenArray, 0, movesTakenArray.Length);
        Array.Clear(starsArray, 0, starsArray.Length);
        Array.Clear(timeTakenArray, 0, timeTakenArray.Length);
        Array.Clear(hintsRemainingArray, 0, hintsRemainingArray.Length);
        Array.Clear(hintsLastTimeArray, 0, hintsLastTimeArray.Length);
        Array.Clear(postcardCompleteStateArray, 0, postcardCompleteStateArray.Length);
        SaveProgressData();
    }

    public void LoadPlayerPrefs()
    {
        //PuzzleColorId = last hint pot refill time in ticks
        string str = PlayerPrefs.GetString("PuzzleColorId", "no");
        long savedPeriod = default(long);
        if (str.Equals("no"))
        {
            long ticks = DateTime.Now.Ticks;
            PlayerPrefs.SetString("PuzzleColorId", (ticks ^ CipherKey).ToString());
            hintPotLastPeriod = ticks;
        }
        else if (long.TryParse(str, out savedPeriod))
        {
            hintPotLastPeriod = savedPeriod ^ CipherKey;
        }
        //PuzzleColorKey = hint count, new players get 3
        string colorKey = PlayerPrefs.GetString("PuzzleColorKey", "no");
        long savedColorKey = default(long);
        if (colorKey.Equals("no"))
        {
            PlayerPrefs.SetString("PuzzleColorKey", (3 ^ CipherKey).ToString());
            hintPotCount = 3;
        }
        else if (long.TryParse(colorKey, out savedColorKey))
        {
            if (Debug.isDebugBuild)
            {
                Debug.Log("PuzzleColorKey : " + savedColorKey.ToString());
            }
            long hintPotValue = savedColorKey ^ CipherKey;
            if (Debug.isDebugBuild)
            {
                Debug.Log("decode : " + hintPotValue);
            }
            hintPotCount = Convert.ToInt32(hintPotValue);
        }
        //PuzzleShapeSeed = unlock levels purchase
        string shapeSeed = PlayerPrefs.GetString("PuzzleShapeSeed", "no");
        long savedShapeSeed = default(long);
        if (shapeSeed.Equals("no"))
        {
            if (Debug.isDebugBuild)
            {
                Debug.Log("PuzzleShapeSeed NOT SET");
            }
            PlayerPrefs.SetString("PuzzleShapeSeed", (Convert.ToInt64(false) + 2927 ^ CipherKey).ToString());
            iapUnlockLevels = false;
        }
        else if (long.TryParse(shapeSeed, out savedShapeSeed))
        {
            if (Debug.isDebugBuild)
            {
                Debug.Log("PuzzleShapeSeed : " + savedShapeSeed.ToString());
            }
            long unlockLevelsValue = (savedShapeSeed ^ CipherKey) - 2927;
            if (Debug.isDebugBuild)
            {
                Debug.Log("decode : " + unlockLevelsValue);
            }
            iapUnlockLevels = Convert.ToBoolean(unlockLevelsValue);
        }
        //PuzzleDiffTrackSeed = disable ads purchase
        string diffTrackSeed = PlayerPrefs.GetString("PuzzleDiffTrackSeed", "no");
        long savedDiffTrackSeed = default(long);
        if (diffTrackSeed.Equals("no"))
        {
            if (Debug.isDebugBuild)
            {
                Debug.Log("PuzzleDiffTrackSeed NOT SET");
            }
            PlayerPrefs.SetString("PuzzleDiffTrackSeed", (Convert.ToInt64(false) + 1066 ^ CipherKey).ToString());
            iapDisableAds = false;
        }
        else if (long.TryParse(diffTrackSeed, out savedDiffTrackSeed))
        {
            if (Debug.isDebugBuild)
            {
                Debug.Log("PuzzleDiffTrackSeed : " + savedDiffTrackSeed.ToString());
            }
            long disableAdsValue = (savedDiffTrackSeed ^ CipherKey) - 1066;
            if (Debug.isDebugBuild)
            {
                Debug.Log("decode : " + disableAdsValue);
            }
            iapDisableAds = Convert.ToBoolean(disableAdsValue);
        }
        // 0 means no daily best yet, fall back to 600
        int number = PlayerPrefs.GetInt("DailyPersonalBest", 0);
        if (number == 0)
        {
            control.dailyTodayPersonalBest = 600f;
        }
        else
        {
            control.dailyTodayPersonalBest = (float)number;
        }
        PlayerPrefs.Save();
    }

    public bool CheckHintPot()
    {
        DateTime dateTime = new DateTime(hintPotLastPeriod);
        DateTime now = DateTime.Now;
        int compare = dateTime.CompareTo(DateTime.Now);
        if (compare < 0)
        {
            if (Debug.isDebugBuild)
            {
                Debug.Log("CheckHintPot : earlier");
            }
            var timeSpan = now - dateTime;
            if (Debug.isDebugBuild)
            {
                Debug.Log(now);
                Debug.Log(dateTime);
                Debug.Log(timeSpan);
                Debug.Log("timediff.TotalHours : " + timeSpan.TotalHours);
                Debug.Log("timediff.Days*24 + timeDiff.Hours : " + (timeSpan.Days * 24 + timeSpan.Hours));
            }
            //hint pot is ready again after more than 6 hours
            if (timeSpan.Days * 24 + timeSpan.Hours > 6)
            {
                return true;
            }
        }
        else if (compare == 0)
        {
            if (Debug.isDebugBuild)
            {
                Debug.Log("CheckHintPot : same");
            }
        }
        else if (Debug.isDebugBuild)
        {
            Debug.Log("CheckHintPot : later");
        }
        return false;
    }

    public void IncrementHintPot()
    {
        if (hintPotCount < HintPotMax)
        {
            hintPotCount++;
            hintPotLastPeriod = DateTime.Now.Ticks;
            PlayerPrefs.SetString("PuzzleColorId", (hintPotLastPeriod ^ CipherKey).ToString());
            PlayerPrefs.SetString("PuzzleColorKey", (hintPotCount ^ CipherKey).ToString());
        }
    }

    public void RewardHintPot(int rewards)
    {
        if (hintPotCount < HintPotMax)
        {
            hintPotCount += rewards;
            hintPotCount = Mathf.Clamp(hintPotCount, 0, HintPotMax);
            PlayerPrefs.SetString("PuzzleColorKey", (hintPotCount ^ CipherKey).ToString());
        }
    }

    public int NumberOfPostcardsUnlocked()
    {
        int count = 0;
        for (int i = 0; i < NumberOfPacks; i++)
        {
            if (postcardCompleteStateArray[i] > 0)
            {
                count++;
            }
        }
        return count;
    }
}



using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using UnityEngine; 
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GetNameScript : MonoBehaviour
{
    public Text resultText;
    public Button goButton;
    public Button todayButton;
    public Button yesterdayButton;
    public Button ereyesterdayButton;
    public Image puzzleReadyTodayImg;
    public Image puzzleReadyYesterdayImg;
    public Image puzzleReadyEreyesterdayImg;
    public GameObject starsParentToday;
    public GameObject starsParentYesterday;
    public GameObject starsParentEreyesterday;
    public Text dateNumTodayText;
    public Text dateNumYesterdayText;
    public Text dateNumEreyesterdayText;
    public Button leaderboardButton;
    public Text leaderboardPromptText;
    public Material grayMaterial;
    //public Button lbDialogSignoutButton;

    //public GameObject dialogLeaderboard;

    //public GameObject dialogGel;

    //public Image dialogTitleImage;

    //public Sprite gcLeaderboardSprite;

    public GameObject dlSpinner;
    public Text rolloverTimeText;
    public Text bestTimeText;
    public Image bestTimeNewImage;
    public LeaderboardScript leaderBoardScript;

    bool bTodayDataExists;
    bool bYesterdayDataExists;
    bool bEreyesterdayDataExists;
    bool bIsDataFetched;
    bool bIsImageFetched;
    string todayFilename;
    string yesterdayFilename;
    string ereyesterdayFilename;
    int todayStars;
    int yesterdayStars;
    int ereyesterdayStars;

    public static GetNameScript Instance;
    void Awake()
    {
        Instance = this;
        //Init();
    }

    public void Init()
    {
        todayFilename = GetDailyIdentity(0);
        yesterdayFilename = GetDailyIdentity(-1);
        ereyesterdayFilename = GetDailyIdentity(-2);
        CheckDailyPrefDataExists();
        CalcDailyPrefDateDiff();
        todayStars = GetDailyStars("dailytodaydata");
        yesterdayStars = GetDailyStars("dailyyesterdaydata");
        ereyesterdayStars = GetDailyStars("dailyereyesterdaydata");
        if (todayStars >= 1 && todayStars != 99)
        {
            ((Component)starsParentToday.transform.Find("StarL")).GetComponent<Image>().enabled = true;
            ((Component)starsParentToday.transform.Find("StarMarkerL")).GetComponent<Image>().enabled = false;
        }
        if (todayStars >= 2 && todayStars != 99)
        {
            ((Component)starsParentToday.transform.Find("StarM")).GetComponent<Image>().enabled = true;
            ((Component)starsParentToday.transform.Find("StarMarkerM")).GetComponent<Image>().enabled = false;
        }
        if (todayStars >= 3 && todayStars != 99)
        {
            ((Component)starsParentToday.transform.Find("StarR")).GetComponent<Image>().enabled = true;
            ((Component)starsParentToday.transform.Find("StarMarkerR")).GetComponent<Image>().enabled = false;
        }
        if (yesterdayStars >= 1 && yesterdayStars != 99)
        {
            ((Component)starsParentYesterday.transform.Find("StarL")).GetComponent<Image>().enabled = true;
            ((Component)starsParentYesterday.transform.Find("StarMarkerL")).GetComponent<Image>().enabled = false;
        }
        if (yesterdayStars >= 2 && yesterdayStars != 99)
        {
            ((Component)starsParentYesterday.transform.Find("StarM")).GetComponent<Image>().enabled = true;
            ((Component)starsParentYesterday.transform.Find("StarMarkerM")).GetComponent<Image>().enabled = false;
        }
        if (yesterdayStars >= 3 && yesterdayStars != 99)
        {
            ((Component)starsParentYesterday.transform.Find("StarR")).GetComponent<Image>().enabled = true;
            ((Component)starsParentYesterday.transform.Find("StarMarkerR")).GetComponent<Image>().enabled = false;
        }
        if (ereyesterdayStars >= 1 && ereyesterdayStars != 99)
        {
            ((Component)starsParentEreyesterday.transform.Find("StarL")).GetComponent<Image>().enabled = true;
            ((Component)starsParentEreyesterday.transform.Find("StarMarkerL")).GetComponent<Image>().enabled = false;
        }
        if (ereyesterdayStars >= 2 && ereyesterdayStars != 99)
        {
            ((Component)starsParentEreyesterday.transform.Find("StarM")).GetComponent<Image>().enabled = true;
            ((Component)starsParentEreyesterday.transform.Find("StarMarkerM")).GetComponent<Image>().enabled = false;
        }
        if (ereyesterdayStars >= 3 && ereyesterdayStars != 99)
        {
            ((Component)starsParentEreyesterday.transform.Find("StarR")).GetComponent<Image>().enabled = true;
            ((Component)starsParentEreyesterday.transform.Find("StarMarkerR")).GetComponent<Image>().enabled = false;
        }
        if (DailyDataExists(todayFilename))
        {
            SetDailyButtonVerb("Play", 0);
            bTodayDataExists = true;
            if (todayStars == 99)
            {
                LoadDailyPuzzleIcon(0, true);
            }
            else
            {
                LoadDailyPuzzleIcon(0, false);
            }
        }
        else
        {
            SetDailyButtonVerb("Download", 0);
        }
        dateNumTodayText.text = GetDailyDateDayString(0);
        if (DailyDataExists(yesterdayFilename))
        {
            SetDailyButtonVerb("Play", -1);
            bYesterdayDataExists = true;
            if (yesterdayStars == 99)
            {
                LoadDailyPuzzleIcon(-1, true);
            }
            else
            {
                LoadDailyPuzzleIcon(-1, false);
            }
        }
        else
        {
            SetDailyButtonVerb("Download", -1);
        }
        dateNumYesterdayText.text = GetDailyDateDayString(-1);
        if (DailyDataExists(ereyesterdayFilename))
        {
            SetDailyButtonVerb("Play", -2);
            bEreyesterdayDataExists = true;
            if (ereyesterdayStars == 99)
            {
                LoadDailyPuzzleIcon(-2, true);
            }
            else
            {
                LoadDailyPuzzleIcon(-2, false);
            }
        }
        else
        {
            SetDailyButtonVerb("Download", -2);
        }
        dateNumEreyesterdayText.text = GetDailyDateDayString(-2);
        //this.leaderBoardScript = base.GetComponent<LeaderboardScript>();
        //if (LevelProgressControl.control.androidSdkLevel < 14)
        //{
        //    this.leaderboardButton.interactable = false;
        //}

        resultText.gameObject.SetActive(false);
        dlSpinner.SetActive(false);
        bestTimeNewImage.gameObject.SetActive(false);
        var timeSpan = TimeSpan.FromSeconds((double)LevelProgressControl.control.dailyTodayPersonalBest);
        string str = string.Format("{0:D2}:{1:D2}", timeSpan.Minutes, timeSpan.Seconds);
        bestTimeText.text = "Today's Personal Best Time is " + str;
         
        base.InvokeRepeating("DailyRolloverCountdown", 0f, 60f);
        if (LevelProgressControl.control.isReturningFromLevel)
        {
            LevelProgressControl.control.isReturningFromLevel = false;
            if (LevelProgressControl.control.dailyTodayPersonalBestBeaten)
            {
                //base.StartCoroutine(this.FlashSchedule());
            }
            //LevelProgressControl.control.dailyTodayPersonalBestBeaten = false;
            int stateId = PlayerPrefs.GetInt("PuzzleStateId", -1);
            if (stateId == -1)
            {
                stateId = 287462819;
                PlayerPrefs.SetInt("PuzzleStateId", stateId);
            }
            stateId = (stateId - 287462819) / 3;
            if (stateId > LevelProgressControl.adPeriodInSeconds)
            {
                if (!LevelProgressControl.control.iapDisableAds)
                {
                    //if (AdsWrapper.Instance.IsVideoAvailable())
                    {
                        //AdsWrapper.Instance.ShowInter(string.Empty);
                        stateId = 287462819;
                        PlayerPrefs.SetInt("PuzzleStateId", stateId);
                        PlayerPrefs.Save();
                    }
                }
                else
                {
                    Debug.Log("IAP to disable ads detected. Skip Ad.");
                }
            }
            else if (Debug.isDebugBuild)
            {
                Debug.Log("currAccumTime still accumulating... : " + stateId.ToString());
            }
        }
    }

    public static void GetTodayArt(ImagePreview image, Material grayMaterial)
    {
        NewLibraryWindow.UpdateDailyClick(false);
        var identify = GetDailyIdentity(0);

        if (DailyDataExists(identify))
        {
            DataManager.Instance.GetImages(false, (ii) =>
            {
                int index = GetIdentifyIndex(identify, ii.Images.Count);
                var imageInfo = ii.Images[index];

                image.Init(imageInfo);
                image.LoadIcon();
                NewLibraryWindow.UpdateDailyClick(true);
            });  
        } 
    }

    public void PressDownloadorPlay(int dailyOffset)
    {
        Debug.Log("PressDownloadorPlay()");
        AudioManager.Instance.PlayClick(); 
        if (DailyDataExists(GetDailyIdentity(dailyOffset)))
        {
            Debug.Log("PLAY");
            LevelProgressControl.control.dailyDayOffset = dailyOffset;
            ExecutePuzzle(GetDailyIdentity(dailyOffset));
        }
        else
        {
            Debug.Log("DOWNLOAD");
            ResetDailyDownload();
            dlSpinner.SetActive(true);
            string str = GetDailyIdentity(dailyOffset);
            base.StartCoroutine(DownloadTextureData(str, dailyOffset)); 
        }
    }

    public void ResetDailyDownload()
    {
        bIsDataFetched = false;
        bIsImageFetched = false;
    }

    public void ExecutePuzzle()
    {
        PlayerPrefs.SetString("datafile", "daily");
        SceneManager.LoadScene("GenericPuzzle");
    }

    public void ExecutePuzzle(string identify)
    {
        LevelProgressControl.control.puzzleSrcType = LevelProgressControl.PuzzleSourceType.PUZZLE_DAILY;
        LevelProgressControl.control.postcardOrigin = SceneManager.GetActiveScene().name;
        LevelProgressControl.control.puzzleBasename = identify;
        PlayerPrefs.SetString("datafile", identify);

        DataManager.Instance.GetImages(false, (ii) =>
        {
            int index = GetIdentifyIndex(identify, ii.Images.Count);
            var imageInfo = ii.Images[index];

            NewLibraryWindow.ExecutePuzzle(imageInfo);
        });
    }
    static int GetIdentifyIndex(string str, int maxSize)
    {
        uint number = (uint)str.GetHashCode();
        return (int)((number + (maxSize + 200)) % maxSize);
    }
    IEnumerator DownloadTextureData(string identify, int dailyOffset)
    {
        if (DataManager.Instance == null)
            yield return null;

        LoadDailyPuzzleIcon(dailyOffset, true);
    } 

    void LoadSilhouette()
    {
        var texture2D = LoadPNG(Application.persistentDataPath + "/daily.png");
    }
    IEnumerator LoadSaveCoroutine(ImageInfo imageInfo, Image image, string id)
    {
        var resTex = new Texture2D(1, 1, TextureFormat.ARGB32, false);
        resTex.filterMode = (FilterMode)(imageInfo.Is3D ? 1 : 0);
        var swd = MainManager.Instance.SavedWorksList.LoadById(id);
        yield return null;
        resTex.LoadImage(swd.Preview);

        image.sprite = Sprite.Create(resTex, new Rect(0f, 0f, (float)resTex.width, (float)resTex.height), new Vector2(0.5f, 0.5f));
    }

    void LoadDailyPuzzleIcon(Image image, string identify, int dailyOffset, bool isSilhouette)
    {
        if (isSilhouette)
        {
            image.material = grayMaterial;
        }
        else
        {
            image.material = null;
        }

        DataManager.Instance.GetImages(false, (ii) =>
        {
            int index = GetIdentifyIndex(identify, ii.Images.Count);
            var imageInfo = ii.Images[index];

            var saveId = MainManager.Instance.SavedWorksList.LastSaveOfImageId(imageInfo.Id);

            if (imageInfo.Is3D && saveId != null)
            {
                bIsImageFetched = true;
                image.gameObject.SetActive(true);
                base.StartCoroutine(LoadSaveCoroutine(imageInfo, image, saveId));
                SetDailyButtonVerb("Play", dailyOffset);
                dlSpinner.SetActive(false); 

                PlayerPrefsWrapper.SetBool("Daily-" + identify, true);
                if (dailyOffset == 0)
                {
                    MainMenu.Instance.RefreshTodayArt();
                }
            }
            else
            {
                DataManager.Instance.GetImageAsset(imageInfo, delegate (bool res, Texture2D tex)
                {
                    if (res)
                    {
                        bIsImageFetched = true;
                        image.sprite = Sprite.Create(tex, new Rect(0f, 0f, (float)tex.width, (float)tex.height), new Vector2(0.5f, 0.5f));
                        if (saveId != null)
                        {
                            base.StartCoroutine(LoadSaveCoroutine(imageInfo, image, saveId));
                        }
                        SetDailyButtonVerb("Play", dailyOffset);
                        dlSpinner.SetActive(false);

                        PlayerPrefsWrapper.SetBool("Daily-" + identify, true);
                        if (dailyOffset == 0)
                        {
                            MainMenu.Instance.RefreshTodayArt();
                        }
                    } 
                });
                if (imageInfo.Is3D)
                {
                    DataManager.Instance.GetImageAsset3D(imageInfo, null);
                }
            }
        }); 
    }

    void LoadDailyPuzzleIcon(int todayOffset, bool isSilhouette)
    {
        switch (todayOffset)
        {
            case 0:
                LoadDailyPuzzleIcon(puzzleReadyTodayImg, GetDailyIdentity(todayOffset), todayOffset, isSilhouette);
                break;
            case -1:
                LoadDailyPuzzleIcon(puzzleReadyYesterdayImg, GetDailyIdentity(todayOffset), todayOffset, isSilhouette);
                break;
            case -2:
                LoadDailyPuzzleIcon(puzzleReadyEreyesterdayImg, GetDailyIdentity(todayOffset), todayOffset, isSilhouette);
                break;
        }

        //Texture2D texture2D = LoadPNG(Application.persistentDataPath + "/" + GetDailyIdentity(todayOffset) + ".png");
  //      switch (todayOffset)
  //      {
  //          case 0:
  //              this.puzzleReadyTodayImg.sprite = Sprite.Create(texture2D, new Rect(0f, 0f, (float)texture2D.width, (float)texture2D.height), new Vector2(0.5f, 0.5f));
  //              if (isSilhouette)
  //              {
        //			this.puzzleReadyTodayImg.material = grayMaterial; 
  //              }
  //              else
  //              {
        //			this.puzzleReadyTodayImg.material = null; 
  //              }
  //              break;
  //          case -1:
  //              this.puzzleReadyYesterdayImg.sprite = Sprite.Create(texture2D, new Rect(0f, 0f, (float)texture2D.width, (float)texture2D.height), new Vector2(0.5f, 0.5f));
  //              if (isSilhouette)
  //              {
  //                  this.puzzleReadyYesterdayImg.material = grayMaterial;
        //		}
  //              else
  //              {
  //                  this.puzzleReadyYesterdayImg.material = null;
        //		}
  //              break;
  //          case -2:
  //              this.puzzleReadyEreyesterdayImg.sprite = Sprite.Create(texture2D, new Rect(0f, 0f, (float)texture2D.width, (float)texture2D.height), new Vector2(0.5f, 0.5f));
  //              if (isSilhouette)
  //              {
  //                  this.puzzleReadyEreyesterdayImg.material = grayMaterial;
        //		}
  //              else
  //              {
  //                  this.puzzleReadyEreyesterdayImg.material = null;
        //		}
  //              break;
  //      }
    }

    public static Texture2D LoadPNG(string filePath)
    {
        Texture2D texture2D = null;
        if (File.Exists(filePath))
        {
            var data = File.ReadAllBytes(filePath);
            texture2D = new Texture2D(2, 2, TextureFormat.RGBA32, false);
            texture2D.filterMode = FilterMode.Point;
            texture2D.wrapMode = TextureWrapMode.Clamp;
            texture2D.LoadImage(data);
        }
        return texture2D;
    }

    public static string GetDailyIdentity(int todayOffset)
    {
        DateTime dateTime = DateTime.UtcNow.AddHours(-8.0);
        if (todayOffset != 0)
        {
            dateTime = dateTime.AddDays((double)todayOffset);
        }
        Debug.Log(dateTime.Date);
        string text = string.Format("{0,3:D3}-{1}", dateTime.DayOfYear, dateTime.Year);
        Debug.Log("Today's identity string: DayOfYear : Year -> " + text);
        return text;
    }

    string GetDailyDateString(int todayOffset)
    {
        DateTime dateTime = DateTime.UtcNow.AddHours(-8.0);
        if (todayOffset != 0)
        {
            dateTime = dateTime.AddDays((double)todayOffset);
        }
        return DateTimeFormatInfo.CurrentInfo.GetAbbreviatedMonthName(dateTime.Month) + " " + dateTime.Day.ToString("00");
    }

    string GetDailyDateDayString(int todayOffset)
    {
        DateTime dateTime = DateTime.UtcNow.AddHours(-8.0);
        if (todayOffset != 0)
        {
            dateTime = dateTime.AddDays((double)todayOffset);
        }
        return dateTime.Day.ToString("00");
    }

    static bool DailyDataExists(string identify)
    {
        return PlayerPrefsWrapper.GetBool("Daily-" + identify);
        //if (DataManager.Instance.AllImages == null || DataManager.Instance.AllImages.Images.Count == 0)
        //	return false;

        //int index = GetIdentifyIndex(identify, DataManager.Instance.AllImages.Images.Count);
        //var imageInfo = DataManager.Instance.AllImages.Images[index];

        //return DataManager.Instance.CheckLocalAsset(imageInfo); 

        //string path = Application.persistentDataPath + "/" + fnameBase + ".png";
  //      string path2 = Application.persistentDataPath + "/" + fnameBase + "-data.txt";
  //      bool result = true;
  //      if (!File.Exists(path))
  //      {
  //          result = false;
  //      }
  //      if (!File.Exists(path2))
  //      {
  //          result = false;
  //      }
  //      return result;
    }

    void SetDailyButtonVerb(string verb, int dailyOffset)
    {
        switch (dailyOffset)
        {
            case 0:
                ((Component)todayButton).GetComponentInChildren<Text>().text = verb + " " + GetDailyDateString(0);
                break;
            case -1:
                ((Component)yesterdayButton).GetComponentInChildren<Text>().text = verb + " " + GetDailyDateString(-1);
                break;
            case -2:
                ((Component)ereyesterdayButton).GetComponentInChildren<Text>().text = verb + " " + GetDailyDateString(-2);
                break;
        }
    }

    public void CleanDailyPuzzleDataFiles()
    {
        Debug.Log("*** CleanDailyPuzzleDataFiles ***");
        var files = Directory.GetFiles(Application.persistentDataPath, "???-20??*.*", SearchOption.TopDirectoryOnly);
        if (files.Length > 12)
        {
            var dataFiles = files;
            foreach (string text in dataFiles)
            {
                Debug.Log("Daily data file -> " + text);
                string fileName = Path.GetFileName(text);
                Debug.Log("Daily data file -> " + fileName);
                if (fileName.Contains(todayFilename))
                {
                    Debug.Log("Daily is today. NOT DELETING!");
                }
                else if (fileName.Contains(yesterdayFilename))
                {
                    Debug.Log("Daily is yesterday. NOT DELETING!");
                }
                else if (fileName.Contains(ereyesterdayFilename))
                {
                    Debug.Log("Daily is ereyesterday. NOT DELETING!");
                }
                else
                {
                    Debug.Log("DELETING " + text);
                    File.Delete(text);
                }
            }
        }
    }

    public int CalcDailyPrefDateDiff()
    {
        Debug.Log("CalcDailyDateDiff");
        var parts = todayFilename.Split('-');
        Debug.Log("today day " + parts[0] + " today year " + parts[1]);
        int todayDay = Convert.ToInt32(parts[0]);
        int todayYear = Convert.ToInt32(parts[1]);
        int todayDays = todayYear * 365 + todayDay;
        Debug.Log("Integer version: today day " + todayDay + " today year " + todayYear + " total days " + todayDays);
        string @string = PlayerPrefs.GetString("dailytodaydata", "nodata");
        if (@string.Equals("nodata"))
        {
            Debug.Log("CalcDailyPrefDateDiff -> nodata for dailytodaydata. This shouldn't happen");
        }
        var savedParts = @string.Split('-');
        int savedDay = Convert.ToInt32(savedParts[0]);
        int savedYear = Convert.ToInt32(savedParts[1]);
        int savedState = Convert.ToInt32(savedParts[2]);
        switch (todayDays - (savedYear * 365 + savedDay))
        {
            case 0:
                if (Debug.isDebugBuild)
                {
                    Debug.Log("daysDiff == 0");
                }
                return 0;
            case 1:
            {
                if (Debug.isDebugBuild)
                {
                    Debug.Log("daysDiff == 1");
                }
                PlayerPrefs.SetString("dailyereyesterdaydata", PlayerPrefs.GetString("dailyyesterdaydata", "nodata"));
                PlayerPrefs.SetString("dailyyesterdaydata", PlayerPrefs.GetString("dailytodaydata", "nodata"));
                string todayData = todayFilename + "-99";
                PlayerPrefs.SetString("dailytodaydata", todayData);
                LevelProgressControl.control.dailyTodayPersonalBest = 600f;
                break;
            }
            case 2:
            {
                if (Debug.isDebugBuild)
                {
                    Debug.Log("daysDiff == 2");
                }
                PlayerPrefs.SetString("dailyereyesterdaydata", PlayerPrefs.GetString("dailytodaydata", "nodata"));
                string yesterdayData = yesterdayFilename + "-99";
                PlayerPrefs.SetString("dailyyesterdaydata", yesterdayData);
                string todayData = todayFilename + "-99";
                PlayerPrefs.SetString("dailytodaydata", todayData);
                LevelProgressControl.control.dailyTodayPersonalBest = 600f;
                break;
            }
            default:
            {
                if (Debug.isDebugBuild)
                {
                    Debug.Log("daysDiff > 2");
                }
                string value = todayFilename + "-99";
                PlayerPrefs.SetString("dailytodaydata", value);
                LevelProgressControl.control.dailyTodayPersonalBest = 600f;
                string yesterdayData = yesterdayFilename + "-99";
                PlayerPrefs.SetString("dailyyesterdaydata", yesterdayData);
                string ereyesterdayData = ereyesterdayFilename + "-99";
                PlayerPrefs.SetString("dailyereyesterdaydata", ereyesterdayData);
                break;
            }
        }
        PlayerPrefs.SetInt("DailyPersonalBest", (int)LevelProgressControl.control.dailyTodayPersonalBest);
        PlayerPrefs.Save();
        return 0;
    }

    void CheckDailyPrefDataExists()
    {
        string @string = PlayerPrefs.GetString("dailytodaydata", "nodata");
        if (@string.Equals("nodata"))
        {
            string value = todayFilename + "-99";
            PlayerPrefs.SetString("dailytodaydata", value);
            if (Debug.isDebugBuild)
            {
                Debug.Log("today PPref = nodata");
            }
            LevelProgressControl.control.dailyTodayPersonalBest = 600f;
        }
        string yesterdayData = PlayerPrefs.GetString("dailyyesterdaydata", "nodata");
        if (yesterdayData.Equals("nodata"))
        {
            string emptyData = yesterdayFilename + "-99";
            PlayerPrefs.SetString("dailyyesterdaydata", emptyData);
            if (Debug.isDebugBuild)
            {
                Debug.Log("yesterday PPref = nodata");
            }
        }
        string ereyesterdayData = PlayerPrefs.GetString("dailyereyesterdaydata", "nodata");
        if (ereyesterdayData.Equals("nodata"))
        {
            string emptyData = ereyesterdayFilename + "-99";
            PlayerPrefs.SetString("dailyereyesterdaydata", emptyData);
            if (Debug.isDebugBuild)
            {
                Debug.Log("ereyesterday PPref = nodata");
            }
        }
        PlayerPrefs.Save();
    }

    static int GetDailyStars(string prefname)
    {
        string str = PlayerPrefs.GetString(prefname, "nodata");
        var parts = str.Split('-');
        return Convert.ToInt32(parts[2]);
    } 

    public void PressShowLeaderboardButton()
    {
        AudioManager.Instance.PlayClick();
        leaderBoardScript.ShowLeaderboards();
    } 

    public void PressSignoutButton()
    {
        AudioManager.Instance.PlayClick();
        leaderBoardScript.LeaderboardLogout();
        PressCloseButton();
    }

    public void PressCloseButton()
    { 
        SetButtonsInteractable(true);
        AudioManager.Instance.PlayClick();
    }

    void SetButtonsInteractable(bool bSetting)
    {
        todayButton.interactable = bSetting;
        yesterdayButton.interactable = bSetting;
        ereyesterdayButton.interactable = bSetting;
        leaderboardButton.interactable = bSetting;
        //if (LevelProgressControl.control.androidSdkLevel < 14)
        //{
        //    this.leaderboardButton.interactable = false;
        //}
        float a = 1f;
        if (!bSetting)
        {
            a = 0.25f;
        }
        Color color = new Color(1f, 1f, 1f, a);
        ((Component)todayButton).GetComponentInChildren<Text>().color = color;
        ((Component)yesterdayButton).GetComponentInChildren<Text>().color = color;
        ((Component)ereyesterdayButton).GetComponentInChildren<Text>().color = color;
    }

    void DailyRolloverCountdown()
    {
        DateTime dateTime = DateTime.UtcNow.AddHours(-8.0);
        Debug.Log("UTC Now today (PST) : " + dateTime);
        DateTime midnight = new DateTime(dateTime.Year, dateTime.Month, dateTime.Day);
        Debug.Log("UTC Rollover midnight 24 hrs early (PST) : " + midnight);
        DateTime nextMidnight = midnight.AddDays(1.0);
        Debug.Log("UTC Rollover midnight (PST) : " + nextMidnight);
        var timeSpan = nextMidnight.Subtract(dateTime);
        Debug.Log("Time until roll over " + timeSpan);
        string str = string.Format("{0:D2}:{1:D2}", timeSpan.Hours, timeSpan.Minutes);
        rolloverTimeText.text = "New Daily Puzzle in " + str;
    }

    IEnumerator FlashSchedule()
    {
        while (true)
        {
            yield return (object)new WaitForSeconds(1f);
            bestTimeText.gameObject.SetActive(false);
            yield return (object)new WaitForSeconds(0.25f);
            bestTimeText.gameObject.SetActive(true);
        }
    }
}



using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DailyGame : MonoBehaviour
{
    public enum PuzzleState
    {
        PUZZLE_PLAYING = 1,
        PUZZLE_PAUSED,
        PUZZLE_COMPLETE_SEQUENCE,
        PUZZLE_COMPLETE
    }

    public GameObject[] emptyStarsArray;
    public GameObject[] fullStarsArray;
    public ParticleSystem particlesCascade;

    [HideInInspector]
    public LeaderboardScript leaderBoardScript;

    public Text guiTimerText;
    public Text movesText;
    public int starAwardIncrement = 5;

    PuzzleState puzzleState = PuzzleState.PUZZLE_PAUSED;
    float startTime;
    int numberOfCells;
    float timer;
    int errorsCount = 0;
    long previousSeconds = 0;

    public static DailyGame Instance;

    static bool isdaily = false;
    void Awake()
    {
        Instance = this;

        
            ResetState(true);

            if (GetNameScript.Instance != null)
            {
                leaderBoardScript = GetNameScript.Instance.leaderBoardScript;
            }
        
    }
    public static void ResetState(bool isDailyArt)
    {
        isdaily = isDailyArt;
        if (Instance != null)
        {
            if (isDailyArt)
            {
                Instance.movesText.gameObject.SetActive(true);
                Instance.guiTimerText.gameObject.SetActive(true);
                Instance.puzzleState = PuzzleState.PUZZLE_PAUSED;
            }
            else
            {
                Instance.movesText.gameObject.SetActive(false);
                Instance.guiTimerText.gameObject.SetActive(false);
                Instance.puzzleState = PuzzleState.PUZZLE_PAUSED;
            }
        }
        //drop the instance when the art is not a daily one
        if (!isdaily)
        {
            Instance = null;
        }
    }

    public static void UpdateGUI(int numOfcells)
    {
        if (Instance != null)
        {
            Instance.gameObject.SetActive(isdaily);
            if (isdaily)
            {
                Instance.StartGame(numOfcells);
            }
        }
    }

    public static bool IsDailyArt()
    {
        return isdaily;
    }

    void Update()
    {
        if (puzzleState == PuzzleState.PUZZLE_PLAYING)
        {
            timer += Time.smoothDeltaTime;
            var timeSpan = TimeSpan.FromSeconds((double)timer);
            // refresh the labels once per second
            if (previousSeconds < (long)timeSpan.TotalSeconds)
            {
                guiTimerText.text = string.Format("Time {0:D2}:{1:D2}", timeSpan.Minutes, timeSpan.Seconds);
                movesText.text = "Errors " + errorsCount.ToString();
                previousSeconds = (long)timeSpan.TotalSeconds;
            }
        }
    }

    public void StartGame(int numOfcells)
    {
        previousSeconds = 0;
        movesText.gameObject.SetActive(true);
        guiTimerText.gameObject.SetActive(true);
        puzzleState = PuzzleState.PUZZLE_PLAYING;
        errorsCount = 0;
        timer = 0;
        startTime = Time.deltaTime;
        numberOfCells = numOfcells;
    }

    public static void IncresetError()
    {
        if (isdaily && Instance != null)
        {
            Instance.errorsCount++;
            Instance.movesText.text = "Errors " + Instance.errorsCount.ToString();
        }
    }

    public void Complete()
    {
        if (puzzleState == PuzzleState.PUZZLE_PLAYING)
        {
            base.StartCoroutine(CompletionSchedule());
            puzzleState = PuzzleState.PUZZLE_COMPLETE_SEQUENCE;

            int starsCount = CalculateStarsWon();
            int oldStarsCount = 0;
            //result is saved as "basename-stars" in the prefs slot of that day
            switch (LevelProgressControl.control.dailyDayOffset)
            {
                case -2:
                    oldStarsCount = GetDailyStars("dailyereyesterdaydata");
                    PlayerPrefs.SetString("dailyereyesterdaydata", LevelProgressControl.control.puzzleBasename + "-" + starsCount);
                    if (Debug.isDebugBuild)
                    {
                        Debug.Log("Ereyesterday COMPLETED");
                    }
                    break;
                case -1:
                    oldStarsCount = GetDailyStars("dailyyesterdaydata");
                    PlayerPrefs.SetString("dailyyesterdaydata", LevelProgressControl.control.puzzleBasename + "-" + starsCount);
                    if (Debug.isDebugBuild)
                    {
                        Debug.Log("Yesterday COMPLETED");
                    }
                    break;
                case 0:
                    {
                        oldStarsCount = GetDailyStars("dailytodaydata");
                        PlayerPrefs.SetString("dailytodaydata", LevelProgressControl.control.puzzleBasename + "-" + starsCount);
                        if (Debug.isDebugBuild)
                        {
                            Debug.Log("Today COMPLETED");
                        }
                        if (timer < LevelProgressControl.control.dailyTodayPersonalBest)
                        {
                            LevelProgressControl.control.dailyTodayPersonalBest = timer;
                            //LevelProgressControl.control.dailyTodayPersonalBestBeaten = true;
                            LevelProgressControl.control.SaveProgressData();
                        }
                        long score = Convert.ToInt64(timer);
                        if (leaderBoardScript != null)
                        {
#if UNITY_ANDROID
                            // google play games leaderboard id
                            this.leaderBoardScript.ReportLeaderboardScore(score, GPGSIds.leaderboard_best_time);
#else
                            //pixelart_dailybest
                            leaderBoardScript.ReportLeaderboardScore(score, "pixelart_dailybest");
#endif
                        }
                        break;
                    }
            }
            LevelProgressControl.control.SaveProgressData();

            //MainMenu.Instance.RefreshTodayArt();
        }
    }

    IEnumerator CompletionSchedule()
    { 
        yield return (object)new WaitForSeconds(1f);
        particlesCascade.Play();
        for (int i = 0; i < emptyStarsArray.Length; i++)
        {
            emptyStarsArray[i].GetComponent<Animation>().Play();
        }
        movesText.CrossFadeAlpha(1f, 0.5f, false); 
        guiTimerText.CrossFadeAlpha(1f, 0.5f, false); 
        yield return (object)new WaitForSeconds(0.6f);
        //fill the won stars one by one
        int starsWon = CalculateStarsWon();
        if (starsWon >= 1)
        {
            fullStarsArray[0].GetComponent<SpriteRenderer>().enabled = true;
            fullStarsArray[0].GetComponent<RadialParticlesScript>().Reset(false);
        }
        yield return (object)new WaitForSeconds(0.2f);
        if (starsWon >= 2)
        {
            fullStarsArray[1].GetComponent<SpriteRenderer>().enabled = true;
            fullStarsArray[1].GetComponent<RadialParticlesScript>().Reset(false);
        }
        yield return (object)new WaitForSeconds(0.2f);
        if (starsWon == 3)
        {
            fullStarsArray[2].GetComponent<SpriteRenderer>().enabled = true;
            fullStarsArray[2].GetComponent<RadialParticlesScript>().Reset(false);
        }
        puzzleState = PuzzleState.PUZZLE_COMPLETE; 
    }
    int CalculateStarsWon()
    {
        // faster time and fewer errors give more stars
        float duration = Time.deltaTime - startTime;
        if (duration <= numberOfCells + starAwardIncrement && errorsCount < 3)
        {
            return 3;
        }
        if (duration <= numberOfCells + starAwardIncrement * 2 && errorsCount < 10)
        {
            return 2;
        }
        if (duration <= numberOfCells + starAwardIncrement * 3)
        {
            return 1;
        }
        return 0;
    }

    int GetDailyStars(string prefname)
    {
        string str = PlayerPrefs.GetString(prefname, "nodata");
        var parts = str.Split('-');
        return Convert.ToInt32(parts[2]);
    }

}

using UnityDebug = global::UnityEngine.Debug;
#if UNITY_ANDROID
using GooglePlayGames;
using GooglePlayGames.BasicApi;
#elif UNITY_IOS
using UnityEngine.SocialPlatforms;
using UnityEngine.SocialPlatforms.GameCenter;
#endif
using System;
using UnityEngine;

public class LeaderboardScript : MonoBehaviour
{
    [Flags]
    public enum AchievementFlags
    {
        COMPLETE_C1 = 1,
        COMPLETE_C2 = 2,
        COMPLETE_C3 = 4,
        THREESTAR_C1 = 8,
        THREESTAR_C2 = 0x10,
        THREESTAR_C3 = 0x20
    }

    private void Start()
    {   
        this.InitLeaderboards(); 
    }

    //public void CheckForPastAchievements()
    //{
    //    if (this.IsAuthenticated())
    //    {
    //        UnityDebug.Log("CheckForPastAchievements... IsAuthenticated YES");
    //        int @int = PlayerPrefs.GetInt("CheckedPastAch", 0);
    //        AchievementFlags achievementFlags = (AchievementFlags)@int;
    //        UnityDebug.Log("CheckForPastAchievements : CheckedPastAch Pref : " + @int);
    //        UnityDebug.Log("Achievement flags : " + achievementFlags);
    //        if ((achievementFlags & AchievementFlags.COMPLETE_C1) == (AchievementFlags)0 && this.TestChapterComplete(1))
    //        {
    //            UnityDebug.Log("CheckForPastAchievements... awarding CgkIifzNtrcZEAIQAw");
    //            this.ReportAchievementProgress("CgkIifzNtrcZEAIQAw", 100.0);
    //            achievementFlags |= AchievementFlags.COMPLETE_C1;
    //        }
    //        if ((achievementFlags & AchievementFlags.COMPLETE_C2) == (AchievementFlags)0 && this.TestChapterComplete(2))
    //        {
    //            UnityDebug.Log("CheckForPastAchievements... awarding CgkIifzNtrcZEAIQBA");
    //            this.ReportAchievementProgress("CgkIifzNtrcZEAIQBA", 100.0);
    //            achievementFlags |= AchievementFlags.COMPLETE_C2;
    //        }
    //        if ((achievementFlags & AchievementFlags.COMPLETE_C3) == (AchievementFlags)0 && this.TestChapterComplete(3))
    //        {
    //            UnityDebug.Log("CheckForPastAchievements... awarding CgkIifzNtrcZEAIQBQ");
    //            this.ReportAchievementProgress("CgkIifzNtrcZEAIQBQ", 100.0);
    //            achievementFlags |= AchievementFlags.COMPLETE_C3;
    //        }
    //        if ((achievementFlags & AchievementFlags.THREESTAR_C1) == (AchievementFlags)0 && this.TestChapter3Starred(1))
    //        {
    //            UnityDebug.Log("CheckForPastAchievements... awarding CgkIifzNtrcZEAIQBg");
    //            this.ReportAchievementProgress("CgkIifzNtrcZEAIQBg", 100.0);
    //            achievementFlags |= AchievementFlags.THREESTAR_C1;
    //        }
    //        if ((achievementFlags & AchievementFlags.THREESTAR_C2) == (AchievementFlags)0 && this.TestChapter3Starred(2))
    //        {
    //            UnityDebug.Log("CheckForPastAchievements... awarding CgkIifzNtrcZEAIQBw");
    //            this.ReportAchievementProgress("CgkIifzNtrcZEAIQBw", 100.0);
    //            achievementFlags |= AchievementFlags.THREESTAR_C2;
    //        }
    //        if ((achievementFlags & AchievementFlags.THREESTAR_C3) == (AchievementFlags)0 && this.TestChapter3Starred(3))
    //        {
    //            UnityDebug.Log("CheckForPastAchievements... awarding CgkIifzNtrcZEAIQCA");
    //            this.ReportAchievementProgress("CgkIifzNtrcZEAIQCA", 100.0);
    //            achievementFlags |= AchievementFlags.THREESTAR_C3;
    //        }
    //        PlayerPrefs.SetInt("CheckedPastAch", (int)achievementFlags);
    //        PlayerPrefs.Save();
    //    }
    //    else
    //    {
    //        UnityDebug.Log("CheckForPastAchievements... IsAuthenticated NO");
    //    }
    //}

    public void InitLeaderboards()
    {
        if (!this.IsAuthenticated())
        {
            try
            {
                Social.localUser.Authenticate(new Action<bool>(this.ProcessAuthentication));
            }
            catch (Exception ex)
            {
                Debug.LogError(ex);
            }
        }
        else
        {
            UnityDebug.Log("InitLeaderboards :  Already Authenticated");
        }
    }

    public bool IsAuthenticated()
    {
        if (Social.localUser.authenticated)
        {
            return true;
        }
        return false;
    }

    private void ProcessAuthentication(bool success)
    {
        if (success)
        {
            UnityDebug.Log("ProcessAuthentication - success");
            //this.CheckForPastAchievements();
        }
        else
        {
            UnityDebug.Log("ProcessAuthentication - failed");
        }
    }

    public void ShowAchievements()
    {
        if (this.IsAuthenticated())
        {
            Social.ShowAchievementsUI();
        }
    }

    public void ShowLeaderboards()
    {
        if (this.IsAuthenticated())
        {
            Social.ShowLeaderboardUI();
        }
        else
        {
            try
            {
                Social.localUser.Authenticate((b) =>
                {
                    if (b)
                    {
                        Social.ShowLeaderboardUI();
                    }
                });
            }
            catch (Exception ex)
            {
                Debug.LogError(ex);
            }
        }
    }

    public void ReportLeaderboardScore(long score, string leaderBoardID)
    {
        if (this.IsAuthenticated())
        {
            Social.ReportScore(score, leaderBoardID, delegate(bool success)
            {
                if (success)
                {
                    UnityDebug.Log("Reported time successfully");
                }
                else
                {
                    UnityDebug.Log("Failed to report time " + leaderBoardID);
                }
            });
        }
        else
        {
            UnityDebug.Log("ReportLeaderboardScore NOT authenticated");
        }
    }

    //public void ReportAchievementProgress(string achievementID, double progress)
    //{
    //    if (this.IsAuthenticated())
    //    {
    //        Social.ReportProgress(achievementID, progress, delegate(bool success)
    //        {
    //            if (success)
    //            {
    //                UnityDebug.Log("Reported achievement successfully");
    //            }
    //            else
    //            {
    //                UnityDebug.Log("Failed to report achievement " + achievementID);
    //            }
    //        });
    //    }
    //    else
    //    {
    //        UnityDebug.Log("ReportAchievementProgress NOT authenticated");
    //    }
    //}

    public void LeaderboardLogout()
    {
#if (UNITY_ANDROID || (UNITY_IPHONE && !NO_GPGS))
        ((GooglePlayGames.PlayGamesPlatform)Social.Active).SignOut();
#endif
    }

    public int GetChapterNumber(int postcardIndex)
    {
        return postcardIndex / 6 + 1;
    }

    public bool TestChapterComplete(int num)
    {
        UnityDebug.Log("TestChapterComplete : " + num);
        int value2 = 0;
        int value3 = (num - 1) * 6;
        for (int i = value3; i < value3 + 6; i++)
        {
            value2 += LevelProgressControl.control.postcardCompleteStateArray[i];
        }
        if (value2 == 12)
        {
            return true;
        }
        return false;
    }

    public bool TestChapter3Starred(int num)
    {
        UnityDebug.Log("TestChapter3Starred : " + num);
        int value2 = 0;
        int value3 = (num - 1) * 72;
        int value4 = value3 + 72;
        for (int i = value3; i < value4; i++)
        {
            value2 += LevelProgressControl.control.starsArray[i];
        }
        if (value2 == 216)
        {
            return true;
        }
        return false;
    }

    //public void TestChapter3StarredFromPostcard(int pcIdx)
    //{
    //    UnityDebug.Log("TestChapter3StarredFromPostcard : " + pcIdx);
    //    int @int = PlayerPrefs.GetInt("CheckedPastAch", 0);
    //    AchievementFlags achievementFlags = (AchievementFlags)@int;
    //    int chapterNumber = this.GetChapterNumber(pcIdx);
    //    switch (chapterNumber)
    //    {
    //        default:
    //            return;
    //        case 1:
    //            if ((achievementFlags & AchievementFlags.THREESTAR_C1) == (AchievementFlags)0 && this.TestChapter3Starred(chapterNumber))
    //            {
    //                this.ReportAchievementProgress("CgkIifzNtrcZEAIQBg", 100.0);
    //                achievementFlags |= AchievementFlags.THREESTAR_C1;
    //            }
    //            break;
    //        case 2:
    //            if ((achievementFlags & AchievementFlags.THREESTAR_C2) == (AchievementFlags)0 && this.TestChapter3Starred(chapterNumber))
    //            {
    //                this.ReportAchievementProgress("CgkIifzNtrcZEAIQBw", 100.0);
    //                achievementFlags |= AchievementFlags.THREESTAR_C2;
    //            }
    //            break;
    //        case 3:
    //            if ((achievementFlags & AchievementFlags.THREESTAR_C3) == (AchievementFlags)0 && this.TestChapter3Starred(chapterNumber))
    //            {
    //                this.ReportAchievementProgress("CgkIifzNtrcZEAIQCA", 100.0);
    //                achievementFlags |= AchievementFlags.THREESTAR_C3;
    //            }
    //            break;
    //    }
    //    PlayerPrefs.SetInt("CheckedPastAch", (int)achievementFlags);
    //    PlayerPrefs.Save();
    //}

    //public void TestChapterCompleteFromPostcard(int pcIdx)
    //{
    //    UnityDebug.Log("TestChapterCompleteFromPostcard : " + pcIdx);
    //    int @int = PlayerPrefs.GetInt("CheckedPastAch", 0);
    //    AchievementFlags achievementFlags = (AchievementFlags)@int;
    //    int chapterNumber = this.GetChapterNumber(pcIdx);
    //    switch (chapterNumber)
    //    {
    //        default:
    //            return;
    //        case 1:
    //            if ((achievementFlags & AchievementFlags.COMPLETE_C1) == (AchievementFlags)0 && this.TestChapterComplete(chapterNumber))
    //            {
    //                this.ReportAchievementProgress("CgkIifzNtrcZEAIQAw", 100.0);
    //                achievementFlags |= AchievementFlags.COMPLETE_C1;
    //            }
    //            break;
    //        case 2:
    //            if ((achievementFlags & AchievementFlags.COMPLETE_C2) == (AchievementFlags)0 && this.TestChapterComplete(chapterNumber))
    //            {
    //                this.ReportAchievementProgress("CgkIifzNtrcZEAIQBA", 100.0);
    //                achievementFlags |= AchievementFlags.COMPLETE_C2;
    //            }
    //            break;
    //        case 3:
    //            if ((achievementFlags & AchievementFlags.COMPLETE_C3) == (AchievementFlags)0 && this.TestChapterComplete(chapterNumber))
    //            {
    //                this.ReportAchievementProgress("CgkIifzNtrcZEAIQBQ", 100.0);
    //                achievementFlags |= AchievementFlags.COMPLETE_C3;
    //            }
    //            break;
    //    }
    //    PlayerPrefs.SetInt("CheckedPastAch", (int)achievementFlags);
    //    PlayerPrefs.Save();
    //}
}



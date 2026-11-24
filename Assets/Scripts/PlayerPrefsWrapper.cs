using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class PlayerPrefsWrapper
{
    static string s_listSeparator = "\n";

    public static bool Exists(string key)
    {
        return PlayerPrefs.HasKey(key);
    }

    public static string GetString(string key)
    {
        return PlayerPrefs.GetString(key);
    }

    public static void SetString(string key, string value)
    {
        PlayerPrefs.SetString(key, value);
    }

    //bools are saved as "1" or "0" strings, lists as one string split by new lines
    public static bool GetBool(string key)
    {
        return PlayerPrefs.GetString(key) == "1";
    }

    public static void SetBool(string key, bool value)
    {
        PlayerPrefs.SetString(key, (!value) ? "0" : "1");
    }

    public static int GetInt(string key)
    {
        return PlayerPrefs.GetInt(key);
    }

    public static void SetInt(string key, int value)
    {
        PlayerPrefs.SetInt(key, value);
    }

    public static List<string> GetStringList(string key)
    {
        string @string = GetString(key);
        if (string.IsNullOrEmpty(@string))
        {
            return new List<string>();
        }
        return @string.Split(new string[1] {
            s_listSeparator
        }, StringSplitOptions.RemoveEmptyEntries).ToList();
    }

    public static void SetStringList(string key, List<string> list)
    {
        if (list != null)
        {
            var stringBuilder = new StringBuilder();
            foreach (string item in list)
            {
                stringBuilder.Append(item).Append(s_listSeparator);
            }
            SetString(key, stringBuilder.ToString());
        }
    }

    public static void Clear()
    {
        PlayerPrefs.DeleteAll();
    }
}



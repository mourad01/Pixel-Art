using UnityDebug = global::UnityEngine.Debug;
using System;
using System.Collections.Generic;

public class DeepLinkWrapper
{
    public static string DeepLink { get; private set; }

    public static void Init()
    {
    }

    public static Dictionary<string, string> Parse(string par)
    {
        UnityDebug.Log(par);
        if (!string.IsNullOrEmpty(par))
        {
            var uri = new Uri(par);
            if (uri.Host == "main")
            {
                string text = uri.Query.Replace("?", string.Empty);
                var parts = text.Split('&');
                var dictionary = new Dictionary<string, string>();
                dictionary.Add("host", uri.Host);
                var pairs = parts;
                foreach (string pair in pairs)
                {
                    var keyValue = pair.Split('=');
                    dictionary.Add(keyValue[0], keyValue[1]);
                }
                return dictionary;
            }
        }
        return null;
    }
}



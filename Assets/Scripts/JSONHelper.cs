using System;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

internal static class JSONHelper
{
    public static JSONObject ConvertToJSON(List<string> collection)
    {
        var jSONObject = new JSONObject(JSONObject.Type.ARRAY);
        foreach (string item in collection)
        {
            jSONObject.Add(item);
        }
        return jSONObject;
    }

    public static List<string> ConvertToStringList(JSONObject jsonData)
    {
        var list = new List<string>();
        foreach (JSONObject jsonObject in jsonData.list)
        {
            list.Add(jsonObject.str);
        }
        return list;
    }

    public static JSONObject ConvertToJSON(Dictionary<string, bool> collection)
    {
        var jSONObject = new JSONObject(JSONObject.Type.OBJECT);
        foreach (KeyValuePair<string, bool> keyValuePair in collection)
        {
            jSONObject.AddField(keyValuePair.Key, keyValuePair.Value);
        }
        return jSONObject;
    }

    public static Dictionary<string, bool> ConvertToBoolDictionary(JSONObject jsonData)
    {
        var dictionary = new Dictionary<string, bool>();
        if (!jsonData.IsNull)
        {
            {
                foreach (string key in jsonData.keys)
                {
                    dictionary.Add(key, jsonData[key].b);
                }
                return dictionary;
            }
        }
        return dictionary;
    }

    public static JSONObject ConvertToJSON(Dictionary<string, int> collection)
    {
        var jSONObject = new JSONObject(JSONObject.Type.OBJECT);
        //ints are stored as strings, ConvertToIntDictionary parses them back
        foreach (KeyValuePair<string, int> item in collection)
        {
            jSONObject.AddField(item.Key, item.Value.ToString());
        }
        return jSONObject;
    }

    public static JSONObject ConvertToJSON(Dictionary<string, object> collection)
    {
        var jSONObject = new JSONObject(JSONObject.Type.OBJECT);
        foreach (KeyValuePair<string, object> item in collection)
        {
            jSONObject.AddField(item.Key, item.Value.ToString());
        }
        return jSONObject;
    }

    public static JSONObject ConvertToJSON(Dictionary<int, int> collection)
    {
        var jSONObject = new JSONObject(JSONObject.Type.OBJECT);
        foreach (KeyValuePair<int, int> item in collection)
        {
            jSONObject.AddField(item.Key.ToString(), item.Value.ToString());
        }
        return jSONObject;
    }

    public static Dictionary<int, int> ConvertToIntDictionary(JSONObject jsonData)
    {
        var dictionary = new Dictionary<int, int>();
        if (!jsonData.IsNull)
        {
            {
                foreach (string key in jsonData.keys)
                {
                    dictionary.Add(int.Parse(key), int.Parse(jsonData[key].str));
                }
                return dictionary;
            }
        }
        return dictionary;
    }

    public static Dictionary<string, string> ConvertToStrDictionary(JSONObject jsonData)
    {
        var dictionary = new Dictionary<string, string>();
        if (!jsonData.IsNull)
        {
            {
                foreach (string key in jsonData.keys)
                {
                    dictionary.Add(key, jsonData[key].str);
                }
                return dictionary;
            }
        }
        return dictionary;
    }

    public static JSONObject ConvertToJSON(Dictionary<string, DateTime> collection)
    {
        var jSONObject = new JSONObject(JSONObject.Type.OBJECT);
        // invariant format so dates parse the same on every device locale
        foreach (KeyValuePair<string, DateTime> item in collection)
        {
            jSONObject.AddField(item.Key, item.Value.ToString(NumberFormatInfo.InvariantInfo));
        }
        return jSONObject;
    }

    public static JSONObject ConvertToJSON(Dictionary<int, DateTime> collection)
    {
        var dictionary = new Dictionary<string, string>();
        foreach (KeyValuePair<int, DateTime> item in collection)
        {
            dictionary.Add(item.Key.ToString(), item.Value.ToString(NumberFormatInfo.InvariantInfo));
        }
        return new JSONObject(dictionary);
    }

    public static Dictionary<string, DateTime> ConvertToStringDateTimeDictionary(JSONObject jsonData)
    {
        var dictionary = new Dictionary<string, DateTime>();
        if (!jsonData.IsNull)
        {
            {
                foreach (string key in jsonData.keys)
                {
                    dictionary.Add(key, DateTime.Parse(jsonData[key].str));
                }
                return dictionary;
            }
        }
        return dictionary;
    }

    public static Dictionary<int, DateTime> ConvertToIntDateTimeDictionary(JSONObject jsonData)
    {
        var dictionary = new Dictionary<int, DateTime>();
        foreach (string key in jsonData.keys)
        {
            dictionary.Add(int.Parse(key), DateTime.Parse(jsonData[key].str));
        }
        return dictionary;
    }

    public static JSONObject ConvertToJSON(List<int> collection)
    {
        var jSONObject = new JSONObject(JSONObject.Type.ARRAY);
        foreach (int item in collection)
        {
            jSONObject.Add(item);
        }
        return jSONObject;
    }

    public static List<int> ConvertToIntList(JSONObject jsonData)
    {
        var list = new List<int>();
        //numbers come back as float
        foreach (JSONObject jsonObject in jsonData.list)
        {
            list.Add((int)jsonObject.f);
        }
        return list;
    }

    public static JSONObject ConvertToJSON(List<byte> collection)
    {
        var jSONObject = new JSONObject(JSONObject.Type.ARRAY);
        foreach (byte item in collection)
        {
            jSONObject.Add(item);
        }
        return jSONObject;
    }

    public static List<byte> ConvertToByteList(JSONObject jsonData)
    {
        var list = new List<byte>();
        foreach (JSONObject jsonObject in jsonData.list)
        {
            list.Add((byte)jsonObject.f);
        }
        return list;
    }

    public static JSONObject ConvertToJSON(Vector3 vector)
    {
        var jSONObject = new JSONObject();
        jSONObject.AddField("x", vector.x);
        jSONObject.AddField("y", vector.y);
        jSONObject.AddField("z", vector.z);
        return jSONObject;
    }

    public static Vector3 ConvertToVector3(JSONObject jsonData)
    {
        Vector3 zero = Vector3.zero;
        zero.x = jsonData["x"].f;
        zero.y = jsonData["y"].f;
        zero.z = jsonData["z"].f;
        return zero;
    }

    public static JSONObject ConvertToJSON(Quaternion quaternion)
    {
        var jSONObject = new JSONObject();
        jSONObject.AddField("x", quaternion.x);
        jSONObject.AddField("y", quaternion.y);
        jSONObject.AddField("z", quaternion.z);
        jSONObject.AddField("w", quaternion.w);
        return jSONObject;
    }

    public static Quaternion ConvertToQuaternion(JSONObject jsonData)
    {
        Quaternion result = default(Quaternion);
        result.x = jsonData["x"].f;
        result.y = jsonData["y"].f;
        result.z = jsonData["z"].f;
        result.w = jsonData["w"].f;
        return result;
    }
}



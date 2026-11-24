using UnityDebug = global::UnityEngine.Debug;
using System;
public class DebugLogger
{
    //set to false to mute every log below
    public static bool IsDevelopmentBuild = true;

    public static void Log(object message)
    {
        if (IsDevelopmentBuild)
        {
            UnityDebug.Log(message);
        }
    }

    public static void Log(object message, UnityEngine.Object obj)
    {
        if (IsDevelopmentBuild)
        {
            UnityDebug.Log(message, obj);
        }
    }

    public static void LogWarning(object message)
    {
        if (IsDevelopmentBuild)
        {
            UnityDebug.LogWarning(message);
        }
    }

    public static void LogWarning(object message, UnityEngine.Object obj)
    {
        if (IsDevelopmentBuild)
        {
            UnityDebug.LogWarning(message, obj);
        }
    }

    public static void LogError(object message)
    {
        if (IsDevelopmentBuild)
        {
            UnityDebug.LogError(message);
        }
    }

    public static void LogError(object message, UnityEngine.Object obj)
    {
        if (IsDevelopmentBuild)
        {
            UnityDebug.LogError(message, obj);
        }
    }

    public static void LogException(Exception exception)
    {
        if (IsDevelopmentBuild)
        {
            UnityDebug.LogException(exception);
        }
    }

    public static void LogException(Exception exception, UnityEngine.Object obj)
    {
        if (IsDevelopmentBuild)
        {
            UnityDebug.LogException(exception, obj);
        }
    }
}

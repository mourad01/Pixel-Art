using UnityDebug = global::UnityEngine.Debug;
using System;
public class DebugLogger
{
    public static bool IsDevelopmentBuild = true;

    public static void Log(object message)
    {
        if (DebugLogger.IsDevelopmentBuild)
        {
            UnityDebug.Log(message);
        }
    }

    public static void Log(object message, UnityEngine.Object obj)
    {
        if (DebugLogger.IsDevelopmentBuild)
        {
            UnityDebug.Log(message, obj);
        }
    }

    public static void LogWarning(object message)
    {
        if (DebugLogger.IsDevelopmentBuild)
        {
            UnityDebug.LogWarning(message);
        }
    }

    public static void LogWarning(object message, UnityEngine.Object obj)
    {
        if (DebugLogger.IsDevelopmentBuild)
        {
            UnityDebug.LogWarning(message, obj);
        }
    }

    public static void LogError(object message)
    {
        if (DebugLogger.IsDevelopmentBuild)
        {
            UnityDebug.LogError(message);
        }
    }

    public static void LogError(object message, UnityEngine.Object obj)
    {
        if (DebugLogger.IsDevelopmentBuild)
        {
            UnityDebug.LogError(message, obj);
        }
    }

    public static void LogException(Exception exception)
    {
        if (DebugLogger.IsDevelopmentBuild)
        {
            UnityDebug.LogException(exception);
        }
    }

    public static void LogException(Exception exception, UnityEngine.Object obj)
    {
        if (DebugLogger.IsDevelopmentBuild)
        {
            UnityDebug.LogException(exception, obj);
        }
    }
}

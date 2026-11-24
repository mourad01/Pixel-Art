using System;
using System.Collections.Generic;
using UnityEngine;

public static class AnalyticsHelper
{ 
    public static bool IsTestDevice()
    {

        return false;
    }

    public static string GetPlatform()
    {
        if (RuntimePlatformIs("OSXEditor"))
        {
            return "MAC_CLIENT";
        }
        if (RuntimePlatformIs("OSXPlayer"))
        {
            return "MAC_CLIENT";
        }
        if (RuntimePlatformIs("WindowsPlayer"))
        {
            return "PC_CLIENT";
        }
        if (RuntimePlatformIs("OSXWebPlayer"))
        {
            return "WEB";
        }
        if (RuntimePlatformIs("OSXDashboardPlayer"))
        {
            return "MAC_CLIENT";
        }
        if (RuntimePlatformIs("WindowsWebPlayer"))
        {
            return "WEB";
        }
        if (RuntimePlatformIs("WindowsEditor"))
        {
            return "PC_CLIENT";
        }
        if (RuntimePlatformIs("IPhonePlayer"))
        {
            if (SystemInfo.deviceModel.Contains("iPad"))
            {
                return "IOS_TABLET";
            }
            return "IOS_MOBILE";
        }
        if (RuntimePlatformIs("PS3"))
        {
            return "PS3";
        }
        if (RuntimePlatformIs("XBOX360"))
        {
            return "XBOX360";
        }
        if (RuntimePlatformIs("Android"))
        {
            return (!IsTablet()) ? "ANDROID_MOBILE" : "ANDROID_TABLET";
        }
        if (RuntimePlatformIs("NaCL"))
        {
            return "WEB";
        }
        if (RuntimePlatformIs("LinuxPlayer"))
        {
            return "PC_CLIENT";
        }
        if (RuntimePlatformIs("WebGLPlayer"))
        {
            return "WEB";
        }
        if (RuntimePlatformIs("FlashPlayer"))
        {
            return "WEB";
        }
        if (!RuntimePlatformIs("MetroPlayerX86") && !RuntimePlatformIs("MetroPlayerX64") && !RuntimePlatformIs("MetroPlayerARM") && !RuntimePlatformIs("WSAPlayerX86") && !RuntimePlatformIs("WSAPlayerX64") && !RuntimePlatformIs("WSAPlayerARM"))
        {
            if (RuntimePlatformIs("WP8Player"))
            {
                return (!IsTablet()) ? "WINDOWS_MOBILE" : "WINDOWS_TABLET";
            }
            if (!RuntimePlatformIs("BB10Player") && !RuntimePlatformIs("BlackBerryPlayer"))
            {
                if (RuntimePlatformIs("TizenPlayer"))
                {
                    return (!IsTablet()) ? "ANDROID_MOBILE" : "ANDROID_TABLET";
                }
                if (RuntimePlatformIs("PSP2"))
                {
                    return "PSVITA";
                }
                if (RuntimePlatformIs("PS4"))
                {
                    return "PS4";
                }
                if (RuntimePlatformIs("PSMPlayer"))
                {
                    return "WEB";
                }
                if (RuntimePlatformIs("XboxOne"))
                {
                    return "XBOXONE";
                }
                if (RuntimePlatformIs("SamsungTVPlayer"))
                {
                    return "ANDROID_CONSOLE";
                }
                return "UNKNOWN";
            }
            return (!IsTablet()) ? "BLACKBERRY_MOBILE" : "BLACKBERRY_TABLET";
        }
        if (SystemInfo.deviceType == DeviceType.Handheld)
        {
            return (!IsTablet()) ? "WINDOWS_MOBILE" : "WINDOWS_TABLET";
        }
        return "PC_CLIENT";
    }

    static bool RuntimePlatformIs(string platformName)
    {
        return Enum.IsDefined(typeof(RuntimePlatform), platformName) && Application.platform.ToString() == platformName;
    }

    static float ScreenSizeInches()
    {
        float widthInches = (float)Screen.width / Screen.dpi;
        float heightInches = (float)Screen.height / Screen.dpi;
        return (float)Math.Sqrt((double)(widthInches * widthInches + heightInches * heightInches));
    }

    static bool IsTablet()
    {
        return ScreenSizeInches() > 6f;
    }
}



using UnityEngine;

public class ScreenToolWrapper
{
    // private static float s_density = -1f;

    public static float Density
    {
        get
        {
            return Screen.dpi;
        }
    }

    public static float IphoneXExtraPixels
    {
        get
        {
            return 0f;
        }
    }

    public static float IphoneXExtraBottomPixels
    {
        get
        {
            return 0f;
        }
    }

    public static bool IsIphoneX
    {
        get
        {
            return false;
        }
    }
}



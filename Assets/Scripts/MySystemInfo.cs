using UnityEngine;

public class MySystemInfo
{
    static bool? s_IsTablet;

    public static bool IsTablet
    {
        get
        {
            if (!s_IsTablet.HasValue)
            {
                s_IsTablet = (DeviceDiagonalSizeInInches() > 6.5f);
            }
            return s_IsTablet.Value;
        }
    }

    static float DeviceDiagonalSizeInInches()
    {
        float f = (float)Screen.width / Screen.dpi;
        float heightInches = (float)Screen.height / Screen.dpi;
        return Mathf.Sqrt(Mathf.Pow(f, 2f) + Mathf.Pow(heightInches, 2f));
    }
}



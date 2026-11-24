using UnityEngine;

public class MySystemInfo
{
    static bool? s_IsTablet;

    public static bool IsTablet
    {
        get
        {
            var nullable = s_IsTablet;
            if (!nullable.HasValue)
            {
                s_IsTablet = (DeviceDiagonalSizeInInches() > 6.5f);
            }
            var isTablet = s_IsTablet;
            return isTablet.Value;
        }
    }

    static float DeviceDiagonalSizeInInches()
    {
        float f = (float)Screen.width / Screen.dpi;
        float f2 = (float)Screen.height / Screen.dpi;
        return Mathf.Sqrt(Mathf.Pow(f, 2f) + Mathf.Pow(f2, 2f));
    }
}



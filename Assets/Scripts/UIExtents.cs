using UnityEngine;
using UnityEngine.UI;

public static class UIExtents
{
    public static void SetAlpha(this MaskableGraphic graphic, float alpha)
    {
        Color color = graphic.color;
        color.a = alpha;
        graphic.color = color;
    }

    public static bool IsDark(this Color color)
    {
        double luminance = 1.0 - (0.299 * (double)color.r + 0.587 * (double)color.g + 0.114 * (double)color.b);
        return luminance >= 0.5;
    }
}



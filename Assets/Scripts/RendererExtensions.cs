using UnityEngine;

public static class RendererExtensions
{
    static int CountCornersVisibleFrom(this RectTransform rectTransform, Camera camera)
    {
        Rect rect = new Rect(0f, 0f, (float)Screen.width, (float)Screen.height);
        var corners = new Vector3[4];
        rectTransform.GetWorldCorners(corners);
        int visibleCount = 0;
        for (int i = 0; i < corners.Length; i++)
        {
            Vector3 point = camera.WorldToScreenPoint(corners[i]);
            if (rect.Contains(point))
            {
                visibleCount++;
            }
        }
        return visibleCount;
    }

    public static bool IsFullyVisibleFrom(this RectTransform rectTransform, Camera camera)
    {
        return rectTransform.CountCornersVisibleFrom(camera) == 4;
    }

    public static bool IsVisibleFrom(this RectTransform rectTransform, Camera camera)
    {
        return rectTransform.CountCornersVisibleFrom(camera) > 0;
    }
}



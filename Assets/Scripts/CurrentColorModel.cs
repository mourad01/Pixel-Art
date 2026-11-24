using System;
using UnityEngine;

public class CurrentColorModel
{
    public Action<CurrentColorModel, bool> OnStateChanged;

    public Color Color { get;  set; }

    public void UpdateColor(Color color)
    {
        Color = color;
        OnStateChanged.SafeInvoke(this, true);
    }
}



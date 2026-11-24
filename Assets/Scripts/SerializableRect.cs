using System;
using UnityEngine;

[Serializable]
public class SerializableRect
{
    public float X { get; set; }

    public float Y { get; set; }

    public float Width { get; set; }

    public float Height { get; set; }

    public SerializableRect(Rect rect)
    {
        X = rect.x;
        Y = rect.y;
        Width = rect.width;
        Height = rect.height;
    }

    public Rect ToRect()
    {
        return new Rect(X, Y, Width, Height);
    }
}



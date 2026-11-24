using System;
using UnityEngine;

[Serializable]
public class SerializableVector3
{
    public float X { get; set; }

    public float Y { get; set; }

    public float Z { get; set; }

    public SerializableVector3(Vector3 vec)
    {
        X = vec.x;
        Y = vec.y;
        Z = vec.z;
    }

    public SerializableVector3(float x, float y, float z)
    {
        X = x;
        Y = y;
        Z = z;
    }

    public Vector3 ToVector3()
    {
        return new Vector3(X, Y, Z);
    }
}



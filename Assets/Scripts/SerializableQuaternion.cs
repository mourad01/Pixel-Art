using System;
using UnityEngine;

[Serializable]
public class SerializableQuaternion
{
    public float X { get; set; }

    public float Y { get; set; }

    public float Z { get; set; }

    public float W { get; set; }

    public SerializableQuaternion(Quaternion vec)
    {
        X = vec.x;
        Y = vec.y;
        Z = vec.z;
        W = vec.w;
    }

    public SerializableQuaternion(float x, float y, float z, float w)
    {
        X = x;
        Y = y;
        Z = z;
        W = w;
    }

    public Quaternion ToQuaternion()
    {
        return new Quaternion(X, Y, Z, W);
    }
}



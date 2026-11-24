using System;

[Serializable]
public class CashImage
{
    public byte[] Bytes { get; set; }

    public CashImage(byte[] bytes)
    {
        Bytes = bytes;
    }
}



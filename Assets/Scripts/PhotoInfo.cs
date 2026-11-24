using System;

[Serializable]
public class PhotoInfo
{
    public string Id { get; set; }

    public PhotoSource Source { get; set; }

    public PhotoInfo(string id, PhotoSource source)
    {
        Id = id;
        Source = source;
    }
}



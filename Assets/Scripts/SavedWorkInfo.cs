using System;

[Serializable]
public class SavedWorkInfo
{
    public string Id { get; set; }

    public DateTime DateTime { get; set; }

    public SavedWorkInfo(string id, DateTime dateTime)
    {
        Id = id;
        DateTime = dateTime;
    }
}



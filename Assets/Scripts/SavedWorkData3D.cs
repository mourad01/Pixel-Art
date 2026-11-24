

using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SavedWorkData3D : ISavedWorkData
{
    [NonSerialized]
    History m_history;

    public string Id { get; private set; }

    public ImageInfo ImageInfo { get; private set; }

    public string FileName { get; private set; }

    public byte[] Preview { get; private set; }

    public bool Completed { get; private set; }

    public int FilterId { get; set; }

    public Rect UvRect1
    {
        get
        {
            if (UvRect != null)
            {
                return UvRect.ToRect();
            }
            return new Rect(0f, 0f, 1f, 1f);
        }
    }

    public SerializableRect UvRect { get; set; }

    [Obsolete("History saves in enother file now. Use History2")]
    public List<HistoryStep> History { get; private set; }

    public History History2
    {
        get
        {
            return m_history;
        }
        private set
        {
            m_history = value;
        }
    }

    //one flag per vox cube, true when it is colored
    public List<bool> Progress { get; private set; }

    public bool Is3D
    {
        get
        {
            return ImageInfo.Is3D;
        }
    }

    public string FullFileName
    {
        get
        {
            return SavedWorksList.GetPathToSave(Id);
        }
    }

    public SavedWorkData3D()
    {
        Id = Guid.NewGuid().ToString();
        UvRect = new SerializableRect(new Rect(0f, 0f, 1f, 1f));
        Progress = new List<bool>();
    }

    public void Init(ImageInfo imageInfo, byte[] preview, bool completed, List<HistoryStep> history)
    {
        ImageInfo = imageInfo;
        Preview = preview;
        Completed = completed;
    }

    public void Init(ImageInfo imageInfo, byte[] preview, bool completed, List<HistoryStep> history, List<bool> progress)
    {
        ImageInfo = imageInfo;
        Preview = preview;
        Completed = completed;
        Progress = progress;
    }

    public static bool IsNeedToSave(NumberColoring go)
    {
        if (go == null)
        {
            return false;
        }
        return !go.IsEmpty();
    }

    public void SetFile(string file)
    {
        FileName = file;
    }

    public void Apply(NumberColoring nc)
    {
        var transform = nc.transform;
        var texture2D = new Texture2D(1, 1, TextureFormat.ARGB32, false);
        texture2D.filterMode = FilterMode.Point;
        texture2D.LoadImage(Preview);
    }

    public void SetImageInfo(ImageInfo ii)
    {
        ImageInfo = ii;
    }

    // 3D works keep no history file
    public void InitHistory()
    {
    }

    public override string ToString()
    {
        return "SavedWorkData3D " + Id;
    }
}



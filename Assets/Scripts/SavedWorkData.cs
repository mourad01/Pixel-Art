

using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SavedWorkData : ISavedWorkData
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

    public SavedWorkData()
    {
        Id = Guid.NewGuid().ToString();
        UvRect = new SerializableRect(new Rect(0f, 0f, 1f, 1f));
    }

    public void Init(ImageInfo imageInfo, byte[] preview, bool completed, List<HistoryStep> history)
    {
        ImageInfo = imageInfo;
        Preview = preview;
        Completed = completed;
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
        History2 = new History();
        History2.Init(FullFileName + ".his");
        //move the old in-save history to the .his file, or rebuild it for very old saves
        if (History != null && History.Count > 0)
        {
            foreach (HistoryStep historyStep in History)
            {
                History2.AddStep(historyStep);
            }
            History = null;
        }
        else if (History2.Steps.Count == 0)
        {
            GenerateHistoryForOldSave();
        }
        nc.ApplyResTexture(texture2D, Completed, History2);
    }

    void GenerateHistoryForOldSave()
    {
        var texture2D = new Texture2D(1, 1, TextureFormat.ARGB32, false);
        texture2D.LoadImage(Preview);
        var pixels = texture2D.GetPixels();
        int width = texture2D.width;
        //one step per colored pixel of the preview, order is unknown so it gets shuffled
        for (int i = 0; i < pixels.Length; i++)
        {
            if (pixels[i].a > 0.5f && pixels[i] != Color.white)
            {
                var historyStep = new HistoryStep();
                int x = i % width;
                int y = i / width;
                historyStep.Add(new ShortVector2(x, y));
                History2.AddStep(historyStep);
            }
        }
        History2.Shuffle();
    }

    public void SetImageInfo(ImageInfo ii)
    {
        ImageInfo = ii;
    }

    public void InitHistory()
    {
        if (History2 == null)
        {
            History2 = new History();
            History2.Init(FullFileName + ".his");
        }
    }

    public override string ToString()
    {
        return "SavedWorkData " + Id;
    }
}



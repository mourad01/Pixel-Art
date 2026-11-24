using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using VoxDLL;

public class GameController : UnitySingleton<GameController>
{
    public Action<Color, int> OnCountLeft;
    public Action<Color> OnColorComplete;
    public Action<Color> OnPaintedPixel;
    public Action OnComplete;

    List<VoxCubeItem> _voxCubeItems;
    List<bool> _voxCubeProgress;
    CashImage3D _vox;
    int colorsCounter;
    List<Color> _colors;

    public ColorSettings ColorSettings { get; set; }

    public bool IsLongClick { get; set; }

    public Camera VideoCamera { get; set; }

    public List<VoxCubeItem> VoxCubeItems
    {
        get
        {
            return _voxCubeItems;
        }
        set
        {
            if (_voxCubeItems == null)
            {
                _voxCubeItems = new List<VoxCubeItem>();
                _voxCubeItems = value;
            }
        }
    }

    public List<bool> VoxCubeProgress
    {
        get
        {
            return _voxCubeProgress;
        }
        private set
        {
            _voxCubeProgress = value;
        }
    }

    public event Action<List<Color>> VoxLoadComplete;

    public event Action<float> DepthCameraChange;

    public void Initilized(Color[] colorPallete)
    {
        ColorSettings = new ColorSettings(colorPallete);
        _colors = ColorSettings.GetColorPalleteList();
        OnVoxLoadComplete(_colors);

        if (DailyGame.IsDailyArt())
        { 
            DailyGame.UpdateGUI((int)_vox.CubesCount);
        }
    }

    public void SetCustomData(CashImage3D vox)
    {
        _vox = vox;
    }

    public void SetCurrentIndexCollorPallete(int index)
    {
        Loader3D.CurrentIndex = index;
        SetHighLightColorInCubeItems();
    }

    public void ChangeCamDepthPosition(float z)
    {
        SetTransparentColorByOffsetZ(z);
        OnDepthCameraChange(z);
    }

    public void SetCubeItemProgress(VoxCubeItem item)
    {
        int index = VoxCubeItems.IndexOf(item);
        _voxCubeProgress[index] = true;
    }
    public bool CheckColorLeft(int colorIndex, bool soundEnabled)
    {
        //vox color indexes start at 1, the palette list at 0
        if (OnCountLeft != null)
        {
            OnCountLeft(_colors[colorIndex - 1], _vox.ColorsCount[colorIndex]);
            return _vox.ColorsCount[colorIndex] > 0;
        }
        return false;
    }
    public void OnCubeColored(int colorIndex, bool soundEnabled = true)
    {
        if (colorIndex > 0)
        {
            _vox.ColorsCount[colorIndex] = _vox.ColorsCount[colorIndex] - 1;

            CheckColorLeft(colorIndex, soundEnabled);
            if (soundEnabled)
            {
                if (colorIndex < _colors.Count)
                    OnPaintedPixel(_colors[colorIndex]);
                else
                    OnPaintedPixel(_colors[colorIndex - 1]);
            }
            if (_vox.ColorsCount[colorIndex] <= 0)
            {
                OnColorComplete.SafeInvoke(_colors[colorIndex - 1]);
                if (soundEnabled)
                    AudioManager.Instance.PlayCompleteColor();
            }
            colorsCounter++;
            //every cube painted, the model is done
            if (colorsCounter == _vox.CubesCount)
            {
                UnitySingleton<ProgressManager>.instance.SetComplete();
                OnComplete.SafeInvoke();
                AudioManager.Instance.PlayVictory();
                UnitySingleton<ProgressManager>.instance.SaveWork(delegate (SavedWorkData3D swd)
                {
                    AnalyticsManager.Instance.ImageDone(swd.ImageInfo, colorsCounter, _colors.Count);
                });
            }
        }
    }

    public void UpdateColors()
    {
        SetProgressOnCubes();
    } 

    void SetHighLightColorInCubeItems()
    {
        if (AppData.BulbMode)
        {
            foreach (VoxCubeItem voxCubeItem in VoxCubeItems)
            {
                voxCubeItem.SetHighLightColorByFaceCube();
            }
        }
    }

    void SetTransparentColorByOffsetZ(float z)
    {
        if (VoxCubeItems != null)
        {
            foreach (VoxCubeItem voxCubeItem in VoxCubeItems)
            {
                voxCubeItem.SetTransparentColorByOffsetZ(z);
            }
        }
    }

    void SetProgressOnCubes()
    {
        var savedWorkData3D = UnitySingleton<ProgressManager>.instance.LoadProgressTest();
        List<bool> list = null;
        if (savedWorkData3D != null)
        {
            list = savedWorkData3D.Progress;
        }
        // replay the saved progress without sounds
        if (list != null && list.Count() > 0)
        {
            _voxCubeProgress = list;
            for (int i = 0; i < _voxCubeProgress.Count(); i++)
            {
                if (_voxCubeProgress[i])
                {
                    UnitySingleton<GameController>.Instance.OnCubeColored(_voxCubeItems[i].ColorIndex, false);
                    _voxCubeItems[i].SetProgressForCube();
                }
            }
        }
        else if (_voxCubeProgress == null)
        {
            _voxCubeProgress = new List<bool>();
            for (int j = 0; j < _voxCubeItems.Count(); j++)
            {
                _voxCubeProgress.Add(false);
            }
        }
        //color index 0 cubes have nothing to paint, mark them done
        foreach (VoxCubeItem voxCubeItem in _voxCubeItems)
        {
            if (voxCubeItem.ColorIndex == 0)
            {
                SetCubeItemProgress(voxCubeItem);
            }
        }
        UnitySingleton<ProgressManager>.instance.StartSaveProcess();
    }

    public void OnVoxLoadComplete(List<Color> list)
    {
        var voxLoadComplete = VoxLoadComplete;
        if (voxLoadComplete != null)
        {
            voxLoadComplete(list);
        }
    }

    void OnDepthCameraChange(float z)
    {
        var depthCameraChange = DepthCameraChange;
        if (depthCameraChange != null)
        {
            depthCameraChange(z);
        }
    }
}



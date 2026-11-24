


using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProgressManager : UnitySingleton<ProgressManager>
{
    ImageInfo _info;
    float TimeDelayForSave = 30f;
    CamShot camShot;
    bool isStartSaveWork;
    SavedWorkData3D m_savedWorkData;
    Action<SavedWorkData3D> saveWaiter;
    bool completed;

    public void Init(ImageInfo info, GameObject saveCamera, SavedWorkData3D savedWorkData)
    {
        if (info == null)
        {
            _info = new ImageInfo("test");
        }
        else
        {
            _info = info;
        }
        if (saveCamera != null)
        {
            camShot = saveCamera.GetComponent<CamShot>();
        }
        else
        {
            Debug.LogError("Save Camera not found!!!");
        }
        if (camShot == null)
        {
            Debug.LogError("CamShot not found!!!");
        }
        m_savedWorkData = savedWorkData;
        if (m_savedWorkData != null)
        {
            completed = m_savedWorkData.Completed;
        }
        camShot.ColorPalleteComplete += new Action<byte[]>(CamShot_ColorPalleteComplete);
    }

    public void StartSaveProcess()
    {
        base.StartCoroutine(Process());
    }

    public void StopSaveProcess()
    {
        base.StopCoroutine(Process());
    }

    public SavedWorkData3D LoadProgressTest()
    {
        return m_savedWorkData;
    }

    void CamShot_ColorPalleteComplete(byte[] obj)
    {
        var voxCubeProgress = UnitySingleton<GameController>.instance.VoxCubeProgress;
        if (voxCubeProgress != null)
        {
            m_savedWorkData = MainManager.Instance.SavedWorksList.Save3D(_info, obj, m_savedWorkData, completed, null, voxCubeProgress);
            isStartSaveWork = false;
        }
    }

    public void SetComplete()
    {
        completed = true;
    }

    public void SaveWork(Action<SavedWorkData3D> handler)
    {
        saveWaiter = handler;
        var voxCubeProgress = UnitySingleton<GameController>.instance.VoxCubeProgress;
        if (voxCubeProgress != null)
        {
            camShot.ColorPalleteComplete += new Action<byte[]>(WaitForScreenshot);
            camShot.TakeHiResShot();
        }
        handler.SafeInvoke(m_savedWorkData);
    }

    void WaitForScreenshot(byte[] bytes)
    {
        camShot.ColorPalleteComplete -= new Action<byte[]>(WaitForScreenshot);
        var voxCubeProgress = UnitySingleton<GameController>.instance.VoxCubeProgress;
        if (voxCubeProgress != null)
        {
            m_savedWorkData = MainManager.Instance.SavedWorksList.Save3D(_info, bytes, m_savedWorkData, completed, null, voxCubeProgress);
            isStartSaveWork = false;
        }
    }
    IEnumerator Process()
    {
        while (true)
        {
            yield return new WaitForSeconds(TimeDelayForSave);
            if (!isStartSaveWork)
            {
                isStartSaveWork = true;
                camShot.TakeHiResShot();
            }
            else
            {
                isStartSaveWork = false;
            }
        }
    }
}

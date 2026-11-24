using System;

public class SpecBoostersModel
{
    public Action<SpecBoostersModel> OnStateChanged;
    public Action<SpecBoostersModel> OnLassoModeChanged;
    public Action<SpecBoostersModel> OnBombModeChanged;

    bool m_lassoMode;
    bool m_bombMode;

    public bool LassoMode
    {
        get
        {
            return m_lassoMode;
        }
        set
        {
            m_lassoMode = value;
            OnLassoModeChanged.SafeInvoke(this);
        }
    }

    public bool BombMode
    {
        get
        {
            return m_bombMode;
        }
        set
        {
            m_bombMode = value;
            OnBombModeChanged.SafeInvoke(this);
        }
    }

    public int LassoCount
    {
        get
        {
            return AppData.LassoCount;
        }
    }

    public int BombCount
    {
        get
        {
            return AppData.BombCount;
        }
    }

    public void AddLasso()
    {
        AppData.LassoCount++;
        OnStateChanged.SafeInvoke(this);
    }

    public void AddBomb()
    {
        AppData.BombCount++;
        OnStateChanged.SafeInvoke(this);
    }

    public void SpendLasso()
    {

            AppData.LassoCount--;

        if (AppData.LassoCount <= 0)
        {
            LassoMode = false;
        }
        OnStateChanged.SafeInvoke(this);
    }

    public void SpendBomb()
    {
        /*if (!IAPWrapper.Instance.NoAds)
        {
            AppData.BombCount--;
        }*/
        if (AppData.BombCount <= 0)
        {
            BombMode = false;
        }
        OnStateChanged.SafeInvoke(this);
    }

    //lasso and bomb can't be on at the same time
    public void ChangeLassoMode(bool forceEnable = false)
    {
        LassoMode = (forceEnable || !LassoMode);
        if (LassoMode)
        {
            BombMode = false;
        }
    }

    public void ChangeBombMode(bool forceEnable = false)
    {
        BombMode = (forceEnable || !BombMode);
        if (BombMode)
        {
            LassoMode = false;
        }
    }
}



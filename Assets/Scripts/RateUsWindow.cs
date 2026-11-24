using System;
using UnityEngine;

public class RateUsWindow : MonoBehaviour
{
    Action m_closeHandler;

    public static bool NeedShow(RateUsReason reason)
    {
        return false;
    }

    public void Init(Action closeHandler)
    {
        m_closeHandler = closeHandler;
        gameObject.SetActive(true);
        AppData.AddNewRateUsView();
    }

    public void YesButtonClick()
    {
        RateUsTool.OpenRateUs();
        AppData.AppRated = true;
    }

    public void Close()
    {
        gameObject.SetActive(false);
        m_closeHandler.SafeInvoke();
        m_closeHandler = null;
    }
}



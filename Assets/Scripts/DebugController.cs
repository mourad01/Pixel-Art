using System;
using UnityEngine;

public class DebugController : MonoBehaviour
{
    public Action<bool> OnEnableDebugTexts;

    Action externalGui;

    [SerializeField] float m_updateInterval = 0.5f;

    float m_fps;
    float m_accum;
    int m_frames;
    float m_timeleft;

    public static DebugController Instance { get; private set; }

    public bool EnableDebugTexts { get; private set; }

    void Awake()
    {
        Instance = this;
    }

    void OnGUI()
    {
        if (externalGui != null)
        {
            externalGui.SafeInvoke();
        }
        else if (EnableDebugTexts && GUILayout.Button("fps: " + m_fps.ToString()))
        {
            //fps button cycles the frame cap, unlimited > 30 > 20
            int targetFrameRate = Application.targetFrameRate;
            Debug.Log(": " + targetFrameRate);
            if (targetFrameRate < 0)
            {
                Application.targetFrameRate = 30;
            }
            else if (targetFrameRate == 30)
            {
                Application.targetFrameRate = 20;
            }
            else
            {
                Application.targetFrameRate = -1;
            }
        }
    }

    void Update()
    {
        CalculateFps();
    }

    public void ShowExternalGui(Action _externalGui)
    {
        externalGui = _externalGui;
    }

    public void StopShowExternalGui()
    {
        externalGui = null;
    }

    public void ClickDebugTexts()
    {
        EnableDebugTexts = !EnableDebugTexts;
        OnEnableDebugTexts.SafeInvoke(EnableDebugTexts);
    }

    void CalculateFps()
    {
        //average fps over each update interval
        m_timeleft -= Time.deltaTime;
        m_accum += Time.timeScale / Time.deltaTime;
        m_frames++;
        if ((double)m_timeleft <= 0.0)
        {
            m_fps = m_accum / (float)m_frames;
            m_timeleft = m_updateInterval;
            m_accum = 0f;
            m_frames = 0;
        }
    }
}



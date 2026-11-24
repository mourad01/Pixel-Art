

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseWindow : MonoBehaviour
{
    public enum WindowState
    {
        Opened,
        Closed
    }

    public enum WindowOpenStyle
    {
        None,
        FromRight,
        FromBottom,
        FromLeft,
        FromTop
    }

    public Action OnStartOpen;
    public Action OnOpen;
    public Action OnStartClose;
    public Action OnClose;

    [SerializeField] WindowOpenStyle m_openStyle = WindowOpenStyle.FromBottom;

    Canvas m_canvas;
    RectTransform m_rectTransform;
    Vector2 m_openedPosition = Vector2.zero;
    Vector2 m_closedPosition = Vector2.zero;
    float m_moveTime = 0.1f;
    WindowState m_currentWindowState = WindowState.Closed;

    public virtual bool EnableBanner
    {
        get
        {
            return false;
        }
    }

    RectTransform RectTransform
    {
        get
        {
            if (m_rectTransform == null)
            {
                m_rectTransform = (RectTransform)transform;
            }
            return m_rectTransform;
        }
    }

    protected abstract string WindowName
    {
        get;
    }

    public virtual void InitCanvas(Canvas canvas, bool setClosePosition = true)
    {
        m_canvas = canvas;
        //closed position sits 50 px outside the canvas edge
        switch (m_openStyle)
        {
            case WindowOpenStyle.None:
                m_closedPosition = Vector2.zero;
                break;
            case WindowOpenStyle.FromRight:
                m_closedPosition = new Vector2(((RectTransform)canvas.transform).rect.width + 50f, 0f);
                break;
            case WindowOpenStyle.FromLeft:
                m_closedPosition = new Vector2(0f - ((RectTransform)canvas.transform).rect.width - 50f, 0f);
                break;
            case WindowOpenStyle.FromBottom:
                m_closedPosition = new Vector2(0f, 0f - ((RectTransform)canvas.transform).rect.height - 50f);
                break;
            case WindowOpenStyle.FromTop:
                m_closedPosition = new Vector2(0f, ((RectTransform)canvas.transform).rect.height - 50f);// 0f - ((RectTransform)canvas.transform).rect.height - 50f);
                break;
        }
        if (setClosePosition)
        {
            RectTransform.anchoredPosition = m_closedPosition;
        }
    }

    Vector2 GetClosedPosition(WindowOpenStyle style)
    {
        switch (style)
        {
            case WindowOpenStyle.None:
                return Vector2.zero;
            case WindowOpenStyle.FromRight:
                return new Vector2(((RectTransform)m_canvas.transform).rect.width + 50f, 0f);
            case WindowOpenStyle.FromLeft:
                return new Vector2(0f - ((RectTransform)m_canvas.transform).rect.width - 50f, 0f);
            case WindowOpenStyle.FromBottom:
                return new Vector2(0f, 0f - ((RectTransform)m_canvas.transform).rect.height - 50f);
            case WindowOpenStyle.FromTop:
                return new Vector2(0f, ((RectTransform)m_canvas.transform).rect.height - 50f);// 0f - ((RectTransform)this.m_canvas.transform).rect.height - 50f);
            default:
                return Vector2.zero;
        }
    }

    public virtual void Open()
    {
        Open(m_openStyle);
    }

    public void Open(WindowOpenStyle style)
    {
        SendActiveScreenEvent();
        base.StopAllCoroutines();
        OnStartOpen.SafeInvoke();
        gameObject.SetActive(true);
        switch (style)
        {
            case WindowOpenStyle.FromRight:
                base.StartCoroutine(OpenFromSideCoroutine(GetClosedPosition(style)));
                break;
            case WindowOpenStyle.FromLeft:
                base.StartCoroutine(OpenFromSideCoroutine(GetClosedPosition(style)));
                break;
            case WindowOpenStyle.FromBottom:
                base.StartCoroutine(OpenFromBottomCoroutine());
                break;
            case WindowOpenStyle.FromTop:
                base.StartCoroutine(OpenFromTopCoroutine());
                break;
            case WindowOpenStyle.None:
                RectTransform.anchoredPosition = m_openedPosition;
                OnOpen.SafeInvoke();
                break;
        }
    }

    public void DefferedOpen(float time = 0.1f)
    {
        gameObject.SetActive(true);
        RectTransform.anchoredPosition = m_closedPosition;
        base.StartCoroutine(DefferedOpenCoroutine(m_openStyle, time));
    }

    public void DefferedOpen(WindowOpenStyle style, float time = 0.1f)
    {
        gameObject.SetActive(true);
        RectTransform.anchoredPosition = GetClosedPosition(style);
        base.StartCoroutine(DefferedOpenCoroutine(style, time));
    }

    IEnumerator DefferedOpenCoroutine(WindowOpenStyle style, float time)
    {
        yield return new WaitForSeconds(time);
        Open(style);
    }

    public void FastOpen()
    {
        SendActiveScreenEvent();
        gameObject.SetActive(true);
        RectTransform.anchoredPosition = m_openedPosition;
    }

    public virtual bool Close()
    {
        base.StopAllCoroutines();
        OnStartClose.SafeInvoke();
        switch (m_openStyle)
        {
            case WindowOpenStyle.FromRight:
                base.StartCoroutine(CloseToSideCoroutine(GetClosedPosition(m_openStyle)));
                break;
            case WindowOpenStyle.FromLeft:
                base.StartCoroutine(CloseToSideCoroutine(GetClosedPosition(m_openStyle)));
                break;
            case WindowOpenStyle.FromBottom:
                base.StartCoroutine(CloseToBottomCoroutine());
                break;
            case WindowOpenStyle.FromTop:
                base.StartCoroutine(CloseToTopCoroutine());
                break;
            case WindowOpenStyle.None:
                gameObject.SetActive(false);
                OnClose.SafeInvoke();
                break;
        }
        return true;
    }

    public virtual bool Close(WindowOpenStyle style)
    {
        base.StopAllCoroutines();
        OnStartClose.SafeInvoke();
        switch (style)
        {
            case WindowOpenStyle.FromRight:
                base.StartCoroutine(CloseToSideCoroutine(GetClosedPosition(style)));
                break;
            case WindowOpenStyle.FromLeft:
                base.StartCoroutine(CloseToSideCoroutine(GetClosedPosition(style)));
                break;
            case WindowOpenStyle.FromBottom:
                base.StartCoroutine(CloseToBottomCoroutine());
                break;
            case WindowOpenStyle.FromTop:
                base.StartCoroutine(CloseToTopCoroutine());
                break;
            case WindowOpenStyle.None:
                gameObject.SetActive(false);
                OnClose.SafeInvoke();
                break;
        }
        return true;
    }

    public void FastClose()
    {
        gameObject.SetActive(false);
        RectTransform.anchoredPosition = m_closedPosition;
    } 

    IEnumerator OpenFromSideCoroutine(Vector2 closedPosition)
    { 
        bool sign = RectTransform.anchoredPosition.x > m_openedPosition.x;
        float speed = (m_openedPosition.x - closedPosition.x) / m_moveTime;
        yield return null;

        while (true)
        {
            // cap delta time so a slow frame does not jump too far
            var delta = speed * Mathf.Min(0.05f, Time.deltaTime);
            var newSign = (RectTransform.anchoredPosition.x + delta > m_openedPosition.x);
            //stop once the window passes its opened position
            if (newSign != sign)
            {
                RectTransform.anchoredPosition = m_openedPosition;
                yield return null;
                OnOpen.SafeInvoke();
                yield break;
            } 
            RectTransform.anchoredPosition += new Vector2(delta, 0f);
            yield return null;
        }
    }
    IEnumerator CloseToBottomCoroutine()
    { 
        bool sign = RectTransform.anchoredPosition.y > m_closedPosition.y;
        float speed = (m_closedPosition.y - m_openedPosition.y) / m_moveTime;
        yield return null;

        while (true)
        {
            var delta = speed * Mathf.Min(0.05f, Time.deltaTime);
            var newSign = (RectTransform.anchoredPosition.y + delta > m_openedPosition.y);
            if (newSign != sign)
            {
                RectTransform.anchoredPosition = m_openedPosition;
                yield return null;
                gameObject.SetActive(false);
                OnOpen.SafeInvoke();
                yield break;
            }
            RectTransform.anchoredPosition += new Vector2(0, delta);
            yield return null;
        }
    }
    IEnumerator OpenFromBottomCoroutine()
    { 
        var sign = (RectTransform.anchoredPosition.y > m_openedPosition.y);
        var speed = (m_openedPosition.y - m_closedPosition.y) / m_moveTime;

        yield return null;

        while (true)
        {
            var delta = speed * Mathf.Min(0.05f, Time.deltaTime); 
            var newSign = (RectTransform.anchoredPosition.y + delta > m_openedPosition.y);
            if (newSign != sign)
            {
                RectTransform.anchoredPosition = m_openedPosition;
                yield return null;
                OnOpen.SafeInvoke();

                yield break;
            } 
            RectTransform.anchoredPosition += new Vector2(0f, delta);

            yield return null;
        }
    }
    IEnumerator OpenFromTopCoroutine()
    { 
        var sign = (RectTransform.anchoredPosition.y < m_openedPosition.y);
        var speed = (m_openedPosition.y - m_closedPosition.y) / m_moveTime;
        yield return null;

        while (true)
        {
            var delta = speed * Mathf.Min(0.05f, Time.deltaTime); 
            var newSign = (RectTransform.anchoredPosition.y + delta < m_openedPosition.y);
            if (newSign != sign)
            {
                RectTransform.anchoredPosition = m_openedPosition;
                yield return null;

                OnOpen.SafeInvoke();
                yield break;
            }

            RectTransform.anchoredPosition += new Vector2(0f, delta);
            yield return null;
        }
    }
    IEnumerator CloseToSideCoroutine(Vector2 closedPosition)
    { 
        var sign = (RectTransform.anchoredPosition.x > closedPosition.x);
        var speed = (closedPosition.x - m_openedPosition.x) / m_moveTime;
        yield return null;

        while (true)
        {
            var delta = speed * Mathf.Min(0.05f, Time.deltaTime); 
            var newSign = (RectTransform.anchoredPosition.x + delta > closedPosition.x);
            if (newSign != sign)
            {
                RectTransform.anchoredPosition = closedPosition;
                gameObject.SetActive(false);
                OnClose.SafeInvoke();
                yield break;
            } 
            RectTransform.anchoredPosition += new Vector2(delta, 0f);
            yield return null;
        }
    } 
    IEnumerator CloseToTopCoroutine()
    { 
        var sign = (RectTransform.anchoredPosition.y > m_closedPosition.y);
        var speed = (m_closedPosition.y - m_openedPosition.y) / m_moveTime;
        yield return null;

        while (true)
        {
            var delta = speed * Mathf.Min(0.05f, Time.deltaTime); 
            var newSign = (RectTransform.anchoredPosition.y + delta > m_closedPosition.y);
            if (newSign != sign)
            {
                RectTransform.anchoredPosition = m_closedPosition;
                gameObject.SetActive(false);
                OnClose.SafeInvoke();
                yield break; 
            } 
            // no yield here so it closes in the same frame
            RectTransform.anchoredPosition += new Vector2(0f, delta);
        }
    }

    void OnEnable()
    {
        switch (m_currentWindowState)
        {
            case WindowState.Opened:
                RectTransform.anchoredPosition = m_openedPosition;
                break;
            case WindowState.Closed:
                RectTransform.anchoredPosition = m_closedPosition;
                break;
        }
    }

    public virtual void SendActiveScreenEvent()
    {
        /*if (!string.IsNullOrEmpty(this.WindowName) && INPluginWrapper.Instance != null)
        {
            //INPluginWrapper.Instance.SetActiveScreen(this.WindowName);
        }*/
    }
}



using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Popup : MonoBehaviour
{
    Action m_firstButtonClick;
    Action m_secondButtonClick;

    [SerializeField] RectTransform m_messagePanel;
    [SerializeField] Text m_message;
    [SerializeField] Text m_button1Text;
    [SerializeField] Text m_button2Text;
    [SerializeField] GameObject m_blockPlane;
    [SerializeField] Vector2 m_messageOpenedPos = Vector2.zero;
    [SerializeField] Vector2 m_messageClosedPos = Vector2.zero;
    [SerializeField] float m_showTime = 4f;

    public void Init()
    {
        m_messagePanel.anchoredPosition = m_messageClosedPos;
        m_messagePanel.gameObject.SetActive(false);
    }

    public void Show(string text, int time = 4)
    {
        if (gameObject != null && m_message != null && m_messagePanel != null && m_messagePanel.gameObject != null)
        {
            m_showTime = time;
            m_message.text = text;
            m_messagePanel.gameObject.SetActive(true);
            base.StartCoroutine(ShowMessageCoroutine());
        }
    }

    public void Show(string text, Action firstButtonClick, Action secondButtonClick)
    {
        if (gameObject != null)
        {
            m_message.text = text;
            m_firstButtonClick = firstButtonClick;
            m_secondButtonClick = secondButtonClick;
            m_messagePanel.gameObject.SetActive(true);
            base.StartCoroutine(ShowMessageCoroutine());
        }
    }

    public void FirstButtonClick()
    {
        m_firstButtonClick.SafeInvoke();
        Close();
    }

    public void SecondButtonClick()
    {
        m_secondButtonClick.SafeInvoke();
        Close();
    }

    public void Close()
    {
        base.StartCoroutine(CloseCoroutine());
    }
    IEnumerator ShowMessageCoroutine()
    {
        yield return null;
        if (m_blockPlane != null)
        {
            m_blockPlane.SetActive(true);
        }
        m_messagePanel.anchoredPosition = m_messageClosedPos;
        var time = 0.3f;
        var speed = (m_messageOpenedPos - m_messageClosedPos) / time;

        while (true)
        {
            var deltaTime = Time.deltaTime;
            if (time <= deltaTime)
            {
                m_messagePanel.anchoredPosition = m_messageOpenedPos;
                if (m_showTime > 0f)
                {
                    yield return new WaitForSeconds(m_showTime);
                    Close(); 
                }
                yield break;
            }
            time -= deltaTime; 
            m_messagePanel.anchoredPosition += speed * deltaTime;
            yield return null;
        }
    }
    IEnumerator CloseCoroutine()
    {
        if (m_blockPlane != null)
        {
            m_blockPlane.SetActive(false);
        }
        var time = 0.3f;
        var speed = (m_messageClosedPos - m_messageOpenedPos) / time;

        while(true)
        {
            var deltaTime = Time.deltaTime;
            if (time <= deltaTime)
            {
                m_messagePanel.anchoredPosition = m_messageClosedPos;
                m_messagePanel.gameObject.SetActive(false);
                yield break;
            }
            time -= deltaTime; 
            m_messagePanel.anchoredPosition += speed * deltaTime;
            yield return null;
        }
    }
}

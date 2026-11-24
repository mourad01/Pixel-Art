

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Switcher : MonoBehaviour
{
    [SerializeField] float m_switchDelta = 100f;
    [SerializeField] RectTransform m_pointer;
    [SerializeField] Image m_background;
    [SerializeField] Color m_onColor = Color.green;
    [SerializeField] Color m_offColor = Color.gray;

    public bool Value { get; private set; }

    public void UpdateState(bool value, bool fast)
    {
        if (Value == value && !fast)
        {
            return;
        }
        Value = value;
        base.StopAllCoroutines();
        if (fast)
        {
            m_pointer.anchoredPosition = new Vector2((!Value) ? (0f - m_switchDelta) : m_switchDelta, 0f);
            m_background.color = ((!Value) ? m_offColor : m_onColor);
        }
        else
        {
            base.StartCoroutine(UpdateStateCoroutine());
        }
    }
    IEnumerator UpdateStateCoroutine()
    {
        var time = 0.1f;
        var speed = ((!Value) ? (0f - m_switchDelta) : m_switchDelta) * 2f / time; 
        var sign = (m_pointer.anchoredPosition.x > ((!Value) ? (0f - m_switchDelta) : m_switchDelta)); 
        var zeroSign = (m_pointer.anchoredPosition.x > 0f);
        yield return null;

        while (true)
        {
            var deltaTime = Mathf.Min(Time.deltaTime, 0.05f); 
            // swap the background color once the pointer crosses the middle
            var newZeroSign = (m_pointer.anchoredPosition.x > 0f);
            if (zeroSign != newZeroSign)
            {
                m_background.color = ((!Value) ? m_offColor : m_onColor);
            } 
            var newSign = (m_pointer.anchoredPosition.x + speed * deltaTime > ((!Value) ? (0f - m_switchDelta) : m_switchDelta));
            if (newSign != sign)
            {
                m_pointer.anchoredPosition = new Vector2((!Value) ? (0f - m_switchDelta) : m_switchDelta, 0f);
                yield break;
            } 
            m_pointer.anchoredPosition += new Vector2(speed * deltaTime, 0f);
            yield return null;
        }
    }
}

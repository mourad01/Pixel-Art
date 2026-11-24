

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PositScrollRect : ScrollRect
{
    public float Speed = 5000f;
    public float DistForScale = 100f;
    public float Scale = 1f;
    public bool DynamicSize;
    public Action OnDragStart;
    public Action<int> OnSelectIndex;
    public Action<int> OnDragSelectIndex;
    public Action<int, float, float> OnScaleElement;
    public Action<int> OnSlideToIndex;
    public Action OnSwipe;
    public Action OnNavigationButtonClick;

    RectTransform m_rectTransform;
    List<RectTransform> m_list;
    int m_currentElement;
    HorizontalOrVerticalLayoutGroup m_contentGroup;
    bool m_drag;
    int m_pointerId = -1;
    Canvas m_canvas;
    float m_screenWidth;
    float m_rectWidth;
    float m_rectHeight;
    float m_startDragPos;

    public bool Moving
    {
        get
        {
            return m_drag;
        }
    }

    public bool Inverted { get; set; }

    public bool NextAvailable
    {
        get
        {
            return m_currentElement < m_list.Count - 1;
        }
    }

    public bool PrevAvailable
    {
        get
        {
            return m_currentElement > 0;
        }
    }

    public void Reinit(float screenWidth, bool withoutPosCorrection = false)
    {
        m_list = new List<RectTransform>();
        m_drag = false;
        int childCount = content.childCount;
        for (int i = 0; i < childCount; i++)
        {
            var rectTransform = content.GetChild(i) as RectTransform;
            if (rectTransform.gameObject.activeSelf)
            {
                m_list.Add(rectTransform);
            }
        }
        m_contentGroup = ((Component)content).GetComponent<HorizontalOrVerticalLayoutGroup>();
        m_rectTransform = (transform as RectTransform);
        m_canvas = FindParentCanvas();
        if (horizontal)
        {
            m_screenWidth = (m_canvas.transform as RectTransform).rect.width;
        }
        else if (vertical)
        {
            m_screenWidth = (m_canvas.transform as RectTransform).rect.height;
        }
        m_rectWidth = m_rectTransform.rect.width;
        m_rectHeight = m_rectTransform.rect.height;
        SetContentSize();
        if (!withoutPosCorrection)
        {
            var content = base.content;
            Vector2 anchoredPosition = content.anchoredPosition;
            Vector2 anchoredPosition2 = base.content.anchoredPosition;
            content.anchoredPosition = anchoredPosition + new Vector2(0f - anchoredPosition2.x, 0f);
        }
        UpdateImageIndex();
        UpdateScale(true);
    }

    Canvas FindParentCanvas()
    {
        Transform rectTransform = m_rectTransform;
        while (true)
        {
            rectTransform = rectTransform.parent;
            if (rectTransform != null)
            {
                var canvas = ((Component)rectTransform).GetComponent<Canvas>();
                if (!(canvas != null))
                {
                    continue;
                }
                m_canvas = canvas;
            }
            break;
        }
        return m_canvas;
    }

    public void SetContentSize()
    {
        if (horizontal)
        {
            float totalWidth = 0f;
            foreach (RectTransform rectTransform in m_list)
            {
                totalWidth += rectTransform.rect.width;
            }
            totalWidth += (float)(m_list.Count - 1) * m_contentGroup.spacing;
            var content = base.content;
            float x = totalWidth;
            Vector2 sizeDelta = base.content.sizeDelta;
            content.sizeDelta = new Vector2(x, sizeDelta.y);
        }
        else if (vertical)
        {
            float value2 = 0f;
            foreach (RectTransform item2 in m_list)
            {
                value2 += item2.rect.height;
            }
            value2 += (float)(m_list.Count - 1) * m_contentGroup.spacing;
            var content2 = content;
            Vector2 sizeDelta2 = content.sizeDelta;
            content2.sizeDelta = new Vector2(sizeDelta2.x, value2);
        }
        if (horizontal)
        {
            enabled = true;
        }
        else if (vertical)
        {
            enabled = true;
        }
    }

    public void PrevButtonClick(bool force = false)
    {
        if (m_drag && !force)
        {
            return;
        }
        if (horizontal)
        {
            if (m_currentElement > 0)
            {
                base.StartCoroutine(MoveCoroutine(m_list[m_currentElement - 1].rect.width + m_contentGroup.spacing));
                OnDragStart.SafeInvoke();
            }
        }
        else if (vertical && m_currentElement > 0)
        {
            base.StartCoroutine(MoveCoroutine(0f - (m_list[m_currentElement - 1].rect.height + m_contentGroup.spacing)));
            OnDragStart.SafeInvoke();
        }
        OnNavigationButtonClick.SafeInvoke();
    }

    public void NextButtonClick(bool force = false)
    {
        if (m_drag && !force)
        {
            return;
        }
        if (horizontal)
        {
            Vector2 anchoredPosition = content.anchoredPosition;
            if (anchoredPosition.x - m_rectTransform.rect.width - 5f > (0f - m_list[m_currentElement].rect.width) * (float)m_list.Count - m_contentGroup.spacing * (float)(m_list.Count - 1))
            {
                base.StartCoroutine(MoveCoroutine(0f - (m_list[m_currentElement].rect.width + m_contentGroup.spacing)));
                OnDragStart.SafeInvoke();
            }
        }
        else if (vertical)
        {
            Vector2 anchoredPosition2 = content.anchoredPosition;
            if (0f - anchoredPosition2.y - m_rectHeight - 5f > (0f - m_list[m_currentElement].rect.height) * (float)m_list.Count - m_contentGroup.spacing * (float)(m_list.Count - 1))
            {
                base.StartCoroutine(MoveCoroutine(m_list[m_currentElement].rect.height + m_contentGroup.spacing));
                OnDragStart.SafeInvoke();
            }
        }
        OnNavigationButtonClick.SafeInvoke();
    }

    public override void OnBeginDrag(PointerEventData data)
    {
        if (!m_drag)
        {
            if (horizontal)
            {
                Vector3 localPosition = base.content.localPosition;
                m_startDragPos = localPosition.x;
                Vector2 move = data.delta / (float)Camera.main.pixelWidth * m_screenWidth;
                if (Inverted)
                {
                    move.x *= -1f;
                }
                var content = base.content;
                content.localPosition += new Vector3(move.x, 0f, 0f);
                m_drag = true;
            }
            else if (vertical)
            {
                Vector3 pos = content.localPosition;
                m_startDragPos = pos.y;
                Vector2 scrollOffset = data.delta / (float)Camera.main.pixelWidth * m_screenWidth;
                if (Inverted)
                {
                    scrollOffset.y *= -1f;
                }
                var content2 = content;
                content2.localPosition += new Vector3(0f, scrollOffset.y, 0f);
                m_drag = true;
            }
            m_pointerId = data.pointerId;
            OnDragStart.SafeInvoke();
        }
    }

    public override void OnDrag(PointerEventData data)
    {
        if (data.pointerId == m_pointerId)
        {
            if (horizontal)
            {
                Vector2 move = data.delta / (float)Camera.main.pixelWidth * m_screenWidth;
                if (Inverted)
                {
                    move.x *= -1f;
                }
                var content = base.content;
                content.localPosition += new Vector3(move.x, 0f, 0f);
                UpdateScale(false);
            }
            else if (vertical)
            {
                Vector2 scrollOffset = data.delta / (float)Camera.main.pixelWidth * m_screenWidth;
                if (Inverted)
                {
                    scrollOffset.y *= -1f;
                }
                var content2 = content;
                content2.localPosition += new Vector3(0f, scrollOffset.y, 0f);
                UpdateScale(false);
            }
        }
    }

    public override void OnEndDrag(PointerEventData data)
    {
        if (data.pointerId == m_pointerId)
        {
            if (horizontal)
            {
                if (DynamicSize)
                {
                    MoveToIndex(m_currentElement);
                    return;
                }
                Vector2 move = data.delta / (float)Camera.main.pixelWidth * m_screenWidth;
                if (Inverted)
                {
                    move.x *= -1f;
                }
                var content = base.content;
                content.localPosition += new Vector3(move.x, 0f, 0f);
                Vector3 localPosition = base.content.localPosition;
                float dragDistance = localPosition.x - m_startDragPos;
                if (dragDistance > 0f)
                {
                    if (dragDistance < 100f)
                    {
                        base.StartCoroutine(MoveCoroutine(0f - dragDistance));
                    }
                    else
                    {
                        Vector2 anchoredPosition = base.content.anchoredPosition;
                        if (anchoredPosition.x > 0f)
                        {
                            Vector2 anchoredPosition2 = base.content.anchoredPosition;
                            base.StartCoroutine(MoveCoroutine(0f - anchoredPosition2.x));
                        }
                        else
                        {
                            int b = Mathf.RoundToInt(dragDistance / (m_list[m_currentElement].rect.width + m_contentGroup.spacing));
                            base.StartCoroutine(MoveCoroutine((m_list[0].rect.width + m_contentGroup.spacing) * (float)Mathf.Max(1, b) - dragDistance));
                        }
                    }
                }
                else if (dragDistance > -100f)
                {
                    base.StartCoroutine(MoveCoroutine(0f - dragDistance));
                }
                else
                {
                    Vector2 anchoredPosition3 = base.content.anchoredPosition;
                    if (anchoredPosition3.x - m_rectTransform.rect.width < (0f - m_list[0].rect.width) * (float)m_list.Count - m_contentGroup.spacing * (float)(m_list.Count - 1))
                    {
                        float value2 = (0f - (m_list[0].rect.width + m_contentGroup.spacing)) * (float)(m_list.Count - 1);
                        Vector2 anchoredPosition4 = base.content.anchoredPosition;
                        base.StartCoroutine(MoveCoroutine(value2 - anchoredPosition4.x));
                    }
                    else
                    {
                        int b2 = Mathf.RoundToInt((0f - dragDistance) / (m_list[m_currentElement].rect.width + m_contentGroup.spacing));
                        base.StartCoroutine(MoveCoroutine((0f - (m_list[m_currentElement].rect.width + m_contentGroup.spacing)) * (float)Mathf.Max(1, b2) - dragDistance));
                    }
                }
            }
            else if (vertical)
            {
                Vector2 scrollOffset = data.delta / (float)Camera.main.pixelWidth * m_screenWidth;
                var content2 = content;
                content2.localPosition += new Vector3(0f, scrollOffset.y, 0f);
                Vector3 pos = content.localPosition;
                float value3 = pos.y - m_startDragPos;
                if (value3 > 0f)
                {
                    if (value3 < 100f)
                    {
                        base.StartCoroutine(MoveCoroutine(0f - value3));
                    }
                    else
                    {
                        Vector2 anchoredPosition5 = content.anchoredPosition;
                        if (anchoredPosition5.y + m_rectHeight > m_list[m_currentElement].rect.height * (float)m_list.Count + m_contentGroup.spacing * (float)(m_list.Count - 1))
                        {
                            float value4 = content.rect.height - m_rectHeight;
                            Vector2 anchoredPosition6 = content.anchoredPosition;
                            base.StartCoroutine(MoveCoroutine(value4 - anchoredPosition6.y));
                        }
                        else
                        {
                            base.StartCoroutine(MoveCoroutine(m_list[m_currentElement].rect.height + m_contentGroup.spacing - value3));
                        }
                    }
                }
                else if (value3 > -100f)
                {
                    base.StartCoroutine(MoveCoroutine(0f - value3));
                }
                else
                {
                    Vector2 anchoredPosition7 = content.anchoredPosition;
                    if (anchoredPosition7.y - m_rectHeight < 0f)
                    {
                        Vector2 anchoredPosition8 = content.anchoredPosition;
                        base.StartCoroutine(MoveCoroutine(0f - anchoredPosition8.y));
                    }
                    else
                    {
                        base.StartCoroutine(MoveCoroutine(0f - (m_list[m_currentElement].rect.height + m_contentGroup.spacing) - value3));
                    }
                }
            }
            OnSwipe.SafeInvoke();
        }
    }

    public void SetIndex(int index)
    {
        if (m_list.Count > index)
        {
            if (DynamicSize)
            {
                MoveToIndex(index);
            }
            else
            {
                if (horizontal)
                {
                    var content = base.content;
                    float x = (0f - (m_list[0].rect.width + m_contentGroup.spacing)) * (float)index;
                    Vector2 anchoredPosition = base.content.anchoredPosition;
                    content.anchoredPosition = new Vector2(x, anchoredPosition.y);
                }
                else if (vertical)
                {
                    var content2 = content;
                    Vector2 anchoredPosition2 = content.anchoredPosition;
                    content2.anchoredPosition = new Vector2(anchoredPosition2.x, (m_list[0].rect.height + m_contentGroup.spacing) * (float)index);
                }
                UpdateImageIndex();
                UpdateScale(true);
            }
        }
    }

    public void MoveToIndex(int index)
    {
        if (m_list.Count > index)
        {
            if (horizontal)
            {
                base.StartCoroutine(MoveToIndexCoroutine(index));
            }
            else if (!vertical)
            {
            }
            UpdateScale(true);
        }
        return;
    }

    void UpdateImageIndex()
    {
        if (m_list.Count == 0)
        {
            m_currentElement = -1;
        }
        else if (horizontal)
        {
            if (!DynamicSize)
            {
                Vector2 anchoredPosition = content.anchoredPosition;
                m_currentElement = Mathf.RoundToInt((0f - anchoredPosition.x) / (m_list[0].rect.width + m_contentGroup.spacing));
            }
            else
            {
                float pos = (0f - m_contentGroup.spacing) / 2f + (float)m_contentGroup.padding.left;
                for (int i = 0; i < m_list.Count; i++)
                {
                    pos += m_list[i].rect.width + m_contentGroup.spacing;
                    Vector2 anchoredPosition2 = content.anchoredPosition;
                    if (anchoredPosition2.x > 0f - pos)
                    {
                        m_currentElement = i;
                        break;
                    }
                }
                m_currentElement = Mathf.Min(m_currentElement, m_list.Count - 1);
            }
        }
        else if (vertical)
        {
            Vector2 anchoredPosition3 = content.anchoredPosition;
            m_currentElement = Mathf.RoundToInt(anchoredPosition3.y / (m_list[0].rect.height + m_contentGroup.spacing));
        }
        if (!m_drag)
        {
            OnSelectIndex.SafeInvoke(m_currentElement);
        }
        else
        {
            OnDragSelectIndex.SafeInvoke(m_currentElement);
        }
    }

    void UpdateScale(bool all)
    {
        if (m_list != null)
        {
            if (Scale > 1f)
            {
                float pos = 0f;
                for (int i = 0; i < m_list.Count; i++)
                {
                    float value2 = 0f;
                    if (horizontal)
                    {
                        if (i > 0)
                        {
                            pos += m_list[i - 1].rect.width + m_contentGroup.spacing;
                        }
                        Vector2 anchoredPosition = content.anchoredPosition;
                        value2 = Mathf.Abs(anchoredPosition.x + pos);
                    }
                    else if (vertical)
                    {
                        pos = m_list[i].rect.height * (float)i + m_contentGroup.spacing * (float)i;
                        Vector2 anchoredPosition2 = content.anchoredPosition;
                        value2 = Mathf.Abs(anchoredPosition2.y - pos);
                    }
                    if (all || value2 < DistForScale + m_list[i].rect.width + m_contentGroup.spacing)
                    {
                        float par = (!(value2 < DistForScale)) ? 1f : Mathf.Lerp(Scale, 1f, value2 / DistForScale);
                        OnScaleElement.SafeInvoke(i, par, Scale);
                    }
                }
            }
            UpdateImageIndex();
        }
    }

    public void PublicSetDirty()
    {
        base.SetDirty();
    }

    protected override void OnEnable()
    {
        UpdateScale(true);
        base.OnEnable();
    }
    IEnumerator MoveCoroutine(float delta)
    {
        m_drag = true;
        var deltaMoreThan0 = delta >= 0f;

        while (true)
        {
            var nextMove = Time.deltaTime * Speed;
            nextMove = Mathf.Min(nextMove, Mathf.Abs(delta));
            nextMove *= (float)((!(delta < 0f)) ? 1 : (-1));
            if (horizontal)
            {
                content.localPosition += new Vector3(nextMove, 0f, 0f);
            }
            else if (vertical)
            {
                content.localPosition += new Vector3(0f, nextMove, 0f);
            }
            delta -= nextMove;
            if (deltaMoreThan0)
            {
                if (!(delta <= 0f))
                {
                    UpdateScale(false);
                    yield return null;
                    continue;
                }
            }
            else if (!(delta >= 0f))
            {
                UpdateScale(false);
                yield return null;
                continue;
            }

            yield return null;

            m_drag = false;
            UpdateImageIndex();
            SetIndex(m_currentElement);
            UpdateScale(false);

            yield break;
        }
    }

    IEnumerator MoveToIndexCoroutine(int index)
    {
        m_drag = true;
        while(true)
        {
            var resPos = 0f;
            for (int i = 0; i < index; i++)
            {
                resPos -= m_list[i].rect.width + m_contentGroup.spacing;
            }
            var delta = resPos - content.anchoredPosition.x;
            var deltaTime = Mathf.Min(0.05f, Time.deltaTime);
            var newDelta = Mathf.Min(Mathf.Abs(delta), Mathf.Abs(Speed * deltaTime));
            if (delta < 0f)
            {
                newDelta = 0f - newDelta;
            }
            content.localPosition += new Vector3(newDelta, 0f, 0f);
            UpdateScale(false);
            if (!(Mathf.Abs(newDelta) < 1f))
            {
                yield return null;
                continue;
            }
            m_drag = false;

            yield break;
        }
    }
}

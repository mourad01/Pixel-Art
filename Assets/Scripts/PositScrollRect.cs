

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
        //only active children are pages
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
        //canvas size along the scroll axis, used to convert pointer pixels to canvas units
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
        // snap back to the first page
        if (!withoutPosCorrection)
        {
            var content = base.content;
            content.anchoredPosition = content.anchoredPosition + new Vector2(0f - base.content.anchoredPosition.x, 0f);
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
        //content size = sum of pages + spacing between them
        if (horizontal)
        {
            float totalWidth = 0f;
            foreach (RectTransform rectTransform in m_list)
            {
                totalWidth += rectTransform.rect.width;
            }
            totalWidth += (float)(m_list.Count - 1) * m_contentGroup.spacing;
            base.content.sizeDelta = new Vector2(totalWidth, base.content.sizeDelta.y);
        }
        else if (vertical)
        {
            float totalHeight = 0f;
            foreach (RectTransform item in m_list)
            {
                totalHeight += item.rect.height;
            }
            totalHeight += (float)(m_list.Count - 1) * m_contentGroup.spacing;
            content.sizeDelta = new Vector2(content.sizeDelta.x, totalHeight);
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
        //only move if we are not already on the last page
        if (horizontal)
        {
            if (content.anchoredPosition.x - m_rectTransform.rect.width - 5f > (0f - m_list[m_currentElement].rect.width) * (float)m_list.Count - m_contentGroup.spacing * (float)(m_list.Count - 1))
            {
                base.StartCoroutine(MoveCoroutine(0f - (m_list[m_currentElement].rect.width + m_contentGroup.spacing)));
                OnDragStart.SafeInvoke();
            }
        }
        else if (vertical)
        {
            if (0f - content.anchoredPosition.y - m_rectHeight - 5f > (0f - m_list[m_currentElement].rect.height) * (float)m_list.Count - m_contentGroup.spacing * (float)(m_list.Count - 1))
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
                m_startDragPos = base.content.localPosition.x;
                Vector2 move = data.delta / (float)Camera.main.pixelWidth * m_screenWidth;
                if (Inverted)
                {
                    move.x *= -1f;
                }
                base.content.localPosition += new Vector3(move.x, 0f, 0f);
                m_drag = true;
            }
            else if (vertical)
            {
                m_startDragPos = content.localPosition.y;
                Vector2 scrollOffset = data.delta / (float)Camera.main.pixelWidth * m_screenWidth;
                if (Inverted)
                {
                    scrollOffset.y *= -1f;
                }
                content.localPosition += new Vector3(0f, scrollOffset.y, 0f);
                m_drag = true;
            }
            // remember the finger so a second touch can't take over the drag
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
                base.content.localPosition += new Vector3(move.x, 0f, 0f);
                UpdateScale(false);
            }
            else if (vertical)
            {
                Vector2 scrollOffset = data.delta / (float)Camera.main.pixelWidth * m_screenWidth;
                if (Inverted)
                {
                    scrollOffset.y *= -1f;
                }
                content.localPosition += new Vector3(0f, scrollOffset.y, 0f);
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
                base.content.localPosition += new Vector3(move.x, 0f, 0f);
                float dragDistance = base.content.localPosition.x - m_startDragPos;
                //a drag shorter than 100 snaps back, longer ones move by whole pages
                if (dragDistance > 0f)
                {
                    if (dragDistance < 100f)
                    {
                        base.StartCoroutine(MoveCoroutine(0f - dragDistance));
                    }
                    else
                    {
                        //dragged past the first page
                        if (base.content.anchoredPosition.x > 0f)
                        {
                            base.StartCoroutine(MoveCoroutine(0f - base.content.anchoredPosition.x));
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
                    //dragged past the last page, clamp to it
                    if (base.content.anchoredPosition.x - m_rectTransform.rect.width < (0f - m_list[0].rect.width) * (float)m_list.Count - m_contentGroup.spacing * (float)(m_list.Count - 1))
                    {
                        float lastPos = (0f - (m_list[0].rect.width + m_contentGroup.spacing)) * (float)(m_list.Count - 1);
                        base.StartCoroutine(MoveCoroutine(lastPos - base.content.anchoredPosition.x));
                    }
                    else
                    {
                        int steps = Mathf.RoundToInt((0f - dragDistance) / (m_list[m_currentElement].rect.width + m_contentGroup.spacing));
                        base.StartCoroutine(MoveCoroutine((0f - (m_list[m_currentElement].rect.width + m_contentGroup.spacing)) * (float)Mathf.Max(1, steps) - dragDistance));
                    }
                }
            }
            else if (vertical)
            {
                Vector2 scrollOffset = data.delta / (float)Camera.main.pixelWidth * m_screenWidth;
                content.localPosition += new Vector3(0f, scrollOffset.y, 0f);
                float dragY = content.localPosition.y - m_startDragPos;
                if (dragY > 0f)
                {
                    if (dragY < 100f)
                    {
                        base.StartCoroutine(MoveCoroutine(0f - dragY));
                    }
                    else
                    {
                        if (content.anchoredPosition.y + m_rectHeight > m_list[m_currentElement].rect.height * (float)m_list.Count + m_contentGroup.spacing * (float)(m_list.Count - 1))
                        {
                            float maxY = content.rect.height - m_rectHeight;
                            base.StartCoroutine(MoveCoroutine(maxY - content.anchoredPosition.y));
                        }
                        else
                        {
                            base.StartCoroutine(MoveCoroutine(m_list[m_currentElement].rect.height + m_contentGroup.spacing - dragY));
                        }
                    }
                }
                else if (dragY > -100f)
                {
                    base.StartCoroutine(MoveCoroutine(0f - dragY));
                }
                else
                {
                    if (content.anchoredPosition.y - m_rectHeight < 0f)
                    {
                        base.StartCoroutine(MoveCoroutine(0f - content.anchoredPosition.y));
                    }
                    else
                    {
                        base.StartCoroutine(MoveCoroutine(0f - (m_list[m_currentElement].rect.height + m_contentGroup.spacing) - dragY));
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
                    content.anchoredPosition = new Vector2(x, base.content.anchoredPosition.y);
                }
                else if (vertical)
                {
                    content.anchoredPosition = new Vector2(content.anchoredPosition.x, (m_list[0].rect.height + m_contentGroup.spacing) * (float)index);
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
                m_currentElement = Mathf.RoundToInt((0f - content.anchoredPosition.x) / (m_list[0].rect.width + m_contentGroup.spacing));
            }
            else
            {
                //pages have different widths, walk them until we pass the content position
                float pos = (0f - m_contentGroup.spacing) / 2f + (float)m_contentGroup.padding.left;
                for (int i = 0; i < m_list.Count; i++)
                {
                    pos += m_list[i].rect.width + m_contentGroup.spacing;
                    if (content.anchoredPosition.x > 0f - pos)
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
            m_currentElement = Mathf.RoundToInt(content.anchoredPosition.y / (m_list[0].rect.height + m_contentGroup.spacing));
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
                    float distance = 0f;
                    if (horizontal)
                    {
                        if (i > 0)
                        {
                            pos += m_list[i - 1].rect.width + m_contentGroup.spacing;
                        }
                        distance = Mathf.Abs(content.anchoredPosition.x + pos);
                    }
                    else if (vertical)
                    {
                        pos = m_list[i].rect.height * (float)i + m_contentGroup.spacing * (float)i;
                        distance = Mathf.Abs(content.anchoredPosition.y - pos);
                    }
                    //pages closer than DistForScale to the center grow up to Scale
                    if (all || distance < DistForScale + m_list[i].rect.width + m_contentGroup.spacing)
                    {
                        float par = (!(distance < DistForScale)) ? 1f : Mathf.Lerp(Scale, 1f, distance / DistForScale);
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

            //move done, snap exactly on the page
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
            // cap the step so a frame hitch doesn't jump pages
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

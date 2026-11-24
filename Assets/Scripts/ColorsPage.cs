using UnityObject = global::UnityEngine.Object;


using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ColorsPage : MonoBehaviour
{
    public Action<ColorImage, SpecialColorPosition> OnColorClick;

    int m_rowWidth = -1;
    int m_pageIndex = -1;

    [SerializeField] ColorImage m_colorPrefab;

    List<ColorImage> m_colorImages;

    [SerializeField] RectTransform m_firstRow;
    [SerializeField] RectTransform m_secondRow;

    public void Init(List<Color> colors, int startIndex, int rowWidth, float colorSize, int pageIndex)
    {
        m_colorImages = new List<ColorImage>();
        m_rowWidth = rowWidth;
        m_pageIndex = pageIndex;
        bool hasSecondRow = false;
        for (int i = 0; i < colors.Count; i++)
        {
            var colorImage = UnityObject.Instantiate(m_colorPrefab);
            m_colorImages.Add(colorImage);
            colorImage.Init(colors[i], startIndex + i + 1);
            colorImage.OnClick += OnColorClickedHandler;
            //fill the first row, the rest goes to the second one
            if (i < rowWidth)
            {
                colorImage.transform.SetParent(m_firstRow);
            }
            else
            {
                colorImage.transform.SetParent(m_secondRow);
                hasSecondRow = true;
            }
            colorImage.transform.localScale = Vector3.one;
            ((RectTransform)colorImage.transform).sizeDelta = new Vector2(colorSize, colorSize);
            ((RectTransform)colorImage.transform).anchoredPosition = new Vector2((float)(i % rowWidth) * colorSize, 0f);
            colorImage.gameObject.SetActive(true);
        }
        ((RectTransform)transform).sizeDelta = new Vector2((float)rowWidth * colorSize, colorSize * (float)((!hasSecondRow) ? 1 : 2));
        ((RectTransform)m_firstRow.transform).sizeDelta = new Vector2((float)rowWidth * colorSize, colorSize);
        ((RectTransform)m_secondRow.transform).sizeDelta = new Vector2((float)rowWidth * colorSize, (!hasSecondRow) ? 0f : colorSize);
    }

    public void SelectFirstColor()
    {
        if (m_colorImages.Count <= m_rowWidth)
        {
            OnColorClick(m_colorImages[0], SpecialColorPosition.BottomLeft);
        }
        else
        {
            OnColorClick(m_colorImages[0], SpecialColorPosition.None);
        }
    }

    public void DisableColor(Color color)
    {
        var colorImage = m_colorImages.FirstOrDefault((ColorImage a) => a.Color == color);
        if (colorImage != null)
        {
            colorImage.Disable();
        }
    }

    void OnColorClickedHandler(ColorImage colorImage)
    {
        int index = m_colorImages.IndexOf(colorImage);
        //colors at the bottom corners get a special position
        if (m_colorImages.Count <= m_rowWidth)
        {
            if (index == 0 && m_pageIndex == 0)
            {
                OnColorClick(colorImage, SpecialColorPosition.BottomLeft);
            }
            else if (index == m_rowWidth - 1 && m_pageIndex == 0)
            {
                OnColorClick(colorImage, SpecialColorPosition.BottomRight);
            }
            else
            {
                OnColorClick(colorImage, SpecialColorPosition.None);
            }
        }
        else if (index == m_rowWidth)
        {
            OnColorClick(colorImage, SpecialColorPosition.BottomLeft);
        }
        else if (index == 2 * m_rowWidth - 1)
        {
            OnColorClick(colorImage, SpecialColorPosition.BottomRight);
        }
        else
        {
            OnColorClick(colorImage, SpecialColorPosition.None);
        }
    }
}



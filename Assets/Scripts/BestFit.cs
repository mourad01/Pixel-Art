using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BestFit : MonoBehaviour
{
    float timer = 1f;

    [SerializeField] int m_minSize = 16;
    [SerializeField] int m_maxSize = 16;
    [SerializeField] bool m_enableAfterRefit;

    RectTransform m_transform;
    Text m_text;
    string m_textText;
    bool m_textEnabled = true;
    Rect m_rect = default(Rect);
    Canvas m_parentCanvas;
    bool m_waitAndRefit;

    public int MinSize
    {
        get
        {
            return m_minSize;
        }
        set
        {
            m_minSize = value;
        }
    }

    public int MaxSize
    {
        get
        {
            return m_maxSize;
        }
        set
        {
            m_maxSize = value;
        }
    }

    void Start()
    {
        if (m_text == null)
        {
            m_text = base.GetComponent<Text>();
            m_transform = (transform as RectTransform);
            base.StartCoroutine(WaitCanvasCoroutine());
        }
    }

    IEnumerator WaitCanvasCoroutine()
    {
        if (m_parentCanvas == null)
        {
            Transform tr = m_transform;
            while (((Component)tr).GetComponent<Canvas>() == null)
            {
                tr = tr.parent;
                if (tr == null)
                {
                    break;
                }
            }
            if (tr != null)
            {
                m_parentCanvas = ((Component)tr).GetComponent<Canvas>();
            }
            yield return null;
        }
    }

    void Update()
    {
        if (m_parentCanvas != null && !m_parentCanvas.enabled)
        {
            return;
        }
        //only watch the text for the first second
        timer -= Time.deltaTime;
        if (timer < 0f)
        {
            enabled = false;
        }
        if (m_textText != m_text.text || m_waitAndRefit)
        {
            m_textEnabled = m_text.enabled;
            m_text.enabled = false;
            Refit();
        }
        if (!(m_transform.rect != m_rect))
        {
            return;
        }
        //rect changed, refit unless it only got smaller in width
        if (!(m_rect.size.x > m_transform.rect.size.x))
        {
            if (m_rect.size.y > m_transform.rect.size.y)
            {
                m_rect = m_transform.rect;
                return;
            }
            m_textEnabled = m_text.enabled;
            m_text.enabled = false;
            Refit();
            return;
        } 
        m_rect = m_transform.rect;
    }

    public void Refit()
    {
        if (m_text == null)
        {
            Start();
        }
        m_textText = m_text.text;
        // dynamic fonts can use unity best fit, the others are measured by hand
        if (m_text.font.dynamic)
        {
            m_text.resizeTextForBestFit = true;
            m_text.resizeTextMinSize = m_minSize;
            m_text.resizeTextMaxSize = m_maxSize;
            m_text.enabled = m_textEnabled;
        }
        else
        {
            m_text.fontSize = MinSize;
            m_rect = m_transform.rect;
            if (m_parentCanvas != null && !m_parentCanvas.enabled)
            {
                m_waitAndRefit = true;
            }
            else
            {
                UpdateThis();
            }
        }
    }

    void UpdateThis()
    {
        if (m_textText.Length == 0)
        {
            m_text.enabled = m_textEnabled;
        }
        else
        {
            //measure the width of each word at the font base size
            var textGenerator = new TextGenerator();
            var generationSettings = m_text.GetGenerationSettings(m_transform.rect.size);
            textGenerator.Populate(m_textText, generationSettings);
            var characters = textGenerator.characters;
            var parts = m_textText.Split(' ');
            float baseSpaceLength = 0f;
            if (parts.Length > 1 && characters.Count > 0)
            {
                var uICharInfo = characters[parts[0].Length];
                baseSpaceLength = uICharInfo.charWidth;
            }
            var lineWidths = new float[parts.Length];
            int charIndex = 0;
            for (int i = 0; i < parts.Length; i++)
            {
                lineWidths[i] = 0f;
                for (int j = 0; j < parts[i].Length; j++)
                {
                    try
                    {
                        var character = characters[charIndex++];
                        lineWidths[i] += character.charWidth;
                    }
                    catch
                    {
                    }
                }
                charIndex++;
            }
            if (CheckSize(lineWidths, m_maxSize, baseSpaceLength))
            {
                SetSize(m_maxSize);
            }
            else
            {
                CheckRange(lineWidths, m_minSize, m_maxSize, baseSpaceLength);
            }
        }
    }

    void CheckRange(float[] wordLengths, int minSize, int maxSize, float baseSpaceLength)
    {
        //binary search for the biggest size that fits
        int middle = (minSize + maxSize) / 2;
        if (!CheckSize(wordLengths, middle, baseSpaceLength))
        {
            if (minSize == middle)
            {
                SetSize(middle);
            }
            else
            {
                CheckRange(wordLengths, minSize, middle, baseSpaceLength);
            }
        }
        else
        {
            float upperMiddle = (float)((middle + maxSize) / 2);
            if (maxSize == middle || upperMiddle == (float)middle)
            {
                SetSize(middle);
            }
            else
            {
                CheckRange(wordLengths, middle, maxSize, baseSpaceLength);
            }
        }
    }

    bool CheckSize(float[] wordLengths, int targetSize, float baseSpaceLength)
    {
        float scale = (float)targetSize / (float)m_text.font.fontSize;
        var scaledLengths = new float[wordLengths.Length];
        for (int i = 0; i < wordLengths.Length; i++)
        {
            scaledLengths[i] = wordLengths[i] * scale;
        }
        float spaceWidth = baseSpaceLength * scale;
        int linePixels = Mathf.CeilToInt((float)m_text.font.lineHeight * scale);
        if ((float)linePixels >= m_transform.rect.height)
        {
            return false;
        }
        //wrap words into lines and fail if the lines don't fit in height
        var list = new List<float>();
        list.Add(0f);
        int line = 0;
        int word = 0;
        while (word < scaledLengths.Length)
        {
            if (list[line] + spaceWidth + scaledLengths[word] < m_transform.rect.width)
            {
                List<float> list2;
                int index;
                list2 = list; index = line; (list2)[index] = list2[index] + (spaceWidth + scaledLengths[word]);
                word++;
                continue;
            }
            if (!((float)((list.Count + 1) * linePixels) < m_transform.rect.height))
            {
                return false;
            }
            list.Add(0f);
            line++;
        }
        return true;
    }

    void SetSize(int targetSize)
    {
        m_text.fontSize = targetSize;
        if (m_enableAfterRefit)
        {
            m_text.enabled = true;
        }
        else
        {
            m_text.enabled = m_textEnabled;
        }
    }
}

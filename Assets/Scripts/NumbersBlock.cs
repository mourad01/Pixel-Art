

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NumbersBlock : MonoBehaviour
{
    [SerializeField] GameObject m_numberPrefab;
    [SerializeField] GameObject m_content;

    public bool Inited { get;  set; }

    public void Init(int width, int height, Vector2 delta, Color[] values, int fullWidth, int startX, int startY, List<Color> colors)
    {
        Inited = true;
        var transform = m_content.transform;
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                float x = delta.x * ((float)i + 0.55f);
                float y = delta.y * ((float)j + 0.5f);
                var instance = Object.Instantiate(m_numberPrefab);
                var transform2 = instance.transform;
                transform2.SetParent(transform);
                transform2.localScale = Vector3.one;
                transform2.localPosition = new Vector2(x, y);
                var textMesh = instance.GetComponent<TextMesh>();
                textMesh.text = colors.IndexOf(values[startX + i + (startY + j) * fullWidth]).ToString();
                textMesh.characterSize = 0.125f * Mathf.Max(delta.x, delta.y);
            }
        }
    }

    void OnBecameInvisible()
    {
        m_content.SetActive(false);
    }
    IEnumerator OnBecameVisible()
    {
        yield return null;
        m_content.SetActive(true);
        yield return null;
    }
}


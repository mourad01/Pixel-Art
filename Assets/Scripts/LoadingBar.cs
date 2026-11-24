

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadingBar : MonoBehaviour
{
    [SerializeField] List<Image> m_images;
    [SerializeField] List<Color> m_colors;
    [SerializeField] float m_deltaTime = 1f;

    void Start()
    {
        StartBar();
        if (m_images.Count != m_colors.Count)
        {
            Debug.LogError("LoadingBar: wrong parameters! Images.Count != Colors.Count");
        }
        else
        {
            for (int i = 0; i < m_images.Count; i++)
            {
                m_images[i].color = m_colors[i];
            }
        }
    }

    public void StartBar()
    {
        gameObject.SetActive(true);
        base.StartCoroutine(BarCoroutine());
    }

    void Update()
    {
        float dt = Mathf.Min(0.05f, Time.deltaTime);
        transform.Rotate(0f, 0f, dt * 360f);
    }
    IEnumerator BarCoroutine()
    {
        int step = 0;
        int index = 0;
        while (true)
        {
            step++;
            if (step > m_images.Count)
            {
                step -= m_images.Count;
            }
            for (int i = 0; i < m_images.Count; i++)
            {
                index = i - step;
                if (index < 0)
                {
                    index += m_images.Count;
                }
                m_images[i].color = m_colors[index];
            }
            yield return new WaitForSeconds(m_deltaTime);
        }
    }
}

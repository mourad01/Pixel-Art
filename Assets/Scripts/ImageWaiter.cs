using UnityEngine;

public class ImageWaiter : MonoBehaviour
{
    [SerializeField] float m_timer;
    [SerializeField] float m_time = 1f;
    [SerializeField] Transform[] m_indicators;

    void Update()
    {
        m_timer += Time.deltaTime;
        if (m_timer > 2f * m_time)
        {
            m_timer = 0f;
        }
        for (int i = 0; i < m_indicators.Length; i++)
        {
            float t = (m_timer - (float)i * m_time / 6f) / m_time;
            if (t < 0f)
            {
                t += 2f;
            }
            float value2;
            if (t < 1f)
            {
                value2 = Mathf.Lerp(0.3f, 1f, t);
            }
            else
            {
                t -= 1f;
                value2 = Mathf.Lerp(1f, 0.3f, t);
            }
            m_indicators[i].localScale = new Vector3(value2, value2, 1f);
        }
    }
}



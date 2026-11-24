using UnityEngine;

public class ScreenFormatScale : MonoBehaviour
{
    [SerializeField] float m_9x16Scale = 1f;
    [SerializeField] float m_3x4Scale = 1f;

    void Start()
    {
        float aspect = (float)Screen.height / (float)Screen.width;
        float value2 = Mathf.Lerp(m_3x4Scale, m_9x16Scale, (aspect - 1.33f) / 0.449999928f);
        transform.localScale = new Vector3(value2, value2, 1f);
    }
}



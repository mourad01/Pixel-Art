using UnityEngine;

public class ScreenFormatPos : MonoBehaviour
{
    [SerializeField] Vector2 m_pos_4x3 = Vector2.zero;

    void Awake()
    {
        bool found = false;
        if (Screen.width > Screen.height)
        {
            if ((float)Screen.width / (float)Screen.height < 1.51f)
            {
                found = true;
            }
        }
        else if ((float)Screen.height / (float)Screen.width < 1.51f)
        {
            found = true;
        }
        if (found)
        {
            ((RectTransform)transform).anchoredPosition = m_pos_4x3;
        }
    }
}



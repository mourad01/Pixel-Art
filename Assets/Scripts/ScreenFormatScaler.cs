using UnityEngine;

internal class ScreenFormatScaler : MonoBehaviour
{
    [SerializeField] float m_forceScaleX = 1f;
    [SerializeField] float m_forceScaleY = 1f;

    void Awake()
    {
        bool found = false;
        if (Screen.width > Screen.height)
        {
            if ((float)Screen.width / (float)Screen.height < 1.45f)
            {
                found = true;
            }
        }
        else if ((float)Screen.height / (float)Screen.width < 1.45f)
        {
            found = true;
        }
        if (found)
        {
            transform.localScale = new Vector3(m_forceScaleX, m_forceScaleY, 1f);
        }
    }
}



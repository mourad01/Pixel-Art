using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class ScreenFontSize : MonoBehaviour
{
    [SerializeField] int m_fontSize_3x4 = 100;

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
            base.GetComponent<Text>().fontSize = m_fontSize_3x4;
        }
    }
}



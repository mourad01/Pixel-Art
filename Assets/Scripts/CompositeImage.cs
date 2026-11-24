using UnityEngine;

public class CompositeImage : MonoBehaviour
{
    [SerializeField] int m_width = 100;
    [SerializeField] int m_height = 100;
    [SerializeField] int m_pixelsPerUnit = 100;

    public int Width
    {
        get
        {
            return m_width;
        }
        set
        {
            m_width = value;
        }
    }

    public int Height
    {
        get
        {
            return m_height;
        }
        set
        {
            m_height = value;
        }
    }

    public int PixelsPerUnit
    {
        get
        {
            return m_pixelsPerUnit;
        }
        set
        {
            m_pixelsPerUnit = value;
        }
    }
}



using UnityEngine;

public class ImageWaiterNew : MonoBehaviour
{
    float m_angle;

    [SerializeField] Transform m_image;

    void Update()
    {
        if (m_angle <= 10f)
        {
            m_angle += 360f;
        }
        if (m_angle < 90f)
        {
            m_angle -= Mathf.Min(0.05f, Time.deltaTime) * m_angle * 3f;
        }
        else
        {
            m_angle -= Mathf.Min(0.05f, Time.deltaTime) * 270f;
        }
        Quaternion rotation = m_image.rotation;
        rotation.eulerAngles = new Vector3(0f, 0f, m_angle);
        m_image.rotation = rotation;
    }
}



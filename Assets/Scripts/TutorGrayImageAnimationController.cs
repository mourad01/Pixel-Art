using UnityEngine;

public class TutorGrayImageAnimationController : MonoBehaviour
{
    [SerializeField] Material m_material;
    [SerializeField] float m_alpha = 1f;

    void Update()
    {
        m_material.SetFloat("_Alpha", m_alpha);
    }
}



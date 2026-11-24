using System.Collections.Generic;
using UnityEngine;

public class PlatformElement : MonoBehaviour
{
    [SerializeField] GameObject target;
    [SerializeField] List<RuntimePlatform> platforms;

    void Awake()
    {
        if (platforms.Contains(Application.platform))
        {
            target.SetActive(true);
        }
        else
        {
            gameObject.SetActive(false);
        }
    }
}



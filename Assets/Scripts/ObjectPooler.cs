using UnityEngine;

public class ObjectPooler : MonoBehaviour
{
    public static ObjectPooler Instance;

    protected virtual void Awake()
    {
        Instance = this;
    }

    protected virtual void Start()
    {
        FillObjectPool();
    }

    protected virtual void FillObjectPool()
    {
    }

    public virtual GameObject GetPooledGameObject()
    {
        return null;
    }
}



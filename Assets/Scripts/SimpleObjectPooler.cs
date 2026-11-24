

using System.Collections.Generic;
using UnityEngine;

public class SimpleObjectPooler : ObjectPooler
{
    public GameObject GameObjectToPool;
    public int PoolSize = 20;
    public bool PoolCanExpand = true;

    protected GameObject _waitingPool;
    protected List<GameObject> _pooledGameObjects;

    protected override void FillObjectPool()
    {
        _waitingPool = new GameObject("[SimpleObjectPooler] " + name);
        _pooledGameObjects = new List<GameObject>();
        for (int i = 0; i < PoolSize; i++)
        {
            AddOneObjectToThePool();
        }
    }

    public void DisableAll()
    {
        if (_pooledGameObjects != null)
        {
            for (int i = 0; i < _pooledGameObjects.Count; i++)
            {
                if (_pooledGameObjects[i].gameObject != null && _pooledGameObjects[i].gameObject.activeInHierarchy)
                {
                    _pooledGameObjects[i].gameObject.SetActive(false);
                }
            }
        }
    }

    public override GameObject GetPooledGameObject()
    {
        for (int i = 0; i < _pooledGameObjects.Count; i++)
        {
            if (!_pooledGameObjects[i].gameObject.activeInHierarchy)
            {
                return _pooledGameObjects[i];
            }
        }
        //all objects busy, grow the pool if allowed
        if (PoolCanExpand)
        {
            return AddOneObjectToThePool();
        }
        return null;
    }

    protected virtual GameObject AddOneObjectToThePool()
    {
        if (GameObjectToPool == null)
        {
            Debug.LogWarning("The " + base.gameObject.name + " ObjectPooler doesn't have any GameObjectToPool defined.", base.gameObject);
            return null;
        }
        var instance = Object.Instantiate(GameObjectToPool);
        instance.gameObject.SetActive(false);
        instance.transform.parent = _waitingPool.transform;
        instance.name = GameObjectToPool.name + "-" + _pooledGameObjects.Count;
        _pooledGameObjects.Add(instance);
        return instance;
    }
}





using System.Collections.Generic;
using UnityEngine;

public class MultipleObjectPooler : ObjectPooler
{
    public GameObject[] GameObjectsToPool;
    public int[] PoolSize;
    public bool PoolCanExpand = true;

    protected GameObject _waitingPool;
    protected List<GameObject> _pooledGameObjects;

    protected override void FillObjectPool()
    {
        _waitingPool = new GameObject("[MultipleObjectPooler] " + name);
        _pooledGameObjects = new List<GameObject>();
        int poolIndex = 0;
        var gameObjectsToPool = GameObjectsToPool;
        foreach (GameObject typeOfObject in gameObjectsToPool)
        {
            if (poolIndex > PoolSize.Length)
            {
                break;
            }
            for (int j = 0; j < PoolSize[poolIndex]; j++)
            {
                AddOneObjectToThePool(typeOfObject);
            }
            poolIndex++;
        }
    }

    protected virtual GameObject AddOneObjectToThePool(GameObject typeOfObject)
    {
        var instance = Object.Instantiate(typeOfObject);
        instance.gameObject.SetActive(false);
        instance.transform.parent = _waitingPool.transform;
        instance.name = typeOfObject.name + "-" + _pooledGameObjects.Count;
        _pooledGameObjects.Add(instance);
        return instance;
    }

    public override GameObject GetPooledGameObject()
    {
        //pick random objects until an inactive one is found
        int index = Random.Range(0, _pooledGameObjects.Count);
        int tries = 0;
        while (_pooledGameObjects[index].gameObject.activeInHierarchy && tries < _pooledGameObjects.Count)
        {
            index = Random.Range(0, _pooledGameObjects.Count);
            tries++;
        }
        if (_pooledGameObjects[index].gameObject.activeInHierarchy)
        {
            if (PoolCanExpand)
            {
                index = Random.Range(0, GameObjectsToPool.Length);
                return AddOneObjectToThePool(GameObjectsToPool[index]);
            }
            return null;
        }
        return _pooledGameObjects[index];
    }

    public virtual GameObject GetPooledGameObjectOfType(string type)
    {
        GameObject gameObject = null;
        for (int i = 0; i < _pooledGameObjects.Count; i++)
        {
            if (_pooledGameObjects[i].name.Contains(type))
            {
                if (!_pooledGameObjects[i].gameObject.activeInHierarchy)
                {
                    return _pooledGameObjects[i];
                }
                gameObject = _pooledGameObjects[i];
            }
        }
        // all of this type are busy, clone one of them
        if (PoolCanExpand && gameObject != null)
        {
            var pooledInstance = Object.Instantiate(gameObject);
            _pooledGameObjects.Add(pooledInstance);
            return pooledInstance;
        }
        return null;
    }
}



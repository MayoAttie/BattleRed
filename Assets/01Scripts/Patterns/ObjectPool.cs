using System.Collections.Generic;
using UnityEngine;

public class ObjectPool<T> : Singleton<ObjectPool<T>> where T : Component
{
    public GameObject prefab;
    public int initialPoolSize;

    private List<T> pool = new List<T>();

    public ObjectPool(GameObject prefab, int prefabPollSize)
    {
        this.prefab = prefab;
        initialPoolSize = prefabPollSize;
        InitializePool();
    }

    private void InitializePool()
    {
        for (int i = 0; i < initialPoolSize; i++)
        {
            T obj = Object.Instantiate(prefab).GetComponent<T>();
            obj.gameObject.SetActive(false);
            pool.Add(obj);
        }
    }

    public T GetFromPool(Vector3 position, Quaternion rotation)
    {
        foreach (T obj in pool)
        {
            if (!obj.gameObject.activeInHierarchy)
            {
                obj.transform.position = position;
                obj.transform.rotation = rotation;
                obj.gameObject.SetActive(true);
                return obj;
            }
        }

        T newObj = Object.Instantiate(prefab, position, rotation).GetComponent<T>();
        pool.Add(newObj);
        return newObj;
    }
}
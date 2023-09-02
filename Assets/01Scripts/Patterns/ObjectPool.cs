using System.Collections.Generic;
using UnityEngine;

public class ObjectPool<T> : MonoBehaviour where T : Component
{
    public GameObject prefab;
    public int initialPoolSize;

    private List<T> pool = new List<T>();

    // 생성자: 초기 오브젝트 풀 설정
    public ObjectPool(GameObject prefab, int prefabPollSize, Transform parent = null)
    {
        this.prefab = prefab;
        initialPoolSize = prefabPollSize;
        InitializePool(parent);
    }

    // 오브젝트 풀 초기화
    private void InitializePool(Transform parent = null)
    {
        for (int i = 0; i < initialPoolSize; i++)
        {
            T obj = Object.Instantiate(prefab).GetComponent<T>();
            obj.gameObject.SetActive(false);

            // 만약 parent가 제공되었다면 부모로 설정
            if (parent != null)
            {
                obj.transform.SetParent(parent);
            }

            pool.Add(obj);
        }
    }
    // 오브젝트 풀에서 오브젝트 가져오기 및 부모 설정
    public T GetFromPool(Vector3 position, Quaternion rotation, Transform parent = null)
    {
        foreach (T obj in pool)
        {
            if (!obj.gameObject.activeInHierarchy)
            {
                if(parent != null)
                    obj.transform.SetParent(parent);
                obj.transform.position = position;
                obj.transform.rotation = rotation;
                obj.gameObject.SetActive(true);
                return obj;
            }
        }

        T newObj = Object.Instantiate(prefab, position, rotation).GetComponent<T>();
        if (parent != null)
            newObj.transform.SetParent(parent);
        pool.Add(newObj);
        return newObj;
    }

    // 오브젝트 풀로 오브젝트 반환
    public void ReturnToPool(T obj)
    {
        obj.gameObject.SetActive(false);
    }

    // 전체 오브젝트 풀 반환
    public void AllReturnToPool()
    {
        foreach (T obj in pool)
        {
            obj.gameObject.SetActive(false);
        }
    }

    public List<T> GetPoolList() { return pool; }
}
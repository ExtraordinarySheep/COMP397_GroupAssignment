using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ObjectPoolingManager<T> : PersistentSingleton<ObjectPoolingManager<T>> where T : Component
{
    [SerializeField] private T poolPrefab;
    private Queue<T> pool = new Queue<T>();

    public T Get()
    {
        if (pool.Count == 0)
        {
            AddPrefab(1);
        }
        return pool.Dequeue();
    }

    private void AddPrefab(int count)
    {
        for (int i = 0; i < count; i++)
        {
            var prefab = Instantiate(poolPrefab);
            prefab.gameObject.SetActive(false);
            pool.Enqueue(prefab);
        }
    }

    public void ReturnToPool(T prefab)
    {
        prefab.gameObject.SetActive(false);
        pool.Enqueue(prefab);
    }
}
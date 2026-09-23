using System.Collections.Generic;
using UnityEngine;

namespace BitorTools.Pooling
{
    public class ObjectPool<T> where T : Component
{
    private readonly T prefab;
    private readonly Queue<T> objects = new Queue<T>();
    
    public ObjectPool(T prefab, int initialSize)
    {
        this.prefab = prefab;
        
        for (int i = 0; i < initialSize; i++)
        {
            T obj = Object.Instantiate(prefab);
            obj.gameObject.SetActive(false);
            objects.Enqueue(obj);
        }
    }
    
    public T Get()
    {
        if (objects.Count > 0)
        {
            T obj = objects.Dequeue();
            obj.gameObject.SetActive(true);
            return obj;
        }
        
        return Object.Instantiate(prefab);
    }
    
    public void Return(T obj)
    {
        obj.gameObject.SetActive(false);
        objects.Enqueue(obj);
    }
}
}

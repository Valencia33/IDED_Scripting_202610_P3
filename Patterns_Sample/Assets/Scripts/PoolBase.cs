using System.Collections.Generic;
using UnityEngine;

public abstract class AbstractTargetPool : MonoBehaviour
{
    [SerializeField] protected Target prefab;
    [SerializeField] protected int initialSize = 5;
    
    protected Queue<Target> poolQueue = new Queue<Target>();

    public void Initialize(Target targetPrefab)
    {
        this.prefab = targetPrefab;
        for (int i = 0; i < initialSize; i++)
        {
            AddInstanceToPool();
        }
    }

    protected void AddInstanceToPool()
    {
        Target instance = Instantiate(prefab, transform);
        ReturnToPool(instance);
    }

    public virtual Target GetFromPool()
    {
        if (poolQueue.Count == 0)
        {
            AddInstanceToPool();
        }

        Target instance = poolQueue.Dequeue();
        instance.gameObject.SetActive(true);
        instance.transform.SetParent(null);
        return instance;
    }

    public virtual void ReturnToPool(Target instance)
    {
        instance.gameObject.SetActive(false);
        instance.transform.SetParent(transform);
        instance.transform.localPosition = Vector3.zero;
        poolQueue.Enqueue(instance);
    }
}

public class SpecificTargetPool : AbstractTargetPool 
{

}
using System.Collections.Generic;
using UnityEngine;

public class TargetFacade : MonoBehaviour
{
    private static TargetFacade instance;
    public static TargetFacade Instance => instance;

    private Dictionary<int, SpecificTargetPool> targetPools = new Dictionary<int, SpecificTargetPool>();

    private Transform mainPoolContainer;

    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        GameObject poolObj = GameObject.Find("Pool"); //profe yo he escuchado q esto es maluco usarlo pq hay veces da errores pero lo voy a ignorar pq ya sé q hay uno en la escena
        if (poolObj != null) 
        {
            mainPoolContainer = poolObj.transform;
        }
        else 
        {
            mainPoolContainer = this.transform; //por si las moscas
        }
    }

    public Target GetTarget(Target[] availablePrefabs)
    {
        int randomIndex = Random.Range(0, availablePrefabs.Length);
        Target selectedPrefab = availablePrefabs[randomIndex];
        int prefabID = selectedPrefab.GetInstanceID();

        if (!targetPools.ContainsKey(prefabID))
        {
            GameObject poolGO = new GameObject(selectedPrefab.name);
            poolGO.transform.SetParent(mainPoolContainer);
            
            SpecificTargetPool newPool = poolGO.AddComponent<SpecificTargetPool>();
            newPool.Initialize(selectedPrefab);
            targetPools.Add(prefabID, newPool);
        }

        Target target = targetPools[prefabID].GetFromPool();
        target.PoolID = prefabID; 
        return target;
    }

    public void ReturnTarget(Target target)
    {
        if (targetPools.ContainsKey(target.PoolID))
        {
            targetPools[target.PoolID].ReturnToPool(target);
        }
        else
        {
            Destroy(target.gameObject);
        }
    }
}
using UnityEngine;

public class SpawnController : MonoBehaviour
{
    [SerializeField]
    private float spawnRate = 1f;

    [SerializeField]
    private float firstSpawnDelay = 0f;

    private Vector3 spawnPoint;

    private void Start()
    {
        if (TargetFactory.Instance != null && TargetFacade.Instance != null)
        {
            InvokeRepeating("SpawnObject", firstSpawnDelay, spawnRate);

            if (Player.Instance != null)
            {
                Player.Instance.OnPlayerDied += StopSpawning;
            }
        }
    }

    private void SpawnObject()
    {
        Target target = TargetFacade.Instance.GetTarget(((TargetFactory)TargetFactory.Instance).GetRawPrefabs());

        if (target != null)
        {
            spawnPoint = Camera.main.ViewportToWorldPoint(new Vector3(Random.Range(0F, 1F), 1F, transform.position.z));

            target.transform.position = spawnPoint;
            target.transform.rotation = Quaternion.identity;
            
            target.ActivateTarget(); 
        }
    }

    private void StopSpawning() => CancelInvoke();
}
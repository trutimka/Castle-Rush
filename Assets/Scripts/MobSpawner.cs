using System.Linq;
using UnityEngine;

public class MobSpawner : Building
{
    [SerializeField]
    private GameObject mobPrefab;
    
    protected void SpawnMob(Transform target)
    {
        Instantiate(mobPrefab, SpawnPoints.OrderBy(spawnPoint => Vector3.Distance(spawnPoint.transform.position, target.position))
            .FirstOrDefault().transform.position, Quaternion.identity);
    }
}

using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MobSpawner : Building
{
    [SerializeField]
    private GameObject mobPrefab;

    private List<Building> _targets;

    public void Init()
    {
        OnLevelChanged += ResetTargets;
    }
    protected void SpawnMob(Transform target)
    {
        Instantiate(mobPrefab, SpawnPoints.OrderBy(spawnPoint => Vector3.Distance(spawnPoint.transform.position, target.position))
            .FirstOrDefault().transform.position, Quaternion.identity);
    }

    private bool SetTarget(Building target)
    {
        if (_targets.Count >= Level) return false;
        if (_targets.Contains(target)) return false;
        _targets.Add(target);
        return true;
    }

    private void ResetTargets()
    {
        if (_targets.Count > Level) _targets.RemoveRange(0, _targets.Count - Level);
    }
    
}

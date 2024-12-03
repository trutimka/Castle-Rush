using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = Unity.Mathematics.Random;

public class SpawnerTest : MonoBehaviour
{
    [SerializeField] private Transform _spawnPointsHolder;
    
    [SerializeField] private GameObject _spawnerPrefab;
    [SerializeField] private GameObject _goldFarmPrefab;
    [SerializeField] private GameObject _shootingTowerPrefab;

    private List<SpawnPlace> _spawnPlaces = new List<SpawnPlace>();
    
    private void Start()
    {
        _spawnPlaces = _spawnPointsHolder.GetComponentsInChildren<SpawnPlace>().ToList();
        
        FulfillGameField();
    }
    
    

    private void FulfillGameField()
    {
        foreach (var spawnPlace in _spawnPlaces)
        {
            if (UnityEngine.Random.Range(0, 101) < 51)
            {
                Spawn(spawnPlace);
            }
        }
    }

    private void Spawn(SpawnPlace spawnPlace)
    {
        Instantiate(GetRandomBuilding(), spawnPlace.transform.position, Quaternion.identity);
    }

    private GameObject GetRandomBuilding()
    {
        int randomIndex = UnityEngine.Random.Range(0, 3);

        if (randomIndex == 0) return _spawnerPrefab;
        if (randomIndex == 1) return _goldFarmPrefab;
        if (randomIndex == 2) return _shootingTowerPrefab;

        return null;
    }
}

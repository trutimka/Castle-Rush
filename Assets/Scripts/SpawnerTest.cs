using System.Collections.Generic;
using System.Linq;
using UnityEngine;

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
            if (spawnPlace.SpawnAllowed && NoBuildingsNearby(spawnPlace))
            {
                Spawn(spawnPlace);
                MarkNeighboursOccupied(spawnPlace);
            }
        }
    }

    private bool NoBuildingsNearby(SpawnPlace spawnPlace)
    {
        return spawnPlace.Neighbours.All(neighbour => neighbour.SpawnAllowed);
    }

    private void Spawn(SpawnPlace spawnPlace)
    {
        var randomRotation = GetRandomRotation();
        var building = Instantiate(GetRandomBuilding(), spawnPlace.transform.position, randomRotation);
        spawnPlace.ChangeSpawnAllowed(false);
    }

    private Quaternion GetRandomRotation()
    {
        float randomY = Random.Range(0f, 360f);

        return Quaternion.Euler(0f, randomY, 0);
    }

    private void MarkNeighboursOccupied(SpawnPlace spawnPlace)
    {
        foreach (var neighbour in spawnPlace.Neighbours)
        {
            neighbour.ChangeSpawnAllowed(false);
        }
    }

    private GameObject GetRandomBuilding()
    {
        int randomIndex = Random.Range(0, 3);

        if (randomIndex == 0) return _spawnerPrefab;
        if (randomIndex == 1) return _goldFarmPrefab;
        if (randomIndex == 2) return _shootingTowerPrefab;

        return null;
    }
}
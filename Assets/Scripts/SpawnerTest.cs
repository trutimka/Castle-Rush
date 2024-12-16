using System.Collections.Generic;
using System.Linq;
using Photon.Pun;
using UnityEngine;

public class SpawnerTest : MonoBehaviourPun
{
    [SerializeField] private Transform _spawnPointsHolder;

    [SerializeField] private GameObject _spawnerPrefab;
    [SerializeField] private GameObject _goldFarmPrefab;
    [SerializeField] private GameObject _shootingTowerPrefab;

    [SerializeField] private GameObject StartPointPlayer1;
    [SerializeField] private GameObject StartPointPlayer2;

    private List<SpawnPlace> _spawnPlaces = new List<SpawnPlace>();

    private void Start()
    {
        _spawnPlaces = _spawnPointsHolder.GetComponentsInChildren<SpawnPlace>().ToList();
        MarkNeighboursOccupied(_spawnPlaces[1]);
        MarkNeighboursOccupied(_spawnPlaces[_spawnPlaces.Count - 3]);
        Debug.Log(PhotonNetwork.IsMasterClient);
        if (PhotonNetwork.IsMasterClient)
        {
            FulfillGameField();
        }
    }

    private void FulfillGameField()
    {
        foreach (var spawnPlace in _spawnPlaces)
        {
            if (UnityEngine.Random.Range(0, 101) < 51)
            {
                if (spawnPlace.SpawnAllowed && NoBuildingsNearby(spawnPlace))
                {
                    Spawn(spawnPlace);
                    MarkNeighboursOccupied(spawnPlace);
                }
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
        
        var building = PhotonNetwork.Instantiate("Prefabs/Buildings/" + GetRandomBuilding().name, spawnPlace.transform.position, randomRotation);
        var buildingComponent = building.GetComponent<Building>();
        var maxHealth = 60;
        var startHealth = Random.Range(5, maxHealth + 1);
        var countGoldPerSecond = 0.1f;
        buildingComponent.Init(startHealth, maxHealth, countGoldPerSecond);
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
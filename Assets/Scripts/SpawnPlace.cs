using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SpawnPlace : MonoBehaviour
{
    [SerializeField] private List<SpawnPlace> neighbours;

    private bool _spawnAllowed = true;
    public bool SpawnAllowed => _spawnAllowed;

    private void Start()
    {
        Physics.OverlapSphere(transform.position + new Vector3(transform.localScale.x/2, 0, 0), 2).ToList()
            .Where(x =>
            {
                Debug.Log("ss");
                bool result = x.TryGetComponent<SpawnPlace>(out var spawnPlace) &&
                              spawnPlace != GetComponent<SpawnPlace>();
                if (result) neighbours.Add(spawnPlace);
                return result;
            });
        Physics.OverlapSphere(transform.position - new Vector3(transform.localScale.x, 0, 0), 0.2f).ToList()
            .Where(x =>
            {
                bool result = x.TryGetComponent<SpawnPlace>(out var spawnPlace) &&
                              spawnPlace != GetComponent<SpawnPlace>();
                if (result) neighbours.Add(spawnPlace);
                return result;
            });
        Physics.OverlapSphere(transform.position + new Vector3(0, 0, transform.localScale.z), 0.2f).ToList()
            .Where(x =>
            {
                bool result = x.TryGetComponent<SpawnPlace>(out var spawnPlace) &&
                              spawnPlace != GetComponent<SpawnPlace>();
                if (result) neighbours.Add(spawnPlace);
                return result;
            });
        Physics.OverlapSphere(transform.position + new Vector3(0, 0, -transform.localScale.z), 0.2f).ToList()
            .Where(x =>
            {
                bool result = x.TryGetComponent<SpawnPlace>(out var spawnPlace) &&
                              spawnPlace != GetComponent<SpawnPlace>();
                if (result) neighbours.Add(spawnPlace);
                return result;
            });
    }
}
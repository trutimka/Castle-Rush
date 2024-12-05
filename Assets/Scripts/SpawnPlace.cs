using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SpawnPlace : MonoBehaviour
{
    [SerializeField] private List<SpawnPlace> neighbours;
    public List<SpawnPlace> Neighbours => neighbours;
    
    
    private bool _spawnAllowed = true;
    public bool SpawnAllowed => _spawnAllowed;

    private void Awake()
    {
        Vector3[] directions = 
        {
            Vector3.right, 
            Vector3.left, 
            Vector3.forward, 
            Vector3.back
        };

        float checkDistance = transform.localScale.x * 0.5f + 0.1f;

        foreach (var dir in directions)
        {
            CheckNeighbour(dir * checkDistance);
        }
    }

    private void CheckNeighbour(Vector3 offset)
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position + offset, 0.1f);

        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.TryGetComponent<SpawnPlace>(out var spawnPlace) && 
                spawnPlace != this && 
                !neighbours.Contains(spawnPlace))
            {
                neighbours.Add(spawnPlace);
            }
        }
    }

    public void ChangeSpawnAllowed(bool value)
    {
        _spawnAllowed = value;
    }
    
    
}
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Photon.Pun;
using UnityEngine;

public class MobSpawner : Building
{
    [SerializeField]
    private GameObject mobPrefab;
    
    private List<Tuple<GameObject, GameObject>> _roads = new List<Tuple<GameObject, GameObject>>();
    
    [SerializeField]
    private float spawnInterval = 3f; // Интервал между спавнами в секундах

    private bool isSpawning = false; // Флаг, чтобы контролировать процесс спавна
    private Coroutine spawnCoroutine; // Ссылка на корутину спавна
    
    public event Action<GameObject, GameObject> OnRemoveTarget;

    public void Init()
    {
        OnLevelChanged += UpdateLevel;
        OnOwnerChanged += RestartSpawn;
    }
    
    public bool AddTarget(GameObject start, GameObject target)
    {
        Debug.Log("Adding " + target.name + " to MobSpawner");
        if (_roads.Count >= Level) return false;
        if (_roads.FindAll(t => (t.Item1 == start && t.Item2 == target)).Count() != 0) return false;
        _roads.Add(new Tuple<GameObject, GameObject>(start, target));
        Debug.Log(target.name + " is added to MobSpawner");
        return true;
    }

    // Удалить цель (если линия удалена)
    public void RemoveTarget(GameObject start, GameObject target)
    {
        _roads.RemoveAll(t => t.Item1 == target && t.Item2 == target);
        if (_roads.Count == 0) StopSpawning();
    }

    public void StartSpawning()
    {
        if (isSpawning || _roads.Count == 0) return;
        isSpawning = true;
        spawnCoroutine = StartCoroutine(SpawnMobsPeriodically());
        Debug.Log("Spawning Mobs");
    }

    // Остановка спавна
    public void StopSpawning()
    {
        if (!isSpawning) return;
        isSpawning = false;
        if (spawnCoroutine != null)
        {
            StopCoroutine(spawnCoroutine);
        }
    }

    // Сбрасываем цели, если уровень изменился
    private void UpdateLevel()
    {
        // Debug.Log("Delete target: " + _targets.Last());
        if (_roads.Count > Level)
        {
            OnRemoveTarget?.Invoke(_roads.Last().Item1, _roads.Last().Item2);
            var isDeleted = _roads.Remove(_roads.Last());
            Debug.Log("Is Deleted = " + isDeleted);
        }
    }

    private void RestartSpawn()
    {
        StopSpawning();
        foreach (var road in _roads)
        {
            OnRemoveTarget?.Invoke(road.Item1, road.Item2);
        }
        _roads.Clear();
    }

    // Корутин для периодического спавна мобов
    private IEnumerator SpawnMobsPeriodically()
    {
        while (isSpawning)
        {
            foreach (var target in _roads)
            {
                if (target == null) continue; // Если цель была удалена
                SpawnMob(target.Item2);
            }

            yield return new WaitForSecondsRealtime(spawnInterval); // Ждем интервал перед следующим спавном
            
        }

        yield return null;
    }

    // Метод для спавна одного моба
    private void SpawnMob(GameObject target)
    {
        var spawnPoint = SpawnPoints
            .OrderBy(spawnPoint => Vector3.Distance(spawnPoint.transform.position, target.transform.position))
            .FirstOrDefault();

        if (spawnPoint != null)
        {
            Debug.Log(mobPrefab.name);
            var mob = PhotonNetwork.Instantiate("Prefabs/Mobs/" + mobPrefab.name, spawnPoint.transform.position, Quaternion.identity);
            var mobComponent = mob.GetComponent<Mob>();

            if (mobComponent != null)
            {
                mobComponent.SetOwner(Owner);
                mobComponent.Target = target; // Устанавливаем цель для моба
            }
        }
    }
}

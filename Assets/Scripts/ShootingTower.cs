
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ShootingTower : Building
{
    [SerializeField]
    private List<Tuple<GameObject, GameObject>> _roads = new List<Tuple<GameObject, GameObject>>();
    
    [SerializeField]
    private float shootingInterval = 3f; // Интервал между выстрелами в секундах

    private bool isShooting = false; // Флаг, чтобы контролировать процесс
    private Coroutine shootingCoroutine; // Ссылка на корутину

    [SerializeField]
    private float maxDistance = 13f;
    public float MaxDistance => maxDistance * Level;
    
    public event Action<GameObject, GameObject> OnRemoveTarget;

    public void Init()
    {
        OnLevelChanged += UpdateLevel;
        OnOwnerChanged += RestartSpawn;
    }
    
    public bool AddTarget(GameObject start, GameObject target)
    {
        Debug.Log("Adding " + target.name + " to ShootingTower");
        if (_roads.Count >= Level) return false;
        if (_roads.FindAll(t => (t.Item1 == start && t.Item2 == target)).Count() != 0) return false;
        _roads.Add(new Tuple<GameObject, GameObject>(start, target));
        Debug.Log(target.name + " is added to ShootingTower");
        return true;
    }

    // Удалить цель (если линия удалена)
    public void RemoveTarget(GameObject start, GameObject target)
    {
        _roads.RemoveAll(t => t.Item1 == start && t.Item2 == target);
        if (_roads.Count == 0) StopSpawning();
    }

    public void StartShooting()
    {
        if (isShooting || _roads.Count == 0) return;
        isShooting = true;
        shootingCoroutine = StartCoroutine(ShootPeriodically());
        Debug.Log("Shooooooooooooting");
    }

    // Остановка спавна
    public void StopSpawning()
    {
        if (!isShooting) return;
        isShooting = false;
        if (shootingCoroutine != null)
        {
            StopCoroutine(shootingCoroutine);
        }
    }

    // Сбрасываем цели, если уровень изменился
    private void UpdateLevel(int level)
    {
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

    private IEnumerator ShootPeriodically()
    {
        while (isShooting)
        {
            foreach (var target in _roads)
            {
                if (target == null) continue; // Если цель была удалена
                ShootInBuilding(target.Item2);
            }

            yield return new WaitForSecondsRealtime(shootingInterval / Owner.Boost); // Ждем интервал перед следующим спавном
            
        }

        yield return null;
    }

    private void ShootInBuilding(GameObject target)
    {
        var spawnPoint = SpawnPoints
            .OrderBy(spawnPoint => Vector3.Distance(spawnPoint.transform.position, target.transform.position))
            .FirstOrDefault();

        if (spawnPoint != null)
        {
            var bimba = Instantiate(Owner.BimbaPrefab, spawnPoint.transform.position, Quaternion.identity);
            var bimbaComponent = bimba.GetComponent<Bimba>();
            
            if (bimbaComponent != null)
            {
                bimbaComponent.SetOwner(Owner);
                bimbaComponent.Target = target; // Устанавливаем цель для моба
            }
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MobSpawner : Building
{
    [SerializeField]
    private GameObject mobPrefab;

    private List<GameObject> _targets = new List<GameObject>();
    
    [SerializeField]
    private float spawnInterval = 3f; // Интервал между спавнами в секундах

    private bool isSpawning = false; // Флаг, чтобы контролировать процесс спавна
    private Coroutine spawnCoroutine; // Ссылка на корутину спавна

    public void Init()
    {
        OnLevelChanged += ResetTargets;
    }
    
    public bool AddTarget(GameObject target)
    {
        // if (_targets.Count >= Level) return false; // Ограничение на количество целей по уровню
        if (_targets.Contains(target)) return false; // Цель уже добавлена
        _targets.Add(target);
        return true;
    }

    // Удалить цель (если линия удалена)
    public void RemoveTarget(GameObject target)
    {
        _targets.Remove(target);
        if (_targets.Count == 0) StopSpawning();
    }

    public void StartSpawning()
    {
        if (isSpawning || _targets.Count == 0) return;
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
    private void ResetTargets()
    {
        if (_targets.Count > Level) _targets.RemoveRange(0, _targets.Count - Level);
    }

    // Корутин для периодического спавна мобов
    private IEnumerator SpawnMobsPeriodically()
    {
        while (isSpawning)
        {
            foreach (var target in _targets)
            {
                if (target == null) continue; // Если цель была удалена
                SpawnMob(target);
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
            var mob = Instantiate(mobPrefab, spawnPoint.transform.position, Quaternion.identity);
            var mobComponent = mob.GetComponent<Mob>();

            if (mobComponent != null)
            {
                mobComponent.Target = target; // Устанавливаем цель для моба
            }
        }
    }

    private bool SetTarget(GameObject target)
    {
        if (_targets.Count >= Level) return false;
        if (_targets.Contains(target)) return false;
        _targets.Add(target);
        return true;
    }
}

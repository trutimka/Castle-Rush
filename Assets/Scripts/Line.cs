using System;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class Line : MonoBehaviour
{
    [SerializeField] private GameObject pointA;
    [SerializeField] private GameObject pointB;
    
    public GameObject GetPointA() => pointA;
    public GameObject GetPointB() => pointB;
    
    private LineRenderer lineRenderer; // Компонент для рендеринга линии
    
    public event Action OnLineCreated;
    
    private bool isSpawn = false;
    public bool IsSpawn => isSpawn;
    
    private void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.positionCount = 2;
        lineRenderer.startWidth = lineRenderer.endWidth = 1.5f;
    }

    public void Init(GameObject startPoint, Material material, bool spawn)
    {
        pointA = startPoint;
        isSpawn = spawn;
        lineRenderer.material = material;
        lineRenderer.SetPosition(0, pointA.transform.position);
    }

    public bool FinishLine(GameObject startPoint, GameObject endPoint)
    {
        pointA = startPoint;
        pointB = endPoint;
        if (pointA == pointB) return false;
        

        if (isSpawn)
        {
            var spawner = pointA.GetComponentInParent<MobSpawner>();
            if (spawner != null)
            {
                spawner.Init(); // Убедимся, что спавнер инициализирован
                var isAdded = spawner.AddTarget(pointA, pointB);
                if (!isAdded) return false;
                spawner.StartSpawning();
            }
        }
        else
        {
            var tower = pointA.GetComponentInParent<ShootingTower>();
            Vector3 position = pointB.transform.position - pointA.transform.position;
            if (tower != null)
            {
                if (position.magnitude > tower.MaxDistance)
                {
                    Debug.Log("Road: " + position.magnitude);
                    Debug.Log("Tower: " + tower.MaxDistance);
                    return false;
                }
                tower.Init(); // Убедимся, что спавнер инициализирован
                var isAdded = tower.AddTarget(pointA, pointB);
                if (!isAdded) return false;
                tower.StartShooting();
            }
        }
        

        lineRenderer.SetPosition(0, pointA.transform.position);
        lineRenderer.SetPosition(1, pointB.transform.position);
        lineRenderer.enabled = false;
        OnLineCreated?.Invoke();
        return true;
    }

    public void OnDestroy()
    {
        Debug.Log("OnDestroy");
        if (isSpawn)
        {
            Debug.Log("OnDestroySpawn");
            var spawner = pointA.GetComponentInParent<MobSpawner>();
            if (spawner != null)
            {
                spawner.RemoveTarget(pointA, pointB);
            }
        }
        else
        {
            Debug.Log("OnDestroyTower");
            var tower = pointA.GetComponentInParent<ShootingTower>();
            if (tower != null)
            {
                tower.RemoveTarget(pointA, pointB);
            }
        }
    }
}
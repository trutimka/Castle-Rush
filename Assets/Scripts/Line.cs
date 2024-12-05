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
    
    private void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.positionCount = 2;
        lineRenderer.startWidth = lineRenderer.endWidth = 1.5f;
    }

    public void Init(GameObject startPoint, Material material)
    {
        pointA = startPoint;
        lineRenderer.material = material;
        lineRenderer.SetPosition(0, pointA.transform.position);
    }

    public bool FinishLine(GameObject endPoint)
    {
        var closestPoints = FindClosestExits(pointA, endPoint); // Находим ближайшие выходы
        pointA = closestPoints.Item1; // Перезаписываем pointA
        pointB = closestPoints.Item2; // Перезаписываем pointB

        if (pointA == pointB) return false;
        
        var spawner = pointA.GetComponentInParent<MobSpawner>();
        if (spawner != null)
        {
            spawner.Init(); // Убедимся, что спавнер инициализирован
            var isAdded = spawner.AddTarget(pointB);
            if (!isAdded) return false;
            spawner.StartSpawning();
        }

        lineRenderer.SetPosition(0, pointA.transform.position);
        lineRenderer.SetPosition(1, pointB.transform.position);
        lineRenderer.enabled = false;
        OnLineCreated?.Invoke();
        return true;
    }

    public void OnDestroy()
    {
        var spawner = pointA.GetComponentInParent<MobSpawner>();
        if (spawner != null)
        {
            spawner.RemoveTarget(pointB);
        }
    }
    

    private (GameObject, GameObject) FindClosestExits(GameObject buildingA, GameObject buildingB)
    {
        if (buildingA == null || buildingB == null) return (null, null);

        // Получаем все выходы для обоих зданий
        var exitsA = buildingA.GetComponentsInChildren<Transform>()
            .Where(t => t != buildingA.transform) // Исключаем сам объект здания
            .Select(t => t.gameObject)
            .ToList();

        var exitsB = buildingB.GetComponentsInChildren<Transform>()
            .Where(t => t != buildingB.transform)
            .Select(t => t.gameObject)
            .ToList();

        if (exitsA.Count == 0 || exitsB.Count == 0) return (buildingA, buildingB); // Если нет выходов, соединяем здания

        GameObject closestA = null;
        GameObject closestB = null;
        float minDistance = float.MaxValue;

        // Перебираем все пары выходов между двумя зданиями
        foreach (var exitA in exitsA)
        {
            foreach (var exitB in exitsB)
            {
                float distance = Vector3.Distance(exitA.transform.position, exitB.transform.position);
                if (distance < minDistance)
                {
                    minDistance = distance;
                    closestA = exitA;
                    closestB = exitB;
                }
            }
        }

        return (closestA, closestB); // Возвращаем пару ближайших выходов
    }
}
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

    public bool FinishLine(GameObject startPoint, GameObject endPoint)
    {
        pointA = startPoint;
        pointB = endPoint;
        if (pointA == pointB) return false;
        
        var spawner = pointA.GetComponentInParent<MobSpawner>();
        if (spawner != null)
        {
            spawner.Init(); // Убедимся, что спавнер инициализирован
            var isAdded = spawner.AddTarget(pointA, pointB);
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
            spawner.RemoveTarget(pointA, pointB);
        }
    }
}
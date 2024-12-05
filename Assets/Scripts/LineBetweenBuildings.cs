using Unity.VisualScripting;
using UnityEngine;

public class LineBetweenBuildings : MonoBehaviour
{
    [SerializeField] private GameObject pointA;
    [SerializeField] private GameObject pointB;
    [SerializeField] private LayerMask buildingMask;
    [SerializeField] private LayerMask spawnMask;
    private LineRenderer lineRenderer;

    private void Awake()
    {
        // Получаем компонент LineRenderer
        lineRenderer = GetComponent<LineRenderer>();
    }

    private void Start()
    {
        // Настраиваем линию
        lineRenderer.positionCount = 2; // Линия состоит из двух точек
    }

    private void Update()
    {
        lineRenderer.startWidth = lineRenderer.endWidth = 1.5f;
        // Обновляем позиции линии
        // lineRenderer.SetPosition(0, pointA.transform.position); // Начальная точка
        // lineRenderer.SetPosition(1, pointB.transform.position); // Конечная точка

        if (Input.GetMouseButtonDown(0))
        {
            
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 10000, spawnMask))
            {
                pointA = hit.transform.parent == null ? hit.transform.gameObject : hit.transform.parent.gameObject;
                lineRenderer.SetPosition(0, pointA.transform.position);
                Debug.Log(hit.transform.name);
                Debug.Log("hit");
            }
            
        }

        if (Input.GetMouseButtonUp(0))
        {
            if (pointA == null) return;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 10000, buildingMask))
            {
                Vector3 direction = hit.point - pointA.transform.position;
                float distance = direction.magnitude;
                
                var hits = Physics.RaycastAll(pointA.transform.position, direction.normalized, distance, buildingMask);
                var obj1 = hit.transform.parent == null ? hit.transform.gameObject : hit.transform.parent.gameObject;
                foreach (var rayHit in hits)
                {
                   var obj = rayHit.transform.parent == null ? rayHit.transform.gameObject : rayHit.transform.parent.gameObject;
                   if (obj != pointA && obj != obj1)
                   {
                       lineRenderer.SetPosition(1, pointA.transform.position);
                       pointA = null;
                       return;
                   }
                }
                pointB = obj1;
                lineRenderer.SetPosition(1, pointB.transform.position);
            } 
            else
            {
                lineRenderer.SetPosition(1, pointA.transform.position);
            }
            pointA = null;
        }

        if (Input.GetMouseButton(0))
        {
            if (pointA == null) return;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 10000))
            {
                lineRenderer.SetPosition(1, hit.point);
                Debug.Log(hit.transform.name);
                Debug.Log("hit");
            }
            
        }
    }
    
}
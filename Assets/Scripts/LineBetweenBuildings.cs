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
                pointA = hit.collider.gameObject;
                lineRenderer.SetPosition(0, pointA.transform.position);
                Debug.Log(hit.transform.name);
                Debug.Log("hit");
            }
            
        }

        if (Input.GetMouseButtonUp(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 10000, buildingMask))
            {
                Vector3 direction = hit.point - pointA.transform.position;
                float distance = direction.magnitude;

                // Выполняем Raycast
                if (Physics.Raycast(pointA.transform.position, direction.normalized, out RaycastHit hit2, distance, buildingMask))
                {
                    Debug.Log($"На пути найдено здание: {hit.collider.gameObject.name}");
                    if (hit2.collider.gameObject == hit.collider.gameObject 
                        || (hit.transform.parent != null && hit.transform.parent.gameObject == hit2.collider.gameObject) 
                        || (hit2.transform.parent != null && hit2.transform.parent.gameObject == hit.collider.gameObject)
                        || (hit.transform.parent != null && hit2.transform.parent != null && hit.transform.parent.parent.gameObject == hit2.transform.parent.gameObject))
                    {
                        pointB = hit.collider.gameObject;
                        lineRenderer.SetPosition(1, pointB.transform.position);
                        Debug.Log(hit.transform.name);
                        Debug.Log("hit");
                    }
                    else
                    {
                        lineRenderer.SetPosition(1, pointA.transform.position);
                    }
                }
                else
                {
                    lineRenderer.SetPosition(1, pointA.transform.position);
                }
            } 
            else
            {
                lineRenderer.SetPosition(1, pointA.transform.position);
            }
            
        }

        if (Input.GetMouseButton(0))
        {
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
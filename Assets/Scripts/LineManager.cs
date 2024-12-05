using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

public class LineManager : MonoBehaviour
{
    [SerializeField] private LayerMask buildingMask;
    [SerializeField] private LayerMask spawnMask;
    [SerializeField] private Line prefabLine;
    private List<Line> lines = new List<Line>();
    private Line currentLine;

    private void Update()
    {
        
        if (Input.GetMouseButtonDown(0))
        {
            if (currentLine != null)
            {
                lines.Remove(currentLine);
                Destroy(currentLine);
                currentLine = null;
            }
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 10000, spawnMask))
            {
                var pointA = hit.transform.parent == null ? hit.transform.gameObject : hit.transform.parent.gameObject;
                currentLine = Instantiate(prefabLine, Vector3.zero, Quaternion.identity, transform);
                currentLine.Init(pointA);
                
                Debug.Log(hit.transform.name);
                Debug.Log("hit");
            }
            
        }
        
        if (Input.GetMouseButtonUp(0))
        {
            if (currentLine == null) return;

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit, 10000, buildingMask))
            {
                Vector3 direction = hit.point - currentLine.GetPointA().transform.position;
                float distance = direction.magnitude;
                
                var hits = Physics.RaycastAll(currentLine.GetPointA().transform.position, direction.normalized, distance, buildingMask);
                var obj1 = hit.transform.parent == null ? hit.transform.gameObject : hit.transform.parent.gameObject;
                foreach (var rayHit in hits)
                {
                    var obj = rayHit.transform.parent == null ? rayHit.transform.gameObject : rayHit.transform.parent.gameObject;
                    if (obj != currentLine.GetPointA() && obj != obj1)
                    {
                        Destroy(currentLine.gameObject);
                        currentLine = null;
                        return;
                    }
                }
                var pointB = obj1;
                var isFinished = currentLine.FinishLine(pointB);
                if (!isFinished)
                {
                    Destroy(currentLine.gameObject);
                    currentLine = null;
                    return;
                }
                lines.Add(currentLine); // Добавляем в список
                Debug.Log($"Линия завершена: {currentLine.GetPointA().name} -> {currentLine.GetPointB().name}");

                currentLine = null; // Сбрасываем текущую линию
            }
            else
            {
                // Если конечная точка не найдена, уничтожаем текущую линию
                Destroy(currentLine.gameObject);
                currentLine = null;
            }
        }

        if (Input.GetMouseButton(0) && currentLine != null)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit, 10000))
            {
                currentLine.GetComponent<LineRenderer>()
                    .SetPosition(1, hit.point);
            }
        }
    }
}

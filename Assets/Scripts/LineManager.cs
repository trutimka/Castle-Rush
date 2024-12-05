using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;
using Object = UnityEngine.Object;
[RequireComponent(typeof(Player))]
public class LineManager : MonoBehaviour
{
    [SerializeField] private LayerMask buildingMask;
    [SerializeField] private LayerMask spawnMask;
    [SerializeField] private Line prefabLine;
    [SerializeField] private GameObject cubePrefab;
    [SerializeField] private Player player;
    private List<Line> lines = new List<Line>();
    private Line currentLine;
        //todo: добавить проверку пользователя, так как этот инстанс будет общим и его надо будет синхронить, также надо каждой линии добавить пользователя, который ей владеет 
        //upd: А может и не надо, можно будет просто сделать для каждого пользователя по менеджеру.

    private void Start()
    {
        player = GetComponent<Player>();
    }
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
            Ray ray = player.PlayerCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 10000, spawnMask))
            {
                var pointA = hit.transform.parent == null ? hit.transform.gameObject : hit.transform.parent.gameObject;
                if (pointA.TryGetComponent<LineConnectionPoint>(out var line))
                {
                    if (!line.Owner == player) return;
                }
                currentLine = Instantiate(prefabLine, Vector3.zero, Quaternion.identity, transform);
                currentLine.Init(pointA, player.PlayerMaterial);
                
                Debug.Log(hit.transform.name);
                Debug.Log("hit");
            }
            
        }
        
        if (Input.GetMouseButtonUp(0))
        {
            if (currentLine == null) return;

            Ray ray = player.PlayerCamera.ScreenPointToRay(Input.mousePosition);
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
                var closestPoints = FindClosestExits(currentLine.GetPointA(), obj1);
                var pointA = closestPoints.Item1; // Перезаписываем pointA
                var pointB = closestPoints.Item2;
                
                if (pointB.TryGetComponent<LineConnectionPoint>(out var line))
                {
                    //line.SetOwner(currentLine.GetPointA().GetComponent<LineConnectionPoint>().Owner);
                    if (line.Owner == player || line.Owner == null)
                    {
                        var isFinished = currentLine.FinishLine(pointB);
                        if (!isFinished)
                        {
                            Destroy(currentLine.gameObject);
                            currentLine = null;
                            return;
                        }
                        lines.Add(currentLine); // Добавляем в список
                        
                        var gameObjectCube = Instantiate(cubePrefab, Vector3.zero, Quaternion.identity);

                        Vector3 position = pointB.transform.position - currentLine.GetPointA().transform.position;
                        gameObjectCube.transform.position = currentLine.GetPointA().transform.position + position / 2;

                        Vector3 scale = new Vector3(position.magnitude, 0.1f, 1f);
                        gameObjectCube.transform.localScale = scale;

                        float angle = Mathf.Atan2(position.z, position.x) * Mathf.Rad2Deg;
                        gameObjectCube.transform.rotation = Quaternion.Euler(0f, -angle, 0f);

                        gameObjectCube.GetComponent<MeshRenderer>().material = player.PlayerMaterial;

                        Debug.Log($"Линия завершена: {currentLine.GetPointA().name} -> {currentLine.GetPointB().name}");

                        currentLine = null; // Сбрасываем текущую линию
                    }
                    else
                    {
                        Debug.Log("polniy pizdec");
                        var isFinished = currentLine.FinishLine(pointB);
                        if (!isFinished)
                        {
                            Destroy(currentLine.gameObject);
                            currentLine = null;
                            return;
                        }
                        lines.Add(currentLine); // Добавляем в список
                        
                        var gameObjectCube1 = Instantiate(cubePrefab, Vector3.zero, Quaternion.identity);
                        var gameObjectCube2 = Instantiate(cubePrefab, Vector3.zero, Quaternion.identity);

                        Vector3 position = pointB.transform.position - currentLine.GetPointA().transform.position;
                        Vector3 scale = new Vector3(position.magnitude, 0.1f, 1f);
                        float angle = Mathf.Atan2(position.z, position.x) * Mathf.Rad2Deg;
                        gameObjectCube1.transform.position = currentLine.GetPointA().transform.position + position / 4;
                        gameObjectCube1.transform.localScale = scale / 2;
                        gameObjectCube1.transform.rotation = Quaternion.Euler(0f, -angle, 0f);
                        gameObjectCube1.GetComponent<MeshRenderer>().material = pointA.GetComponent<LineConnectionPoint>().Owner.PlayerMaterial;
                        
                        gameObjectCube2.transform.position = currentLine.GetPointA().transform.position + position * 3 / 4;
                        gameObjectCube2.transform.localScale = scale / 2;
                        gameObjectCube2.transform.rotation = Quaternion.Euler(0f, -angle, 0f);
                        gameObjectCube2.GetComponent<MeshRenderer>().material = pointB.GetComponent<LineConnectionPoint>().Owner.PlayerMaterial;

                        Debug.Log($"Линия завершена: {currentLine.GetPointA().name} -> {currentLine.GetPointB().name}");

                        currentLine = null; // Сбрасываем текущую линию
                    }
                }
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
            Ray ray = player.PlayerCamera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit, 10000))
            {
                currentLine.GetComponent<LineRenderer>()
                    .SetPosition(1, hit.point);
            }
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

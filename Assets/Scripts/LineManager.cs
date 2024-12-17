using System;
using System.Collections.Generic;
using System.Linq;
using Photon.Pun;
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
    
    private Dictionary<Line, GameObject> roads = new Dictionary<Line, GameObject>();
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
                if (pointA.TryGetComponent<Building>(out var building))
                {
                    if (building.Owner != player || player == null) return;
                    
                    currentLine = Instantiate(prefabLine, Vector3.zero, Quaternion.identity, transform);
                    currentLine.Init(pointA, player.PlayerMaterial);
                
                    /*Debug.Log(hit.transform.name);
                    Debug.Log("hit");*/
                }
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
                var pointA = closestPoints.Item1;
                var pointB = closestPoints.Item2;
                var startBuilding = pointA.transform.parent == null ? pointA.transform.gameObject : pointA.transform.parent.gameObject;
                var targetBuilding = pointB.transform.parent == null ? pointB.transform.gameObject : pointB.transform.parent.gameObject;
                Debug.Log(startBuilding + " - " + targetBuilding);
                Debug.Log(pointA + " -> " + pointB);
                
                var startBuildingComponent = startBuilding.GetComponent<Building>();
                var targetBuildingComponent = targetBuilding.GetComponent<Building>();
                
                if (targetBuildingComponent.Owner == player || targetBuildingComponent.Owner == null)
                {
                    var isFinished = currentLine.FinishLine(pointA, pointB);
                    if (!isFinished)
                    {
                        Destroy(currentLine.gameObject);
                        currentLine = null;
                        return;
                    }
                    var spawner = pointA.GetComponentInParent<MobSpawner>();
                    spawner.OnRemoveTarget += RemoveLine;
                    lines.Add(currentLine); // Добавляем в список
                    
                    var gameObjectCube = PhotonNetwork.Instantiate("Prefabs/Common/" + cubePrefab.name, Vector3.zero, Quaternion.identity);

                    Vector3 position = pointB.transform.position - pointA.transform.position;
                    Vector3 scale = new Vector3(position.magnitude, 0.1f, 1f);
                    float angle = Mathf.Atan2(position.z, position.x) * Mathf.Rad2Deg;
                    
                    gameObjectCube.transform.position = pointA.transform.position + position / 2;
                    gameObjectCube.transform.localScale = scale;
                    gameObjectCube.transform.rotation = Quaternion.Euler(0f, -angle, 0f);
                    gameObjectCube.GetComponent<MeshRenderer>().material = player.PlayerMaterial;
                    roads[currentLine] = gameObjectCube;

                    Debug.Log($"Линия завершена: {currentLine.GetPointA().name} -> {currentLine.GetPointB().name}");

                    currentLine = null; // Сбрасываем текущую линию
                }
                else
                {
                    bool isExist = false;
                    var anotherLineManager = targetBuildingComponent.Owner.PlayerCamera.GetComponent<LineManager>();
                    Line isExistLine = null;
                    foreach (var player2Line in anotherLineManager.lines)
                    {
                        if (player2Line.GetPointA() == pointB && player2Line.GetPointB() == pointA)
                        {
                            isExist = true;
                            isExistLine = player2Line;
                            Destroy(anotherLineManager.roads[player2Line]);
                            break;
                        }
                    }
                    
                    var isFinished = currentLine.FinishLine(pointA, pointB);
                    if (!isFinished)
                    {
                        Destroy(currentLine.gameObject);
                        currentLine = null;
                        return;
                    }
                    var spawner = pointA.GetComponentInParent<MobSpawner>();
                    spawner.OnRemoveTarget += RemoveLine;
                    lines.Add(currentLine); // Добавляем в список

                    if (isExist)
                    {
                        var gameObjectCube1 = PhotonNetwork.Instantiate("Prefabs/Common/" + cubePrefab.name, Vector3.zero, Quaternion.identity);
                        var gameObjectCube2 = PhotonNetwork.Instantiate("Prefabs/Common/" + cubePrefab.name, Vector3.zero, Quaternion.identity);

                        Vector3 position = pointB.transform.position - pointA.transform.position;
                        Vector3 scale = new Vector3(position.magnitude, 0.1f, 1f);
                        float angle = Mathf.Atan2(position.z, position.x) * Mathf.Rad2Deg;
                        gameObjectCube1.transform.position = pointA.transform.position + position / 4;
                        gameObjectCube1.transform.localScale = scale / 2;
                        gameObjectCube1.transform.rotation = Quaternion.Euler(0f, -angle, 0f);
                        gameObjectCube1.GetComponent<MeshRenderer>().material = startBuildingComponent.Owner.PlayerMaterial;
                    
                        gameObjectCube2.transform.position = pointA.transform.position + position * 3 / 4;
                        gameObjectCube2.transform.localScale = scale / 2;
                        gameObjectCube2.transform.rotation = Quaternion.Euler(0f, -angle, 0f);
                        gameObjectCube2.GetComponent<MeshRenderer>().material = targetBuildingComponent.Owner.PlayerMaterial;
                        anotherLineManager.roads[isExistLine] = gameObjectCube2;
                        roads[currentLine] = gameObjectCube1;
                    }
                    else
                    {
                        var gameObjectCube = PhotonNetwork.Instantiate("Prefabs/Common/" + cubePrefab.name, Vector3.zero, Quaternion.identity);
                        Vector3 position = pointB.transform.position - pointA.transform.position;
                        Vector3 scale = new Vector3(position.magnitude, 0.1f, 1f);
                        float angle = Mathf.Atan2(position.z, position.x) * Mathf.Rad2Deg;

                        gameObjectCube.transform.position = pointA.transform.position + position / 2;
                        gameObjectCube.transform.localScale = scale;
                        gameObjectCube.transform.rotation = Quaternion.Euler(0f, -angle, 0f);
                        gameObjectCube.GetComponent<MeshRenderer>().material = player.PlayerMaterial;
                        roads[currentLine] = gameObjectCube;
                    }

                    Debug.Log($"Линия завершена: {currentLine.GetPointA().name} -> {currentLine.GetPointB().name}");

                    currentLine = null; // Сбрасываем текущую линию
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

    private void RemoveLine(GameObject building, GameObject endLine)
    {
        var line = lines.Find(t => t.GetPointA() == building && t.GetPointB() == endLine);
        if (line == null) return;
        Destroy(roads[line]);
        roads.Remove(line);
        Destroy(line);
        lines.Remove(line);
    }
}

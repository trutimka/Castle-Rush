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
    [SerializeField] private LayerMask shoutingMask;
    [SerializeField] private LayerMask roadMask; // Добавим отдельный слой для дорог (кубов)
    [SerializeField] private Line prefabLine;
    [SerializeField] private GameObject cubePrefab;
    [SerializeField] private Player player;

    private Dictionary<Line, GameObject> roads = new Dictionary<Line, GameObject>();
    private List<Line> lines = new List<Line>();
    private Line currentLine;

    // Переменные для удаления дорог
    private bool isPressingRoad = false;       // Нажата ли сейчас дорога
    private float pressTime = 0f;              // Время удержания
    private float requiredHoldTime = 1.5f;     // Время, которое нужно удерживать для удаления
    private Line roadToDelete = null;          // Линия, которую потенциально удаляем

    private void Start()
    {
        player = GetComponent<Player>();
    }
    private void Update()
    {
        // Обработка создания дорог
        HandleRoadCreation();

        // Обработка удаления дорог по удержанию
        HandleRoadDeletion();
    }

    private void HandleRoadCreation()
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
            
            // Проверка нажатия по дороге для начала удаления
            if (Physics.Raycast(ray, out hit, 10000, roadMask))
            {
                // Мы нажали на дорогу, активируем режим удержания для удаления
                var hitGameObject = hit.transform.gameObject;
                var roadEntry = roads.FirstOrDefault(r => r.Value == hitGameObject);
                if (roadEntry.Key != null)
                {
                    isPressingRoad = true;
                    pressTime = 0f;
                    roadToDelete = roadEntry.Key;
                    return; // Выходим, чтобы не создавать новых линий при нажатии на дорогу
                }
            }

            // Если нажали не на дорогу, пытаемся начать рисовать новую линию
            if (Physics.Raycast(ray, out hit, 10000, spawnMask))
            {
                var pointA = hit.transform.parent == null ? hit.transform.gameObject : hit.transform.parent.gameObject;
                if (pointA.TryGetComponent<Building>(out var building))
                {
                    if (building.Owner != player || player == null) return;
                    
                    currentLine = Instantiate(prefabLine, Vector3.zero, Quaternion.identity, transform);
                    currentLine.Init(pointA, player.PlayerMaterial, true);
                }
            }
            if (Physics.Raycast(ray, out hit, 10000, shoutingMask))
            {
                var pointA = hit.transform.parent == null ? hit.transform.gameObject : hit.transform.parent.gameObject;
                if (pointA.TryGetComponent<Building>(out var building))
                {
                    if (building.Owner != player || player == null) return;
                    
                    currentLine = Instantiate(prefabLine, Vector3.zero, Quaternion.identity, transform);
                    currentLine.Init(pointA, player.PlayerMaterial, false);
                }
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            if (isPressingRoad) 
            {
                // Если мышь отпустили до достижения 1.5 секунд, отменяем удаление
                isPressingRoad = false;
                roadToDelete = null;
            }

            if (currentLine == null) return;

            Ray ray = player.PlayerCamera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit, 10000, buildingMask))
            {
                Vector3 direction = hit.point - currentLine.GetPointA().transform.position;
                float distance = direction.magnitude;
                Debug.Log("Current road distance: " + distance);
                
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
                
                if (currentLine.IsSpawn)
                {
                    CreateLineFromSpawner(obj1);
                }
                else
                {
                    CreateLineFromShoutingTower(obj1);
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

    private void HandleRoadDeletion()
    {
        if (isPressingRoad && roadToDelete != null)
        {
            if (Input.GetMouseButton(0))
            {
                pressTime += Time.deltaTime;
                if (pressTime >= requiredHoldTime)
                {
                    // Удаляем дорогу
                    RemoveRoad(roadToDelete);
                    isPressingRoad = false;
                    roadToDelete = null;
                }
            }
            else
            {
                // Если кнопку отпустили до достижения нужного времени
                isPressingRoad = false;
                roadToDelete = null;
            }
        }
    }

    private void RemoveRoad(Line line)
    {
        // Аналогично методу RemoveLine, но с конкретной линией
        if (line == null) return;
        
        var pointA = line.GetPointA();
        var pointB = line.GetPointB();
        var startBuilding = pointA.transform.parent == null ? pointA.transform.gameObject : pointA.transform.parent.gameObject;
        var targetBuilding = pointB.transform.parent == null ? pointB.transform.gameObject : pointB.transform.parent.gameObject;
        var startBuildingComponent = startBuilding.GetComponent<Building>();
        var targetBuildingComponent = targetBuilding.GetComponent<Building>();

        if (targetBuildingComponent.Owner != null && targetBuildingComponent.Owner != player)
        {
            var anotherLineManager = targetBuildingComponent.Owner.PlayerCamera.GetComponent<LineManager>();
            foreach (var player2Line in anotherLineManager.lines)
            {
                if (player2Line.GetPointA() == pointB && player2Line.GetPointB() == pointA)
                {
                    anotherLineManager.RemakeRoad(player2Line);
                    break;
                }
            }
        }
        
        
        if (roads.ContainsKey(line))
        {
            Destroy(roads[line]);
            roads.Remove(line);
        }
        if (lines.Contains(line))
        {
            lines.Remove(line);
        }
        Destroy(line.gameObject);
        Debug.Log("Дорога удалена!");
    }

    private void CreateLineFromSpawner(GameObject obj1)
    {
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
            
            var gameObjectCube = Instantiate(cubePrefab, Vector3.zero, Quaternion.identity);

            Vector3 position = pointB.transform.position - pointA.transform.position;
            Vector3 scale = new Vector3(position.magnitude, 0.1f, 1f);
            float angle = Mathf.Atan2(position.z, position.x) * Mathf.Rad2Deg;
            
            gameObjectCube.transform.position = pointA.transform.position + position / 2;
            gameObjectCube.transform.localScale = scale;
            gameObjectCube.transform.rotation = Quaternion.Euler(0f, -angle, 0f);
            gameObjectCube.GetComponent<MeshRenderer>().material = player.PlayerMaterial;
            roads[currentLine] = gameObjectCube;

            // Назначаем roadMask для луча (если еще не назначен)
            // Можно через LayerMask.NameToLayer("Road") или настройками.
            gameObjectCube.layer = LayerMask.NameToLayer("Road");
            
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
                var gameObjectCube1 = Instantiate(cubePrefab, Vector3.zero, Quaternion.identity);
                var gameObjectCube2 = Instantiate(cubePrefab, Vector3.zero, Quaternion.identity);

                Vector3 position = pointB.transform.position - pointA.transform.position;
                Vector3 scale = new Vector3(position.magnitude, 0.1f, 1f);
                float angle = Mathf.Atan2(position.z, position.x) * Mathf.Rad2Deg;
                gameObjectCube1.transform.position = pointA.transform.position + position / 4;
                gameObjectCube1.transform.localScale = scale / 2;
                gameObjectCube1.transform.rotation = Quaternion.Euler(0f, -angle, 0f);
                gameObjectCube1.GetComponent<MeshRenderer>().material = startBuildingComponent.Owner.PlayerMaterial;
                gameObjectCube1.layer = LayerMask.NameToLayer("Road");

                gameObjectCube2.transform.position = pointA.transform.position + position * 3 / 4;
                gameObjectCube2.transform.localScale = scale / 2;
                gameObjectCube2.transform.rotation = Quaternion.Euler(0f, -angle, 0f);
                gameObjectCube2.GetComponent<MeshRenderer>().material = targetBuildingComponent.Owner.PlayerMaterial;
                gameObjectCube2.layer = LayerMask.NameToLayer("Road");

                anotherLineManager.roads[isExistLine] = gameObjectCube2;
                roads[currentLine] = gameObjectCube1;
            }
            else
            {
                var gameObjectCube = Instantiate(cubePrefab, Vector3.zero, Quaternion.identity);
                Vector3 position = pointB.transform.position - pointA.transform.position;
                Vector3 scale = new Vector3(position.magnitude, 0.1f, 1f);
                float angle = Mathf.Atan2(position.z, position.x) * Mathf.Rad2Deg;

                gameObjectCube.transform.position = pointA.transform.position + position / 2;
                gameObjectCube.transform.localScale = scale;
                gameObjectCube.transform.rotation = Quaternion.Euler(0f, -angle, 0f);
                gameObjectCube.GetComponent<MeshRenderer>().material = player.PlayerMaterial;
                gameObjectCube.layer = LayerMask.NameToLayer("Road");
                roads[currentLine] = gameObjectCube;
            }

            Debug.Log($"Линия завершена: {currentLine.GetPointA().name} -> {currentLine.GetPointB().name}");
            currentLine = null; // Сбрасываем текущую линию
        }
    }

    private void CreateLineFromShoutingTower(GameObject obj1)
    {
        var closestPoints = FindClosestExits(currentLine.GetPointA(), obj1);
        var pointA = closestPoints.Item1;
        var pointB = closestPoints.Item2;
        var startBuilding = pointA.transform.parent == null ? pointA.transform.gameObject : pointA.transform.parent.gameObject;
        var targetBuilding = pointB.transform.parent == null ? pointB.transform.gameObject : pointB.transform.parent.gameObject;
        pointB = targetBuilding;
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
            var tower = pointA.GetComponentInParent<ShootingTower>();
            tower.OnRemoveTarget += RemoveLine;
            lines.Add(currentLine); // Добавляем в список
            
            var gameObjectCube = Instantiate(cubePrefab, Vector3.zero, Quaternion.identity);

            Vector3 position = (pointB.transform.position - pointA.transform.position);
            Vector3 scale = new Vector3(position.magnitude, 0.1f, 1f);
            float angle = Mathf.Atan2(position.z, position.x) * Mathf.Rad2Deg;
            
            gameObjectCube.transform.position = pointA.transform.position + position / 2;
            gameObjectCube.transform.localScale = scale;
            gameObjectCube.transform.rotation = Quaternion.Euler(0f, -angle, 0f);
            gameObjectCube.GetComponent<MeshRenderer>().material = player.PlayerMaterial;
            gameObjectCube.layer = LayerMask.NameToLayer("Road");
            roads[currentLine] = gameObjectCube;

            Debug.Log($"Линия завершена: {currentLine.GetPointA().name} -> {currentLine.GetPointB().name}");
            currentLine = null; // Сбрасываем текущую линию
        }
        else
        {
            bool isExist = false;
            var anotherLineManager = targetBuildingComponent.Owner.PlayerCamera.GetComponent<LineManager>();
            Line isExistLine = null;
            
            var tower = pointA.GetComponentInParent<ShootingTower>();
            
            foreach (var player2Line in anotherLineManager.lines)
            {
                if (player2Line.GetPointA() == pointB && player2Line.GetPointB() == pointA)
                {
                    if (player2Line.transform.position.magnitude > tower.MaxDistance) continue;
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
            tower.OnRemoveTarget += RemoveLine;
            lines.Add(currentLine); // Добавляем в список

            if (isExist)
            {
                var gameObjectCube1 = Instantiate(cubePrefab, Vector3.zero, Quaternion.identity);
                var gameObjectCube2 = Instantiate(cubePrefab, Vector3.zero, Quaternion.identity);

                Vector3 position = (pointB.transform.position - pointA.transform.position);
                Vector3 scale = new Vector3(position.magnitude, 0.1f, 1f);
                float angle = Mathf.Atan2(position.z, position.x) * Mathf.Rad2Deg;

                gameObjectCube1.transform.position = pointA.transform.position + position / 4;
                gameObjectCube1.transform.localScale = scale / 2;
                gameObjectCube1.transform.rotation = Quaternion.Euler(0f, -angle, 0f);
                gameObjectCube1.GetComponent<MeshRenderer>().material = startBuildingComponent.Owner.PlayerMaterial;
                gameObjectCube1.layer = LayerMask.NameToLayer("Road");
            
                gameObjectCube2.transform.position = pointA.transform.position + position * 3 / 4;
                gameObjectCube2.transform.localScale = scale / 2;
                gameObjectCube2.transform.rotation = Quaternion.Euler(0f, -angle, 0f);
                gameObjectCube2.GetComponent<MeshRenderer>().material = targetBuildingComponent.Owner.PlayerMaterial;
                gameObjectCube2.layer = LayerMask.NameToLayer("Road");

                anotherLineManager.roads[isExistLine] = gameObjectCube2;
                roads[currentLine] = gameObjectCube1;
            }
            else
            {
                var gameObjectCube = Instantiate(cubePrefab, Vector3.zero, Quaternion.identity);
                Vector3 position = (pointB.transform.position - pointA.transform.position);
                Vector3 scale = new Vector3(position.magnitude, 0.1f, 1f);
                float angle = Mathf.Atan2(position.z, position.x) * Mathf.Rad2Deg;

                gameObjectCube.transform.position = pointA.transform.position + position / 2;
                gameObjectCube.transform.localScale = scale;
                gameObjectCube.transform.rotation = Quaternion.Euler(0f, -angle, 0f);
                gameObjectCube.GetComponent<MeshRenderer>().material = player.PlayerMaterial;
                gameObjectCube.layer = LayerMask.NameToLayer("Road");
                roads[currentLine] = gameObjectCube;
            }

            Debug.Log($"Линия завершена: {currentLine.GetPointA().name} -> {currentLine.GetPointB().name}");
            currentLine = null; // Сбрасываем текущую линию
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
        if (roads.ContainsKey(line))
        {
            Destroy(roads[line]);
            roads.Remove(line);
        }
        Destroy(line.gameObject);
        lines.Remove(line);
    }

    public void RemakeRoad(Line line)
    {
        Debug.Log("deleting road");
        Destroy(roads[line]);
        
        var gameObjectCube = Instantiate(cubePrefab, Vector3.zero, Quaternion.identity);
        Vector3 position = line.GetPointB().transform.position - line.GetPointA().transform.position;
        Vector3 scale = new Vector3(position.magnitude, 0.1f, 1f);
        float angle = Mathf.Atan2(position.z, position.x) * Mathf.Rad2Deg;

        gameObjectCube.transform.position = line.GetPointA().transform.position + position / 2;
        gameObjectCube.transform.localScale = scale;
        gameObjectCube.transform.rotation = Quaternion.Euler(0f, -angle, 0f);
        gameObjectCube.GetComponent<MeshRenderer>().material = player.PlayerMaterial;
        gameObjectCube.layer = LayerMask.NameToLayer("Road");
        roads[line] = gameObjectCube;
    }
}
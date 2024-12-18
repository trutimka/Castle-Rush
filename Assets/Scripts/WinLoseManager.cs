using UnityEngine;
using System.Linq;

public class WinLoseManager : MonoBehaviour
{
    [SerializeField] private Camera player1Camera;
    [SerializeField] private Camera player2Camera;
    [SerializeField] private Player player1; 
    [SerializeField] private Player player2;

    public void CheckWinLoseConditions()
    {
        // Находим все здания на сцене
        MobSpawner[] allBuildings = FindObjectsOfType<MobSpawner>();

        bool player1HasSpawn = false;
        bool player2HasSpawn = false;

        // Проверяем здания
        foreach (var building in allBuildings)
        {
            var owner = building.Owner;
            if (owner == null) continue;
            
            if (owner.PlayerNumber == player1.PlayerNumber)
                player1HasSpawn = true;
            else if (owner.PlayerNumber == player2.PlayerNumber)
                player2HasSpawn = true;
        }
        //todo: добавить вывод
        // Логика для двух игроков:
        if (player1HasSpawn && !player2HasSpawn)
        {
            Debug.Log($"Игрок {player1.PlayerNumber} победил! Игрок {player2.PlayerNumber} проиграл.");
            // Вызываем логику победы для player1
        }
        else if (!player1HasSpawn && player2HasSpawn)
        {
            Debug.Log($"Игрок {player2.PlayerNumber} победил! Игрок {player1.PlayerNumber} проиграл.");
            // Вызываем логику победы для player2
        }
        else if (!player1HasSpawn && !player2HasSpawn)
        {
            Debug.Log("Оба игрока потеряли спаун-здания. Ничья или особая логика.");
            // Обработка ничьей или другой ситуации
        }
    }

    private void Start()
    {
        player1 = player1Camera.GetComponent<Player>();
        player2 = player2Camera.GetComponent<Player>();
    }

    private void Update()
    {
        CheckWinLoseConditions();
    }
}
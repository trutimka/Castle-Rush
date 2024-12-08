
using System;
using UnityEngine;
using UnityEngine.Serialization;

public class Player : MonoBehaviour
{
    protected float boost = 1.0f;
    [SerializeField]
    private double boostMultiplier = 1.01;
    [SerializeField]
    private float goldCount = 100;
    
    public event Action<float> OnGoldChanged;

    [SerializeField] private GameObject _mobPrefab;
    public GameObject MobPrefab => _mobPrefab;
    
    [SerializeField]
    private Camera playerCamera;
    public Camera PlayerCamera => playerCamera;
    
    [SerializeField]
    private Material playerMaterial;
    
    [SerializeField]
    private Color playerColor;
    
    public Material PlayerMaterial => playerMaterial;
    public Color PlayerColor => playerColor;
    
    public float Boost => boost;
    public float GoldCount => goldCount;
    public bool SpendGold(int amount)
    {
        if (amount < 0 || goldCount < amount) return false;
        goldCount -= amount;
        return true;
    }

    public bool AddGold(float amount)
    {
        if (amount < 0) return false;
        goldCount += amount;
        OnGoldChanged?.Invoke(goldCount);
        return true;
    }
    
    [SerializeField]
    private float slowdownInterval = 2f; // Интервал между замедлениями (в секундах)
    private float slowdownFactor = 0.9f; // Коэффициент замедления

    private float timeSinceLastSlowdown = 0f;
    
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            boost += 0.15f;
        }
        
        timeSinceLastSlowdown += Time.unscaledDeltaTime;

        if (timeSinceLastSlowdown >= slowdownInterval)
        {
            boost *= slowdownFactor;
            timeSinceLastSlowdown = 0f;
        }
    }
   
}

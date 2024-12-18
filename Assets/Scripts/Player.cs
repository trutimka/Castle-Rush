
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
    
    [SerializeField] private GameObject _bimbaPrefab;
    public GameObject BimbaPrefab => _bimbaPrefab;
    [SerializeField]
    private Camera playerCamera;
    public Camera PlayerCamera => playerCamera;
    
    [SerializeField]
    private Material playerMaterial;
    
    [SerializeField]
    private Color playerColor;
    
    [SerializeField] public int PlayerNumber;
    
    public Material PlayerMaterial => playerMaterial;
    public Color PlayerColor => playerColor;

    private int boostMobDamage = 0;
    private int boostMobSpeed = 0;
    private int boostMobHealth = 0;
    private int boostBimbaDamage = 0;
    private int boostBimbaSpeed = 0;
    private float boostGoldGeneration = 0;
    
    public int BoostMobDamage => boostMobDamage;
    public int BoostMobSpeed => boostMobSpeed;
    public int BoostMobHealth => boostMobHealth;
    public int BoostBimbaDamage => boostBimbaDamage;
    public int BoostBimbaSpeed => boostBimbaSpeed;
    public float BoostGoldGeneration => boostGoldGeneration;
    
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

        // if (Input.GetKey(KeyCode.Q))
        // {
        //     UpgradeBimbaSpeed();
        // }
        // if (Input.GetKey(KeyCode.W))
        // {
        //     UpgradeBimbaDamage();
        // }
        // if (Input.GetKey(KeyCode.E))
        // {
        //     UpgradeMobSpeed();
        // }
        // if (Input.GetKey(KeyCode.R))
        // {
        //     UpgradeMobHealth();
        // }
        // if (Input.GetKey(KeyCode.T))
        // {
        //     UpgradeMobDamage();
        // }
        //
        // if (Input.GetKey(KeyCode.Y))
        // {
        //     UpgradeGoldGeneration();
        // }
    }

    public void UpgradeMobDamage()
    {
        boostMobDamage++;
    }

    public void UpgradeMobSpeed()
    {
        boostMobSpeed++;
    }

    public void UpgradeBimbaDamage()
    {
        boostBimbaDamage++;
    }

    public void UpgradeBimbaSpeed()
    {
        boostBimbaSpeed++;
    }

    public void UpgradeMobHealth()
    {
        boostMobHealth++;
    }

    public void UpgradeGoldGeneration()
    {
        boostGoldGeneration += 0.5f;
    }
}

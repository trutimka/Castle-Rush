
using System;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Serialization;

public class Player : MonoBehaviourPunCallbacks, IPunObservable
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
    
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            if (Camera.main.GetComponent<Player>() == this)
            {
                stream.SendNext(boost);
                stream.SendNext(boostMultiplier);
                stream.SendNext(boostMobDamage);
                stream.SendNext(boostMobSpeed);
                stream.SendNext(boostMobHealth);
                stream.SendNext(boostBimbaDamage);
                stream.SendNext(boostBimbaSpeed);
                stream.SendNext(boostGoldGeneration);
            }
        }
        else
        {
            if (Camera.main.GetComponent<Player>() != this)
            {
                boost = (float)stream.ReceiveNext();
                boostMultiplier = (double)stream.ReceiveNext();
                boostMobDamage = (int)stream.ReceiveNext();
                boostMobSpeed = (int)stream.ReceiveNext();
                boostMobHealth = (int)stream.ReceiveNext();
                boostBimbaDamage = (int)stream.ReceiveNext();
                boostBimbaSpeed = (int)stream.ReceiveNext();
                boostGoldGeneration = (float)stream.ReceiveNext();
            }
        }
    }
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

    public void BoostUse(bool pressed)
    {
        if (pressed) boost += 0.15f;
        timeSinceLastSlowdown += Time.unscaledDeltaTime;

        if (timeSinceLastSlowdown >= slowdownInterval)
        {
            boost *= slowdownFactor;
            timeSinceLastSlowdown = 0f;
        }
    }

    public void UpgradeMobDamage()
    {
        if (SpendGold(100*boostMobDamage)) boostMobDamage++;
    }

    public void UpgradeMobSpeed()
    {
        if (SpendGold(100 * boostMobSpeed)) boostMobSpeed++;
    }

    public void UpgradeBimbaDamage()
    {
        if (SpendGold(100 * boostBimbaDamage)) boostBimbaDamage++;
    }

    public void UpgradeBimbaSpeed()
    {
        if (SpendGold(100*boostBimbaSpeed)) boostBimbaSpeed++;
    }

    public void UpgradeMobHealth()
    {
        if (SpendGold(100*boostMobHealth)) boostMobHealth++;
    }

    public void UpgradeGoldGeneration()
    {
        if (SpendGold((int)(100 * boostGoldGeneration))) boostGoldGeneration += 0.5f;
    }
}

using System;
using UnityEngine;
using System.Collections.Generic;
using Photon.Pun;

public abstract class Building : MonoBehaviourPun
{
    protected Player Owner = null;
    protected double CountGoldPerSecond;
    protected int Health;
    protected int MaxHealth;
    protected int Level;
    public event Action OnLevelChanged;
    
    
    [SerializeField]
    protected List<GameObject> spawnPoints;
    
    public List<GameObject> SpawnPoints => spawnPoints;

    public void Init(int startHealth, int maxHealth, double countGoldPerSecond = 1)
    {
        Health = startHealth;
        MaxHealth = maxHealth;
        CountGoldPerSecond = countGoldPerSecond;

        switch (Health)
        {
            case <= 20: Level = 1;OnLevelChanged?.Invoke();break;
            case <= 40: Level = 2;OnLevelChanged?.Invoke();break;
            default: Level = 3;OnLevelChanged?.Invoke();break;
        }
    }
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    protected virtual void ChangePlayer(Player player)
    {
        Owner = player;
    }

    protected virtual void BuildingHit(Mob mob)
    {
        Health -= mob.Damage;
        if (Health <= 0)
        {
            ChangePlayer(mob.Owner);
        }
        
        switch (Health)
        {
            case <= 20: Level = 1;OnLevelChanged?.Invoke();break;
            case <= 40: Level = 2;OnLevelChanged?.Invoke();break;
            default: Level = 3;OnLevelChanged?.Invoke();break;
        }
    }

    protected virtual void BuildingHeal(Mob mob)
    {
        Health += mob.Damage;
        if (Health > MaxHealth)
        {
            Health = MaxHealth;
        }
        
        switch (Health)
        {
            case <= 20: Level = 1;OnLevelChanged?.Invoke();break;
            case <= 40: Level = 2;OnLevelChanged?.Invoke();break;
            default: Level = 3;OnLevelChanged?.Invoke();break;
        }
    }
    
    protected virtual double? GenerateGold()
    {
        return Owner?.Boost * CountGoldPerSecond;
    }

    protected virtual void UpdateMaxHealth(int difference)
    {
        MaxHealth += difference;
    }

    protected virtual void UpdateCountGoldPerSecond(double difference)
    {
        CountGoldPerSecond += difference;
    }
}

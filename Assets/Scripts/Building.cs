using UnityEngine;
using System.Collections.Generic;

public abstract class Building : MonoBehaviour
{
    protected Player Owner = null;
    protected double CountGoldPerSecond;
    protected int Health;
    protected int MaxHealth;
    protected int Level;
    
    
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
            case <= 20: Level = 1;break;
            case <= 40: Level = 2;break;
            default: Level = 3;break;
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
            case <= 20: Level = 1;break;
            case <= 40: Level = 2;break;
            default: Level = 3;break;
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
            case <= 20: Level = 1;break;
            case <= 40: Level = 2;break;
            default: Level = 3;break;
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

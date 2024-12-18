using System;
using UnityEngine;
using System.Collections.Generic;
using Photon.Pun;

public abstract class Building : MonoBehaviourPun
{
    [SerializeField]
    protected Player owner = null;
    [SerializeField]
    protected float CountGoldPerSecond;
    [SerializeField]
    protected int Health;
    [SerializeField]
    protected int MaxHealth;
    [SerializeField]
    protected int Level;
    public event Action<int> OnLevelChanged;
    public event Action<int> OnHealthChanged;
    public event Action OnOwnerChanged;
    
    public Player Owner => owner;
    
    
    [SerializeField]
    protected List<GameObject> spawnPoints;
    
    public List<GameObject> SpawnPoints => spawnPoints;

    public void Init(int startHealth, int maxHealth, float countGoldPerSecond = 1)
    {
        Health = startHealth;
        MaxHealth = maxHealth;
        CountGoldPerSecond = countGoldPerSecond;
        
        OnHealthChanged?.Invoke(Health);

        switch (Health)
        {
            case <= 20: Level = 1;OnLevelChanged?.Invoke(Level);break;
            case <= 40: Level = 2;OnLevelChanged?.Invoke(Level);break;
            default: Level = 3;OnLevelChanged?.Invoke(Level);break;
        }
    }

    private void Start()
    {
        OnLevelChanged?.Invoke(Level);
        OnOwnerChanged?.Invoke();
        OnHealthChanged?.Invoke(Health);
    }

    [SerializeField]
    private float generationInterval = 1f;
    private float timeSinceLastGeneration = 0f;
    
    private void Update()
    {
        if (Owner == null) return;
        timeSinceLastGeneration += Time.unscaledDeltaTime;

        if (timeSinceLastGeneration >= generationInterval)
        {
            GenerateGold();
            timeSinceLastGeneration = 0f;
        }
    }

    public virtual void ChangePlayer(Player player)
    {
        owner = player;
        OnOwnerChanged?.Invoke();
    }

    public virtual void BuildingHit(int damage, Player player)
    {
        if (Health <= 0)
        {
            Health = 1;
            ChangePlayer(player);
        }
        Health -= damage;
        if (Health <= 0) Health = 0;
        
        OnHealthChanged?.Invoke(Health);
        
        switch (Health)
        {
            case <= 20: Level = 1;OnLevelChanged?.Invoke(Level);break;
            case <= 40: Level = 2;OnLevelChanged?.Invoke(Level);break;
            default: Level = 3;OnLevelChanged?.Invoke(Level);break;
        }
    }

    public virtual void BuildingHeal(int damage)
    {
        Health += damage;
        if (Health > MaxHealth)
        {
            Health = MaxHealth;
        }
        
        OnHealthChanged?.Invoke(Health);
        
        switch (Health)
        {
            case <= 20: Level = 1;OnLevelChanged?.Invoke(Level);break;
            case <= 40: Level = 2;OnLevelChanged?.Invoke(Level);break;
            default: Level = 3;OnLevelChanged?.Invoke(Level);break;
        }
    }

    public virtual void BuildingHitWithoutOwner(int damage)
    {
        Health -= damage;
        if (Health <= 0) Health = 0;
        
        OnHealthChanged?.Invoke(Health);
        
        switch (Health)
        {
            case <= 20: Level = 1;OnLevelChanged?.Invoke(Level);break;
            case <= 40: Level = 2;OnLevelChanged?.Invoke(Level);break;
            default: Level = 3;OnLevelChanged?.Invoke(Level);break;
        }
    }
    
    protected virtual void GenerateGold()
    {
        Owner.AddGold(Owner.Boost * (CountGoldPerSecond + Owner.BoostGoldGeneration));
    }

    protected virtual void UpdateMaxHealth(int difference)
    {
        MaxHealth += difference;
    }

    protected virtual void UpdateCountGoldPerSecond(float difference)
    {
        CountGoldPerSecond += difference;
    }
}

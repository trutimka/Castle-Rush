using System;
using UnityEngine;
using System.Collections.Generic;
using Photon.Pun;

public abstract class Building : MonoBehaviourPunCallbacks, IPunObservable
{
    [SerializeField]
    protected Player owner = null;
    [SerializeField]
    protected double CountGoldPerSecond;
    [SerializeField]
    protected int Health;
    [SerializeField]
    protected int MaxHealth;
    [SerializeField]
    protected int Level;
    public event Action OnLevelChanged;
    public event Action OnOwnerChanged;
    
    public Player Owner => owner;
    
    
    [SerializeField]
    protected List<GameObject> spawnPoints;
    
    public List<GameObject> SpawnPoints => spawnPoints;
    
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            // We own this player: send the others our data
            //stream.SendNext(owner);
            stream.SendNext(CountGoldPerSecond);
            stream.SendNext(Health);
            stream.SendNext(MaxHealth);
            stream.SendNext(Level);
        }
        else
        {
            // Network player, receive data
            //owner = stream.ReceiveNext() as Player;
            CountGoldPerSecond = (double)stream.ReceiveNext();
            Health = (int)stream.ReceiveNext();
            MaxHealth = (int)stream.ReceiveNext();
            Level = (int)stream.ReceiveNext();
        }
    }

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
        
        switch (Health)
        {
            case <= 20: Level = 1;OnLevelChanged?.Invoke();break;
            case <= 40: Level = 2;OnLevelChanged?.Invoke();break;
            default: Level = 3;OnLevelChanged?.Invoke();break;
        }
    }

    public virtual void BuildingHeal(int damage)
    {
        Health += damage;
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

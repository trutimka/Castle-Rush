using System;
using UnityEngine;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine.Serialization;

public abstract class Building : MonoBehaviourPunCallbacks, IPunObservable, IPunInstantiateMagicCallback
{
    [SerializeField]
    protected Player owner = null;
    [SerializeField]
    protected float CountGoldPerSecond;
    [SerializeField]
    protected int health;
    [SerializeField]
    protected int MaxHealth;
    [SerializeField]
    protected int level;
    public event Action<int> OnLevelChanged;
    public event Action<int> OnHealthChanged;
    public event Action OnOwnerChanged;
    
    public Player Owner => owner;
    
    public int Health => health;
    
    public int Level => level;
    
    [SerializeField]
    protected List<GameObject> spawnPoints;
    
    public List<GameObject> SpawnPoints => spawnPoints;
    
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        var manager = GameObject.FindGameObjectWithTag("CameraManager").GetComponent<CameraManager>();
        if (stream.IsWriting)
        {
            // We own this player: send the others our data
            stream.SendNext(manager.GetPlayerNumber(Owner));
            stream.SendNext(CountGoldPerSecond);
            stream.SendNext(health);
            stream.SendNext(MaxHealth);
            stream.SendNext(level);
        }
        else
        {
            // Network player, receive data
            owner = manager.GetPlayer((int)stream.ReceiveNext());
            CountGoldPerSecond = (float)stream.ReceiveNext();
            health = (int)stream.ReceiveNext();
            MaxHealth = (int)stream.ReceiveNext();
            level = (int)stream.ReceiveNext();
            OnHealthChanged?.Invoke(health);
            OnOwnerChanged?.Invoke();
        }
    }

    public void OnPhotonInstantiate(PhotonMessageInfo info)
    {
        OnHealthChanged?.Invoke(health);
        OnLevelChanged?.Invoke(level);
        OnOwnerChanged?.Invoke();
    }

    public void Init(int startHealth, int maxHealth, float countGoldPerSecond = 1)
    {
        health = startHealth;
        MaxHealth = maxHealth;
        CountGoldPerSecond = countGoldPerSecond;
        
        OnHealthChanged?.Invoke(health);

        switch (health)
        {
            case <= 20: level = 1;OnLevelChanged?.Invoke(level);break;
            case <= 40: level = 2;OnLevelChanged?.Invoke(level);break;
            default: level = 3;OnLevelChanged?.Invoke(level);break;
        }
    }

    private void Start()
    {
        OnLevelChanged?.Invoke(level);
        OnOwnerChanged?.Invoke();
        OnHealthChanged?.Invoke(health);
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
        if (!photonView.IsMine)
        {
            photonView.TransferOwnership(PhotonNetwork.LocalPlayer);
        }
        owner = player;
        OnOwnerChanged?.Invoke();
    }

    public virtual void BuildingHit(int damage, Player player)
    {
        if (!photonView.IsMine)
        {
            photonView.TransferOwnership(PhotonNetwork.LocalPlayer);
        }
        if (health <= 0)
        {
            health = 1;
            ChangePlayer(player);
        }
        health -= damage;
        if (health <= 0) health = 0;
        
        OnHealthChanged?.Invoke(health);
        
        switch (health)
        {
            case <= 20: level = 1;OnLevelChanged?.Invoke(level);break;
            case <= 40: level = 2;OnLevelChanged?.Invoke(level);break;
            default: level = 3;OnLevelChanged?.Invoke(level);break;
        }
    }

    public virtual void BuildingHeal(int damage)
    {
        if (!photonView.IsMine)
        {
            photonView.TransferOwnership(PhotonNetwork.LocalPlayer);
        }
        health += damage;
        if (health > MaxHealth)
        {
            health = MaxHealth;
        }
        
        OnHealthChanged?.Invoke(health);
        
        switch (health)
        {
            case <= 20: level = 1;OnLevelChanged?.Invoke(level);break;
            case <= 40: level = 2;OnLevelChanged?.Invoke(level);break;
            default: level = 3;OnLevelChanged?.Invoke(level);break;
        }
    }

    public virtual void BuildingHitWithoutOwner(int damage)
    {
        if (!photonView.IsMine)
        {
            photonView.TransferOwnership(PhotonNetwork.LocalPlayer);
        }
        health -= damage;
        if (health <= 0) health = 0;
        
        OnHealthChanged?.Invoke(health);
        
        switch (health)
        {
            case <= 20: level = 1;OnLevelChanged?.Invoke(level);break;
            case <= 40: level = 2;OnLevelChanged?.Invoke(level);break;
            default: level = 3;OnLevelChanged?.Invoke(level);break;
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

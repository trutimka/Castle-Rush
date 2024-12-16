
using Photon.Pun;
using UnityEngine;
using UnityEngine.Serialization;

public class Player : MonoBehaviourPunCallbacks, IPunObservable
{
    protected double boost = 1.0;
    [SerializeField]
    private double boostMultiplier = 1.01;
    [SerializeField]
    private int goldCount = 100;
    
    [SerializeField]
    private Camera playerCamera;
    public Camera PlayerCamera => playerCamera;
    
    [SerializeField]
    private Material playerMaterial;
    public Material PlayerMaterial => playerMaterial;
    
    public double Boost => boost;
    public int GoldCount => goldCount;
    public bool SpendGold(int amount)
    {
        if (amount < 0 || goldCount < amount) return false;
        goldCount -= amount;
        return true;
    }

    public bool AddGold(int amount)
    {
        if (amount < 0) return false;
        goldCount += amount;
        return true;
    }
    
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(boost);
            stream.SendNext(boostMultiplier);
            stream.SendNext(goldCount);
        }
        else
        {
            boost = (double)stream.ReceiveNext();
            boostMultiplier = (double)stream.ReceiveNext();
            goldCount = (int)stream.ReceiveNext();
        }
    }
}

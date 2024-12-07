
using Photon.Pun;
using UnityEngine;

public class GoldFarmer : Building, IPunObservable
{
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        base.OnPhotonSerializeView(stream, info);
    }
    protected override double? GenerateGold()
    {
        return base.GenerateGold() * 2;
    }
}

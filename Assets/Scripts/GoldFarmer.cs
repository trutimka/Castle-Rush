using UnityEngine;

public class GoldFarmer : Building
{
    [SerializeField] private float goldBooster = 2;
    protected override void GenerateGold()
    {
        if (Owner != Camera.main.GetComponent<Player>())
        Owner.AddGold(Owner.Boost * (CountGoldPerSecond + Owner.BoostGoldGeneration) * goldBooster);
    }
}

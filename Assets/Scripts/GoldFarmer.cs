
using UnityEngine;

public class GoldFarmer : Building
{
    protected override double? GenerateGold()
    {
        return base.GenerateGold() * 2;
    }
}

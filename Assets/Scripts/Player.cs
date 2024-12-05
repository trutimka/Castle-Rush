
using UnityEngine;
using UnityEngine.Serialization;

public class Player : MonoBehaviour
{
    protected double boost = 1.0;
    [SerializeField]
    private double boostMultiplier = 1.01;
    [SerializeField]
    private int goldCount = 100;
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
}

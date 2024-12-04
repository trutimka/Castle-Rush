using UnityEngine;


public abstract class Mob : MonoBehaviour
{
    protected int health;
    protected int damage;
    protected float speed;
    protected float attackSpeed;
    protected Player owner;
    
    public int Health => health;
    public int Damage => damage;
    public float Speed => speed;
    public Player Owner => owner;
    public float AttackSpeed => attackSpeed;
}

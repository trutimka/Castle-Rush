using UnityEngine;


public class Mob : MonoBehaviour
{
    [SerializeField]
    private int health = 2;
    [SerializeField]
    private int damage = 1;
    [SerializeField]
    private float speed = 1;
    [SerializeField]
    private float attackSpeed = 1;
    private Player owner;
    private Building target;
    
    public int Health => health;
    public int Damage => damage;
    public float Speed => speed;
    public Player Owner => owner;
    public float AttackSpeed => attackSpeed;
    public Building Target => target;
    
    
}

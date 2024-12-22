using System.Collections;
using Photon.Pun;
using UnityEngine;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Rigidbody))]
public class Mob : MonoBehaviourPun
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
    
    [SerializeField] private float attackCooldown = 1f;

    
    public int Health => health;
    public int Damage => damage;
    public float Speed => speed;
    public Player Owner => owner;
    public float AttackSpeed => attackSpeed;
    // public Building Target => target;
    public GameObject Target;
    
    private Animator animator;
    private Rigidbody rb;
    
    private Vector3 movementDirection;
    private bool isRunning = false;
    
    private void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
        
        if (Target != null)
        {
            InitializeMovement();
        }
    }

    public void SetOwner(Player newOwner)
    {
        owner = newOwner;

        health += owner.BoostMobHealth;
        damage += owner.BoostMobDamage;
        speed += owner.BoostMobSpeed;
        attackSpeed += owner.BoostMobSpeed;
    }

    private void InitializeMovement()
    {
        // Вычисляем направление движения
        movementDirection = (Target.transform.position - transform.position).normalized;

        // Устанавливаем поворот моба в сторону движения
        transform.rotation = Quaternion.LookRotation(movementDirection);

        // Запускаем анимацию бега
        animator.SetBool("isRunning", true);
        isRunning = true;
    }

    private IEnumerator InitializeAttacking()
    {
        var targetBuilding = (Target.transform.parent == null ? Target : Target.transform.parent.gameObject).GetComponent<Building>();
        
        while (targetBuilding != null)
        {
            // Запускаем анимацию атаки
            animator.SetTrigger("Attack");

            // Ждём окончания анимации атаки или сразу наносим урон 
            // Если хотите подождать событие анимации, можно сделать через Animation Event.
            yield return new WaitForSeconds(0.1f); // Небольшая задержка, чтобы анимация начала проигрываться
            if (!photonView.IsMine) continue;
            // Если здание союзное, лечим, иначе наносим урон
            if (targetBuilding.Owner == owner)
            {
                targetBuilding.BuildingHeal(damage);
            }
            else
            {
                targetBuilding.BuildingHit(damage, owner);
            }

            // Если задумка, что моб умирает после некоторого числа атак, можно уменьшать здоровье:
            health -= damage;
            if (health <= 0)
            {
                yield return new WaitForSeconds(0.75f);
                PhotonNetwork.Destroy(gameObject);
                yield break; // Прекращаем корутину
            }

            // Ждём время между атаками
            yield return new WaitForSeconds(attackCooldown);
        }
    }
    
    private void FixedUpdate()
    {
        if (isRunning && photonView.IsMine)
        {
            Vector3 move =  movementDirection * Owner.Boost * speed * Time.deltaTime;
            rb.MovePosition(transform.position + move);

            // Проверяем, достиг ли моб цели
            if (Vector3.Distance(transform.position, Target.transform.position) < 0.5f)
            {
                StopMovement();
            }
        }
    }
    
    private void StopMovement()
    {

        // Останавливаем анимацию бега
        animator.SetBool("isRunning", false);

        // Здесь можно добавить логику взаимодействия с целью
        Debug.Log("Mob reached the target!");
        isRunning = false;
        StartCoroutine(InitializeAttacking());

    }
    
    
}

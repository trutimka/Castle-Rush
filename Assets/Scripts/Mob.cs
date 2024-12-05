using UnityEngine;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Rigidbody))]
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

    private void InitializeAttacking()
    {
        animator.SetTrigger("Attack");
        Destroy(gameObject, 0.75f);
    }
    
    private void FixedUpdate()
    {
        if (isRunning)
        {
            Vector3 move = movementDirection * speed * Time.deltaTime;
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
        InitializeAttacking();
    }
}

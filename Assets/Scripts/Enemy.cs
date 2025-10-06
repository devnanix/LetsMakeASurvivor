using System.Collections;
using UnityEngine;
using UnityEngine.Pool;
using Physics = RotaryHeart.Lib.PhysicsExtension.Physics;

public class Enemy : MonoBehaviour, IDamageable
{
    [Header("Shared")]
    [SerializeField] private SharedTransform _target;
    private Transform target => _target.shared;

    [Header("Settings")]
    [SerializeField] private float healthMax = 1f;
    public float health { get; set; }

    [Header("Locomotion")]
    [SerializeField] private float moveSpeed = 12f;
    [SerializeField] private float acceleration = 48f;
    [SerializeField] private float accelerationMax = 48f;
    [SerializeField] private float rotateSpeed = 360f; // Degrees per second

    [Header("State - Chase:")]
    [SerializeField] private float chaseDistance = 25f;

    [Header("State - Telegraph:")]
    [SerializeField] private float telegraphTime = 1.0f;
    [SerializeField] private Transform telegraphVisual;

    [Header("State - Attack:")]
    [SerializeField] private Vector2 attackSize = Vector2.one * 1.5f;
    [SerializeField] private float attackRange = 3f;
    [SerializeField] private LayerMask targetLayer;
    [SerializeField] private float attackDamage = 1.0f;
    private Collider[] attackHits = new Collider[8];

    [Header("State - Cooldown:")]
    [SerializeField] private float cooldownTime = 1.0f;

    private Rigidbody rb;
    private Vector3 goalVelocity = Vector3.zero;
    private float stateTime;
    private StateMachine stateMachine;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();

        stateMachine = new StateMachine();
        stateMachine.AddState(Chase, null, null);
        stateMachine.AddState(Telegraph, EnterTelegraph, null);
        stateMachine.AddState(Attack, null, null);
        stateMachine.AddState(Cooldown, EnterCooldown, null);
    }

    private void Initialize()
    {
        health = healthMax;
        telegraphVisual.gameObject.SetActive(false);
        stateMachine.InitialState(Chase);
    }

    private void Start()
    {
        Initialize();
    }

    private void FixedUpdate()
    {
        stateMachine.Update();
    }

    void Chase()
    {
        // Chase the target
        Vector3 vector = (target.position - transform.position);
        Vector3 direction = vector.normalized;
        float distance = vector.sqrMagnitude;
        float facing = Vector3.Dot(direction, transform.forward);

        if (distance > chaseDistance) Move(direction);
        Look(direction);
        if (distance <= chaseDistance && facing >= 0.9f)
        {
            stateMachine.ChangeState(Telegraph);
        }
    }

    IEnumerator AnimateTelegraph(float duration)
    {
        telegraphVisual.gameObject.SetActive(true);
        telegraphVisual.localScale = new Vector3(attackSize.x, attackSize.y, 0.01f);
        float time = 0f;
        while(time < duration)
        {
            time += Time.deltaTime;
            float percentage = time / duration;
            telegraphVisual.localScale = new Vector3(attackSize.x, attackSize.y, attackRange * percentage);
            yield return null;
        }
        telegraphVisual.gameObject.SetActive(false);
    }

    void EnterTelegraph()
    {
        stateTime = Time.time + telegraphTime;
        if (telegraphVisual)
        {
            StartCoroutine(AnimateTelegraph(telegraphTime));
        }
    }

    void Telegraph()
    {
        // Telegraph the attack
        Vector3 vector = (target.position - transform.position);
        Vector3 direction = vector.normalized;
        Hold();
        if (Time.time >= stateTime)
        {
            stateMachine.ChangeState(Attack);
        }
    }

    void Attack()
    {
        // Do the attack
        System.Array.Clear(attackHits, 0, attackHits.Length);
        int hits = Physics.OverlapBoxNonAlloc(transform.position + transform.forward * (attackRange * 0.5f) + Vector3.up * (attackSize.y * 0.5f), new Vector3(attackSize.x * 0.5f, attackSize.y * 0.5f, attackRange * 0.5f), attackHits, transform.rotation, targetLayer, RotaryHeart.Lib.PhysicsExtension.PreviewCondition.Both, 0.1f);
        if (hits > 0)
        {
            for (int i = 0; i < hits; i++)
            {
                Collider current = attackHits[i];
                if(current.transform.TryGetComponent(out IDamageable damageable))
                {
                    damageable.Damage(attackDamage);
                }
            }
        }
        Hold();
        stateMachine.ChangeState(Cooldown);
    }

    void EnterCooldown()
    {
        stateTime = Time.time + cooldownTime;
    }

    void Cooldown()
    {
        // Activate the attack cooldown
        Hold();
        if (Time.time >= stateTime)
        {
            stateMachine.ChangeState(Chase);
        }
    }

    private void Hold()
    {
        Move(Vector3.zero);
    }

    private void Move(Vector3 direction)
    {
        Vector3 currentVelocity = rb.linearVelocity;

        goalVelocity = Vector3.MoveTowards(goalVelocity, direction * moveSpeed, acceleration * Time.fixedDeltaTime);

        Vector3 velocityChange = (goalVelocity - currentVelocity);
        velocityChange = Vector3.Scale(velocityChange, new Vector3(1f, 0f, 1f));
        Vector3.ClampMagnitude(velocityChange, accelerationMax);

        rb.AddForce(velocityChange, ForceMode.VelocityChange);
    }

    private void Look(Vector3 direction, float rotationMultiplier = 1f)
    {
        rb.angularVelocity = Vector3.zero;
        if (direction == Vector3.zero) return;

        Quaternion rotateTo = Quaternion.LookRotation(direction);
        Quaternion newRotation = Quaternion.RotateTowards(rb.rotation, rotateTo, rotateSpeed * rotationMultiplier * Time.fixedDeltaTime);

        rb.MoveRotation(newRotation);
    }

    public void Damage(float amount)
    {
        health -= amount;
        if(health <= 0)
        {
            health = 0;
            Death();
        }
    }

    public void Heal(float amount)
    {
        health += amount;
        if (health >= healthMax)
        {
            health = healthMax;
        }
    }
    public void Death()
    {
        Destroy(gameObject);
    }
}

using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("Shared")]
    [SerializeField] private SharedTransform _target;
    private Transform target => _target.shared;

    [Header("Settings")]
    [SerializeField] private float moveSpeed = 12f;
    [SerializeField] private float acceleration = 48f;
    [SerializeField] private float accelerationMax = 48f;
    [SerializeField] private float rotateSpeed = 360f; // Degrees per second

    [Header("State - Chase:")]
    [SerializeField] private float chaseDistance = 25f;

    [Header("State - Telegraph:")]
    [SerializeField] private float telegraphTime = 1.0f;

    //[Header("State - Attack:")]
    //[SerializeField] private float damage = 1.0f;

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

    private void Start()
    {
        stateMachine.InitialState(Chase);
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

    void EnterTelegraph()
    {
        stateTime = Time.time + telegraphTime;
    }

    void Telegraph()
    {
        // Telegraph the attack
        Hold();
        if (Time.time >= stateTime)
        {
            stateMachine.ChangeState(Attack);
        }
    }

    void Attack()
    {
        // Do the attack
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

    private void Look(Vector3 direction)
    {
        rb.angularVelocity = Vector3.zero;
        if (direction == Vector3.zero) return;

        Quaternion rotateTo = Quaternion.LookRotation(direction);
        Quaternion newRotation = Quaternion.RotateTowards(rb.rotation, rotateTo, rotateSpeed * Time.fixedDeltaTime);

        rb.MoveRotation(newRotation);
    }
}

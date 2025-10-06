using UnityEngine;
using Physics = RotaryHeart.Lib.PhysicsExtension.Physics;

public class Player : MonoBehaviour, IDamageable
{
    [Header("Input")]
    [SerializeField] private UserInputHandler input;

    [Header("Shared")]
    [SerializeField] private SharedFloat sharedHealth;

    [Header("Settings")]
    [SerializeField] private float healthMax = 5f;
    [SerializeField] private LayerMask targetLayer;

    [Header("Locomotion")]
    [SerializeField] private float moveSpeed = 12f;
    [SerializeField] private float acceleration = 48f;
    [SerializeField] private float accelerationMax = 48f;
    [SerializeField] private float rotateSpeed = 360f; // Degrees per second

    private Rigidbody rb;
    private Vector3 moveInput = Vector3.zero;
    private Vector3 goalVelocity = Vector3.zero;


    public float health { get => sharedHealth.shared; set => sharedHealth.shared = value; }

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        health = healthMax;
    }

    private void OnEnable()
    {
        input.Locomotion += OnMove;
        input.Jump += OnJump;
    }

    private void OnDisable()
    {
        input.Locomotion -= OnMove;
        input.Jump -= OnJump;
    }

    private void FixedUpdate()
    {
        Move();
        Look();
    }

    private void OnMove(Vector2 input)
    {
        moveInput = new Vector3(input.x, 0, input.y);
    }

    private void OnJump()
    {
        Collider[] results = new Collider[16];
        int hits = Physics.OverlapBoxNonAlloc(transform.position + Vector3.up * (1.5f), Vector3.one * 1.5f, results, transform.rotation, targetLayer, RotaryHeart.Lib.PhysicsExtension.PreviewCondition.Both, 0.1f);
        if (hits > 0)
        {
            for (int i = 0; i < hits; i++)
            {
                Collider current = results[i];
                if (current.transform.TryGetComponent(out IDamageable damageable))
                {
                    damageable.Damage(1f);
                }
            }
        }
    }

    private void Move()
    {
        Vector3 currentVelocity = rb.linearVelocity;

        goalVelocity = Vector3.MoveTowards(goalVelocity, moveInput * moveSpeed, acceleration * Time.fixedDeltaTime);

        Vector3 velocityChange = (goalVelocity - currentVelocity);
        velocityChange = Vector3.Scale(velocityChange, new Vector3(1f, 0f, 1f));
        Vector3.ClampMagnitude(velocityChange, accelerationMax);

        rb.AddForce(velocityChange, ForceMode.VelocityChange);
    }

    private void Look()
    {
        rb.angularVelocity = Vector3.zero;
        if (moveInput == Vector3.zero) return;

        Quaternion rotateTo = Quaternion.LookRotation(moveInput);
        Quaternion newRotation = Quaternion.RotateTowards(rb.rotation, rotateTo, rotateSpeed * Time.fixedDeltaTime);

        rb.MoveRotation(newRotation);
    }

    public void Damage(float amount)
    {
        health -= amount;
        if (health <= 0)
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
        // Player has died.
    }
}

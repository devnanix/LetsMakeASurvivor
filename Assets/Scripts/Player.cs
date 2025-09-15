using UnityEngine;

public class Player : MonoBehaviour
{
    [Header("Input")]
    [SerializeField] private UserInputHandler input;

    [Header("Shared")]
    [SerializeField] private SharedFloat sharedHealth;

    [Header("Settings")]
    [SerializeField] private float moveSpeed = 12f;
    [SerializeField] private float acceleration = 48f;
    [SerializeField] private float accelerationMax = 48f;
    [SerializeField] private float rotateSpeed = 360f; // Degrees per second

    private Rigidbody rb;
    private Vector3 moveInput = Vector3.zero;
    private Vector3 goalVelocity = Vector3.zero;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
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
        sharedHealth.shared -= 1f;
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
}

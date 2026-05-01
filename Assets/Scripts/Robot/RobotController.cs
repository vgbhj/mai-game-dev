using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class RobotController : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 8f;
    public float rotationSpeed = 120f;

    public float turnSpeed = 180f;

    private Rigidbody rb;
    private Vector3 moveInput;
    private float turnInput;
    private bool disabled;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;

        var energy = GetComponent<EnergySystem>();
        if (energy != null)
            energy.OnEnergyDepleted += () => disabled = true;
    }

    void Update()
    {
        var keyboard = Keyboard.current;
        if (keyboard == null) return;

        float v = 0f;
        float turn = 0f;

        if (keyboard.wKey.isPressed || keyboard.upArrowKey.isPressed)    v =  1f;
        if (keyboard.sKey.isPressed || keyboard.downArrowKey.isPressed)  v = -1f;
        if (keyboard.aKey.isPressed || keyboard.leftArrowKey.isPressed)   turn = -1f;
        if (keyboard.dKey.isPressed || keyboard.rightArrowKey.isPressed)  turn =  1f;

        moveInput = disabled ? Vector3.zero : new Vector3(0, 0, v);
        turnInput = disabled ? 0f : turn;
    }

    void FixedUpdate()
    {
        if (Mathf.Abs(turnInput) > 0.01f)
        {
            float angle = turnInput * turnSpeed * Time.fixedDeltaTime;
            rb.MoveRotation(rb.rotation * Quaternion.Euler(0, angle, 0));
        }

        if (moveInput.sqrMagnitude > 0.01f)
        {
            Vector3 move = rb.rotation * moveInput.normalized * moveSpeed * Time.fixedDeltaTime;
            rb.MovePosition(rb.position + move);
        }
    }

    public bool IsMoving => moveInput.sqrMagnitude > 0.01f;
}

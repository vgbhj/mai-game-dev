using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class RobotController : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 8f;
    public float rotationSpeed = 120f;

    private Rigidbody rb;
    private Vector3 moveInput;
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

        float h = 0f;
        float v = 0f;

        if (keyboard.aKey.isPressed || keyboard.leftArrowKey.isPressed)  h = -1f;
        if (keyboard.dKey.isPressed || keyboard.rightArrowKey.isPressed) h =  1f;
        if (keyboard.sKey.isPressed || keyboard.downArrowKey.isPressed)  v = -1f;
        if (keyboard.wKey.isPressed || keyboard.upArrowKey.isPressed)    v =  1f;

        moveInput = disabled ? Vector3.zero : new Vector3(h, 0, v).normalized;
    }

    void FixedUpdate()
    {
        if (moveInput.sqrMagnitude > 0.01f)
        {
            Vector3 move = moveInput * moveSpeed * Time.fixedDeltaTime;
            rb.MovePosition(rb.position + move);

            Quaternion targetRot = Quaternion.LookRotation(moveInput);
            rb.rotation = Quaternion.RotateTowards(
                rb.rotation, targetRot, rotationSpeed * Time.fixedDeltaTime
            );
        }
    }

    public bool IsMoving => moveInput.sqrMagnitude > 0.01f;
}

using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class RobotController : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 8f;
    public float rotationSpeed = 120f;

    public float turnSpeed = 180f;
    public float groundOffset = 1f;

    private Rigidbody rb;
    private Terrain terrain;
    private Vector3 moveInput;
    private float turnInput;
    private bool disabled;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.isKinematic = true;
        terrain = Terrain.activeTerrain;

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

        if (Mathf.Abs(turnInput) > 0.01f)
        {
            float angle = turnInput * turnSpeed * Time.deltaTime;
            transform.rotation *= Quaternion.Euler(0, angle, 0);
        }

        Vector3 pos = transform.position;
        if (moveInput.sqrMagnitude > 0.01f)
        {
            Vector3 move = transform.rotation * moveInput.normalized * moveSpeed * Time.deltaTime;
            pos += move;
        }

        if (terrain != null)
            pos.y = terrain.SampleHeight(pos) + terrain.transform.position.y + groundOffset;

        transform.position = pos;
    }

    public bool IsMoving => moveInput.sqrMagnitude > 0.01f;
}

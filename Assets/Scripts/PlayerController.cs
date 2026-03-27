using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(LineRenderer))]

public class PlayerController : MonoBehaviour
{
    [Header("Stats")]
    public int maxHp = 100;
    public int currentHp;
    public int keyCount = 0;

    [Header("Movement")]
    public float baseSpeed = 5f;
    [HideInInspector] public float currentSpeed;

    [Header("Aiming (Raycast)")]
    public LayerMask groundLayer;
    private LineRenderer aimLine;
    private Rigidbody rb;
    private Camera mainCam;

    [Header("Inputs")]
    public InputAction moveAction;
    public InputAction aimAction;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        aimLine = GetComponent<LineRenderer>();
        mainCam = Camera.main;

        currentHp = maxHp;
        currentSpeed = baseSpeed;

        aimLine.positionCount = 2;
        aimLine.startWidth = 0.05f;
        aimLine.endWidth = 0.05f;
    }

    void OnEnable()
    {
        moveAction.Enable();
        aimAction.Enable();
    }

    void OnDisable()
    {
        moveAction.Disable();
        aimAction.Disable();
    }

    void Update()
    {
        HandleAiming();
    }

    private void FixedUpdate()
    {
        HandleMovement();
    }

    private void HandleMovement()
    {
        Vector2 inputDir = moveAction.ReadValue<Vector2>();

        Vector3 moveDir = new Vector3(inputDir.x, 0, inputDir.y).normalized;
        rb.MovePosition(rb.position + moveDir * currentSpeed * Time.fixedDeltaTime);
    }

    private void HandleAiming()
    {
        Vector2 mousePos = aimAction.ReadValue<Vector2>();
        Ray ray = mainCam.ScreenPointToRay(mousePos);
        if (Physics.Raycast(ray, out RaycastHit hit, 100f, groundLayer))
        {
            Vector3 targetPoint = hit.point;
            targetPoint.y = transform.position.y;

            transform.LookAt(targetPoint);

            aimLine.SetPosition(0, transform.position);
            aimLine.SetPosition(1, targetPoint);
        }
    }

    public void TakeDamage(int damage)
    {
        currentHp -= damage;
        if (currentHp <= 0) Debug.Log("Game Over!");
    }
}

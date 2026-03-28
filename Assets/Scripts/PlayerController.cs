using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    [Header("Stats")]
    public int maxHP = 100;
    public int currentHP;
    public int keyCount = 0;
    public int attackDamage = 20;

    [Header("Movement")]
    public float baseSpeed = 5f;
    public float sprintSpeed = 8f; // 🏃‍♂️ เพิ่มความเร็วตอนวิ่ง
    [HideInInspector] public float currentSpeed;

    [Header("Aiming (Raycast)")]
    public LayerMask groundLayer;
    public float weaponRange = 50f;
    private Rigidbody rb;
    private Camera mainCam;
    private GameObject currentTarget;

    [Header("Inputs")]
    public InputAction moveAction;
    public InputAction aimAction;
    public InputAction shootAction;
    public InputAction sprintAction; // 🏃‍♂️ เพิ่ม Action สำหรับปุ่มวิ่ง

    [Header("Level Exit")]
    public string nextLevelName = "Level 2";
    public int requiredKeysToExit = 1;

    void Awake()
    {
        Time.timeScale = 1f;
        rb = GetComponent<Rigidbody>();
        mainCam = Camera.main;
        currentHP = maxHP;
        currentSpeed = baseSpeed;
    }

    void OnEnable()
    {
        moveAction.Enable();
        aimAction.Enable();
        shootAction.Enable();
        sprintAction.Enable(); // 🏃‍♂️ เปิดใช้งาน Action
    }

    void OnDisable()
    {
        moveAction.Disable();
        aimAction.Disable();
        shootAction.Disable();
        sprintAction.Disable(); // 🏃‍♂️ ปิดใช้งาน Action
    }

    void Update()
    {
        HandleAiming();
        HandleSprint(); // 🏃‍♂️ เรียกใช้ฟังก์ชันเช็กการวิ่ง
        if (shootAction.WasPressedThisFrame()) Shoot();
    }

    void FixedUpdate()
    {
        HandleMovement();
    }

    // 🏃‍♂️ ฟังก์ชันจัดการความเร็ว
    private void HandleSprint()
    {
        // IsPressed() จะเป็นจริงตลอดเวลาที่ปุ่มถูกกดค้างไว้
        if (sprintAction.IsPressed())
        {
            currentSpeed = sprintSpeed;
        }
        else
        {
            currentSpeed = baseSpeed;
        }
    }

    private void HandleMovement()
    {
        Vector2 inputDir = moveAction.ReadValue<Vector2>();
        Vector3 moveDir = new Vector3(inputDir.x, 0, inputDir.y).normalized;
        rb.MovePosition(rb.position + moveDir * currentSpeed * Time.fixedDeltaTime);
    }

    private void HandleAiming()
    {
        Ray ray = mainCam.ScreenPointToRay(aimAction.ReadValue<Vector2>());
        if (Physics.Raycast(ray, out RaycastHit groundHit, 100f, groundLayer))
        {
            Vector3 targetPoint = groundHit.point;
            targetPoint.y = transform.position.y;
            transform.LookAt(targetPoint);
        }

        Vector3 shootOrigin = transform.position + new Vector3(0, 1f, 0) + (transform.forward * 1f);

        if (Physics.Raycast(shootOrigin, transform.forward, out RaycastHit hit, weaponRange))
        {
            if (hit.collider.CompareTag("Enemy"))
            {
                if (currentTarget != hit.collider.gameObject)
                {
                    currentTarget = hit.collider.gameObject;
                    Debug.Log($"🎯 Target locked: {currentTarget.name}");
                }
                Debug.DrawRay(shootOrigin, transform.forward * hit.distance, Color.red);
            }
            else
            {
                currentTarget = null;
                Debug.DrawRay(shootOrigin, transform.forward * hit.distance, Color.darkGreen);
            }
        }
        else
        {
            currentTarget = null;
            Debug.DrawRay(shootOrigin, transform.forward * weaponRange, Color.white);
        }
    }

    private void Shoot()
    {
        if (currentTarget != null && currentTarget.TryGetComponent<Enemy>(out Enemy enemyScript))
        {
            enemyScript.TakeDamage(attackDamage);
            int remainingHP = Mathf.Max(0, enemyScript.hp);

            string status = remainingHP > 0 ? $"Remaining HP: {remainingHP}" : "Enemy is Dead!";
            Debug.Log($"Hit {currentTarget.name}! Damage: {attackDamage} | {status}");
        }
        else
        {
            Debug.Log("Not Hit");
        }
    }

    public void TakeDamage(int damage)
    {
        currentHP -= damage;
        if (currentHP <= 0) Debug.Log("Player Game Over!");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("ExitDoor"))
        {
            if (keyCount >= requiredKeysToExit)
            {
                Debug.Log("Loading to next level....");
                SceneManager.LoadScene(nextLevelName);
            }
            else
            {
                Debug.Log($"Need more keys! Have {keyCount}/{requiredKeysToExit} keys.");
            }
        }
    }
}
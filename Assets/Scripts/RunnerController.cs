using UnityEngine;
using System.Collections;
using UnityEngine.InputSystem;

public class RunnerController : MonoBehaviour
{
    [Header("Run")]
    [SerializeField] private float forwardSpeed = 5f;
    [SerializeField] private float startDelay = 2f;

    [Header("Lane")]
    [SerializeField] private float laneDistance = 2.5f;
    [SerializeField] private float laneChangeSpeed = 12f;

    [Header("Lane Feel")]
    [SerializeField] private Transform visualRoot;
    [SerializeField] private float laneTurnAngle = 18f;
    [SerializeField] private float laneLeanAngle = 10f;
    [SerializeField] private float visualRotateSpeed = 12f;

    [Header("Jump")]
    [SerializeField] private float jumpHeight = 2.8f;
    [SerializeField] private float gravity = 18f;
    [SerializeField] private float fallGravityMultiplier = 1.8f;
    [SerializeField] private float apexGravityMultiplier = 0.35f;
    [SerializeField] private float apexThreshold = 2f;

    [Header("Roll")]
    [SerializeField] private float rollDuration = 0.75f;
    [SerializeField] private float airRollGravityMultiplier = 4.5f;

    [Header("References")]
    [SerializeField] private Animator animator;

    private bool isRunning;
    private bool isRolling;
    private bool forceFastFall;

    private int currentLane = 0; // -1 = sol, 0 = orta, 1 = sağ

    private float targetX;
    private float fixedY;

    private float verticalVelocity;
    private float currentY;
    private float rollTimer;
    private bool isGrounded = true;
    private Quaternion visualBaseLocalRotation;

    private void Awake()
    {
        if (animator == null)
            animator = GetComponentInChildren<Animator>();

        if (animator != null)
            animator.applyRootMotion = false;

        if (visualRoot == null && animator != null)
            visualRoot = animator.transform;
    }

    private void Start()
    {
        isRunning = false;
        isRolling = false;
        forceFastFall = false;

        fixedY = transform.position.y;
        currentY = fixedY;
        targetX = transform.position.x;

        if (animator != null)
        {
            animator.SetBool("isRunning", false);
            animator.SetBool("isGrounded", true);
            animator.SetBool("isRolling", false);
        }
        if (visualRoot != null)
    visualBaseLocalRotation = visualRoot.localRotation;

        StartCoroutine(StartRunRoutine());
    }

    private void Update()
    {
        if (!isRunning) return;

        HandleLaneInput();
        HandleJumpInput();
        HandleRollInput();
        UpdateRoll();
        MoveRunner();
        UpdateVisualLaneTurn();
    }

    private IEnumerator StartRunRoutine()
    {
        yield return new WaitForSeconds(startDelay);

        isRunning = true;

        if (animator != null)
            animator.SetBool("isRunning", true);
    }

    private void HandleLaneInput()
    {
        if (Keyboard.current == null) return;

        if (Keyboard.current.qKey.wasPressedThisFrame || Keyboard.current.aKey.wasPressedThisFrame)
        {
            currentLane = Mathf.Clamp(currentLane - 1, -1, 1);
            targetX = currentLane * laneDistance;
        }

        if (Keyboard.current.eKey.wasPressedThisFrame || Keyboard.current.dKey.wasPressedThisFrame)
        {
            currentLane = Mathf.Clamp(currentLane + 1, -1, 1);
            targetX = currentLane * laneDistance;
        }
    }

    private void HandleJumpInput()
    {
        if (Keyboard.current == null) return;

        if ((Keyboard.current.spaceKey.wasPressedThisFrame || Keyboard.current.wKey.wasPressedThisFrame)
            && isGrounded)
        {
            isGrounded = false;
            verticalVelocity = Mathf.Sqrt(jumpHeight * 2f * gravity);
            forceFastFall = false;

            if (isRolling)
                StopRoll();

            if (animator != null)
                animator.SetBool("isGrounded", false);
        }
    }

    private void HandleRollInput()
    {
        if (Keyboard.current == null) return;

        if (Keyboard.current.sKey.wasPressedThisFrame && !isRolling)
        {
            StartRoll();
        }
    }

    private void StartRoll()
    {
        isRolling = true;
        rollTimer = rollDuration;

        // Havada roll yapılırsa hızlı düş
        if (!isGrounded)
        {
            forceFastFall = true;

            // Karakter hâlâ yukarı çıkıyorsa anında aşağı yönlendir
            if (verticalVelocity > 0f)
                verticalVelocity = 0f;
        }

        if (animator != null)
            animator.SetBool("isRolling", true);
    }

    private void UpdateRoll()
    {
        if (!isRolling) return;

        rollTimer -= Time.deltaTime;

        if (rollTimer <= 0f)
        {
            StopRoll();
        }
    }

    private void StopRoll()
    {
        isRolling = false;
        forceFastFall = false;

        if (animator != null)
            animator.SetBool("isRolling", false);
    }

    private void MoveRunner()
    {
        Vector3 currentPos = transform.position;

        // sürekli ileri koş
        currentPos += Vector3.forward * forwardSpeed * Time.deltaTime;

        // lane geçişi
        float newX = Mathf.MoveTowards(currentPos.x, targetX, laneChangeSpeed * Time.deltaTime);

        // jump physics
        ApplyJumpPhysics();

        transform.position = new Vector3(newX, currentY, currentPos.z);
    }

    private void ApplyJumpPhysics()
    {
        if (isGrounded)
        {
            currentY = fixedY;
            return;
        }

        float currentGravity = gravity;

        // Havada roll yaptıysa sert düşüş
        if (forceFastFall)
        {
            currentGravity *= airRollGravityMultiplier;
        }
        else
        {
            // tepe noktasına yaklaşınca gravity azalır
            if (verticalVelocity > 0f && verticalVelocity < apexThreshold)
                currentGravity *= apexGravityMultiplier;

            // normal düşüş
            if (verticalVelocity < 0f)
                currentGravity *= fallGravityMultiplier;
        }

        verticalVelocity -= currentGravity * Time.deltaTime;
        currentY += verticalVelocity * Time.deltaTime;

        if (currentY <= fixedY)
        {
            currentY = fixedY;
            verticalVelocity = 0f;
            isGrounded = true;
            forceFastFall = false;

            if (animator != null)
                animator.SetBool("isGrounded", true);
        }
    }

    private void UpdateVisualLaneTurn()
    {
        if (visualRoot == null) return;

        float laneDelta = targetX - transform.position.x;

        float targetYaw = 0f;
        float targetZLean = 0f;

        if (Mathf.Abs(laneDelta) > 0.05f)
        {
            float dir = Mathf.Sign(laneDelta);

            targetYaw = dir * laneTurnAngle;
            targetZLean = -dir * laneLeanAngle;
        }

        Quaternion offsetRotation = Quaternion.Euler(0f, targetYaw, targetZLean);
        Quaternion targetRotation = visualBaseLocalRotation * offsetRotation;

        visualRoot.localRotation = Quaternion.Slerp(
            visualRoot.localRotation,
            targetRotation,
            visualRotateSpeed * Time.deltaTime
        );
    }
}
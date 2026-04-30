using UnityEngine;
using System.Collections;
using UnityEngine.InputSystem;

public class RunnerController : MonoBehaviour
{
    [Header("Run")]
    [SerializeField] private float forwardSpeed = 5f;
    [SerializeField] private float startDelay = 2f;

    [Header("Speed Increase")]
    [SerializeField] private float speedIncreaseRate = 0.2f;
    [SerializeField] private float maxSpeed = 20f;

    private float currentSpeed;

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

    [Header("Ground Detect")]
    [SerializeField] private float groundCheckHeight = 2f;
    [SerializeField] private float groundCheckDistance = 5f;
    [SerializeField] private LayerMask groundMask = ~0;
    [SerializeField] private float groundedSnapSpeed = 20f;
    [SerializeField] private float groundOffset = 0f;

    [Header("Hit Reaction")]
    [SerializeField] private string hitTriggerName = "Hit";
    [SerializeField] private float hitInvulnerableTime = 0.6f;
    [Tooltip("Animasyon eventi çalıştıktan (kalktıktan) sonra koşmaya başlamak için ekstra bekleme süresi")]
    [SerializeField] private float postHitResumeDelay = 0.3f; 

    [Header("References")]
    [SerializeField] private Animator animator;

    private bool isRunning;
    private bool isRolling;
    private bool forceFastFall;
    private bool isHitReacting;
    private bool canTakeHit = true;

    private int currentLane = 0;

    private float targetX;
    private float currentGroundY;
    private float verticalVelocity;
    private float currentY;
    private float rollTimer;
    private bool isGrounded = true;
    private Quaternion visualBaseLocalRotation;

    private Vector3 hitLockedPosition;
    private float runnerZ;
    private Vector3 initialPosition;

    public bool IsHitReacting => isHitReacting;
    public Vector3 HitLockedPosition => hitLockedPosition;

    private void Awake()
    {
        initialPosition = transform.position;

        if (animator == null)
            animator = GetComponentInChildren<Animator>();

        if (animator != null)
            animator.applyRootMotion = false;

        if (visualRoot == null && animator != null)
            visualRoot = animator.transform;
    }

    private void Start()
    {
        ResetRunner(initialPosition);
    }

    private void Update()
    {
        if (!isRunning) return;

        if (isHitReacting)
        {
            currentY = Mathf.MoveTowards(currentY, currentGroundY, groundedSnapSpeed * Time.deltaTime);
            transform.position = new Vector3(hitLockedPosition.x, currentY, hitLockedPosition.z);
            return;
        }

        HandleLaneInput();
        HandleJumpInput();
        HandleRollInput();
        UpdateRoll();

        if (isGrounded || verticalVelocity <= 0f)
            UpdateGroundReference();

        MoveRunner();
        UpdateVisualLaneTurn();
    }

    public void ResetRunner()
    {
        ResetRunner(initialPosition);
    }

    public void ResetRunner(Vector3 startPosition)
    {
        StopAllCoroutines();

        isRunning = false;
        isRolling = false;
        forceFastFall = false;
        isHitReacting = false;
        canTakeHit = true;

        currentLane = 0;
        targetX = startPosition.x;
        currentGroundY = startPosition.y;
        currentY = startPosition.y;
        verticalVelocity = 0f;
        rollTimer = 0f;
        isGrounded = true;
        currentSpeed = forwardSpeed;

        runnerZ = startPosition.z;
        hitLockedPosition = startPosition;

        transform.position = startPosition;
        transform.rotation = Quaternion.identity;

        if (animator != null)
        {
            animator.Rebind();
            animator.Update(0f);
            animator.SetBool("isRunning", false);
            animator.SetBool("isGrounded", true);
            animator.SetBool("isRolling", false);
            animator.ResetTrigger(hitTriggerName);
        }

        if (visualRoot != null)
        {
            visualBaseLocalRotation = visualRoot.localRotation;
        }

        StartCoroutine(StartRunRoutine());
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
            if (currentLane > -1 && !IsLaneBlocked(Vector3.left))
            {
                currentLane--;
                targetX = currentLane * laneDistance;
            }
        }

        if (Keyboard.current.eKey.wasPressedThisFrame || Keyboard.current.dKey.wasPressedThisFrame)
        {
            if (currentLane < 1 && !IsLaneBlocked(Vector3.right))
            {
                currentLane++;
                targetX = currentLane * laneDistance;
            }
        }
    }

    private bool IsLaneBlocked(Vector3 direction)
    {
        // Karakterin bel hizasından yana doğru görünmez bir küre (SphereCast) fırlatıyoruz
        Vector3 rayOrigin = transform.position + Vector3.up * 1f; 
        float radius = 0.5f; // Karakterin genişliği
        
        // Yana doğru şerit genişliği kadar tarama yap
        RaycastHit[] hits = Physics.SphereCastAll(rayOrigin, radius, direction, laneDistance, Physics.AllLayers, QueryTriggerInteraction.Collide);
        
        foreach (var hit in hits)
        {
            // Eğer yana atılan ışın bir engele veya hasar bölgesine (otobüse) çarptıysa:
            if (hit.collider.GetComponentInParent<ObstacleDisappear>() != null || hit.collider.GetComponentInParent<DamageZone>() != null)
            {
                Debug.Log("Şerit değiştirme engellendi! Yanda otobüs var.");
                return true; // Şerit dolu, geçişi iptal et!
            }
        }
        return false;
    }

    private void HandleJumpInput()
    {
        if (Keyboard.current == null) return;

        if ((Keyboard.current.spaceKey.wasPressedThisFrame || Keyboard.current.wKey.wasPressedThisFrame) && isGrounded)
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
            StartRoll();
    }

    private void StartRoll()
    {
        isRolling = true;
        rollTimer = rollDuration;

        if (!isGrounded)
        {
            forceFastFall = true;
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
            StopRoll();
    }

    private void StopRoll()
    {
        isRolling = false;
        forceFastFall = false;

        if (animator != null)
            animator.SetBool("isRolling", false);
    }

    private void UpdateGroundReference()
    {
        Vector3 rayOrigin = transform.position + Vector3.up * groundCheckHeight;

        // Eski groundCheckDistance (genelde 5) otobüs boyu kadar derinliği göremeyebilir.
        // Bu yüzden güvenli, çok daha uzun bir 'güvenli mesafe' atıyoruz (50 birim).
        float safeDistance = Mathf.Max(groundCheckDistance, 50f);

        if (Physics.Raycast(rayOrigin, Vector3.down, out RaycastHit hit, safeDistance, groundMask, QueryTriggerInteraction.Ignore))
        {
            currentGroundY = hit.point.y + groundOffset;
        }
        else
        {
            currentGroundY = 0f; // Eğer altından yol biterse, varsayılan zemini en aşağısı (0) olarak kabul et.
        }
    }

    private void MoveRunner()
    {
        if (currentSpeed < maxSpeed)
        {
            currentSpeed += speedIncreaseRate * Time.deltaTime;
        }

        runnerZ += currentSpeed * Time.deltaTime;
        float newX = Mathf.MoveTowards(transform.position.x, targetX, laneChangeSpeed * Time.deltaTime);

        ApplyJumpPhysics();

        transform.position = new Vector3(newX, currentY, runnerZ);
    }

    private void ApplyJumpPhysics()
    {
        if (isGrounded)
        {
            // Karakter yere basıyor sanarken aslında altındaki zemin aniden yok olduysa
            // (örneğin otobüsün üzerinden aşağı boşluğa adım atarsa):
            if (currentY - currentGroundY > 0.5f)
            {
                isGrounded = false;
                verticalVelocity = 0f; // Serbest düşüş başlasın
                
                if (animator != null)
                    animator.SetBool("isGrounded", false);
            }
            else
            {
                // Normal zemin eğimlerinde yavaşça ayak uydur.
                currentY = Mathf.MoveTowards(currentY, currentGroundY, groundedSnapSpeed * Time.deltaTime);
                return;
            }
        }

        float currentGravity = gravity;

        if (forceFastFall)
            currentGravity *= airRollGravityMultiplier;
        else
        {
            if (verticalVelocity > 0f && verticalVelocity < apexThreshold)
                currentGravity *= apexGravityMultiplier;

            if (verticalVelocity < 0f)
                currentGravity *= fallGravityMultiplier;
        }

        verticalVelocity -= currentGravity * Time.deltaTime;
        currentY += verticalVelocity * Time.deltaTime;

        if (verticalVelocity <= 0f && currentY <= currentGroundY + 0.02f)
        {
            currentY = currentGroundY;
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

    public void TriggerHitReaction()
    {
        if (!gameObject.activeInHierarchy) return;
        if (isHitReacting) return;
        if (!canTakeHit) return;

        StartCoroutine(HitReactionRoutine());
    }


    private IEnumerator HitReactionRoutine()
    {
        isHitReacting = true;
        canTakeHit = false;

        hitLockedPosition = transform.position;
        runnerZ = transform.position.z;

        if (isRolling) StopRoll();

        forceFastFall = false;
        verticalVelocity = 0f;
        isGrounded = true;

        UpdateGroundReference();
        currentY = currentGroundY;

        if (animator != null)
        {
            animator.SetBool("isGrounded", true);
            animator.SetBool("isRolling", false);
            animator.SetBool("isRunning", true);
            animator.ResetTrigger(hitTriggerName);
            animator.SetTrigger(hitTriggerName);
        }

        // Animasyon motorunun tetiklenmesi için minik bir bekleme
        yield return new WaitForSeconds(0.1f);

        if (animator != null)
        {
            float safetyTimer = 0f;
            bool isInHitState = false;

            // Maksimum 5 saniyelik sigorta süresi. Event'leri ve sabit süreleri tamamen çöpe atıyoruz!
            // Animasyon %95 oranında tamamlanana kadar (normalizedTime >= 0.95f) karakteri kilitli tutacağız.
            while (safetyTimer < 5f)
            {
                safetyTimer += Time.deltaTime;

                AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
                
                // Karakter düşme animasyonuna girdi mi veya hala geçişte mi?
                if (animator.IsInTransition(0) || stateInfo.normalizedTime < 0.95f)
                {
                    isInHitState = true;
                    yield return null; // Kilitli kalmaya devam et
                }
                else if (isInHitState)
                {
                    // Animasyon %95'i geçti ve geçiş bitti! Artık kilidi açabiliriz.
                    break;
                }
                else
                {
                    yield return null;
                }
            }
        }

        EndHitReactionDirectly();
    }

    public void EndHitReaction()
    {
        // HitAnimationRelay (Event) üzerinden gelen eski çağrıları yoksayıyoruz.
        // Çünkü artık süreci tamamen yukarıdaki Routine yönetiyor.
    }

    private void EndHitReactionDirectly()
    {
        if (!isHitReacting) return;

        isHitReacting = false;

        UpdateGroundReference();
        currentY = currentGroundY;

        forceFastFall = false;
        verticalVelocity = 0f;
        isGrounded = true;
        runnerZ = transform.position.z;

        transform.position = new Vector3(transform.position.x, currentY, runnerZ);

        if (animator != null)
        {
            animator.SetBool("isRunning", true);
            animator.SetBool("isGrounded", true);
            animator.SetBool("isRolling", false);
        }

        StartCoroutine(HitInvulnerabilityRoutine());
    }

    private IEnumerator HitInvulnerabilityRoutine()
    {
        yield return new WaitForSeconds(hitInvulnerableTime);
        canTakeHit = true;
    }
}
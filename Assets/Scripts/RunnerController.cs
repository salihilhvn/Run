using UnityEngine;
using System.Collections;
using UnityEngine.InputSystem;

public class RunnerController : MonoBehaviour
{
    [Header("Run")]
    [SerializeField] private float forwardSpeed = 5f;
    [SerializeField] private float startDelay = 2f;

    [Header("Lane")]
    [SerializeField] private float laneDistance = 2.5f;     // lane arası mesafe
    [SerializeField] private float laneChangeSpeed = 12f;   // sağa sola geçiş hızı

    [Header("References")]
    [SerializeField] private Animator animator;

    private bool isRunning;
    private int currentLane = 0; // -1 = sol, 0 = orta, 1 = sağ

    private float targetX;
    private float fixedY;

    private void Awake()
    {
        if (animator == null)
            animator = GetComponentInChildren<Animator>();
    }

    private void Start()
    {
        isRunning = false;
        fixedY = transform.position.y;
        targetX = transform.position.x;

        if (animator != null)
            animator.SetBool("isRunning", false);

        StartCoroutine(StartRunRoutine());
    }

    private void Update()
    {
        if (!isRunning) return;

        HandleLaneInput();
        MoveRunner();
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

    private void MoveRunner()
    {
        Vector3 currentPos = transform.position;

        // sürekli ileri koş
        currentPos += Vector3.forward * forwardSpeed * Time.deltaTime;

        // lane geçişi
        float newX = Mathf.MoveTowards(currentPos.x, targetX, laneChangeSpeed * Time.deltaTime);

        transform.position = new Vector3(newX, fixedY, currentPos.z);
    }
}
using UnityEngine;
using System.Collections;
using UnityEngine.InputSystem;

public class RunnerController : MonoBehaviour
{
    [Header("Run")]
    [SerializeField] private float forwardSpeed = 5f;
    [SerializeField] private float startDelay = 2f;

    [Header("Lane")]
    [SerializeField] private float laneDistance = 2f;
    [SerializeField] private float laneChangeSpeed = 10f;

    [Header("References")]
    [SerializeField] private Animator animator;

    private bool isRunning = false;
    private int currentLane = 0;

    private float startX;
    private float currentX;
    private float targetX;
    private float currentZ;
    private float fixedY;

    private void Awake()
    {
        if (animator == null)
            animator = GetComponentInChildren<Animator>();
    }

    private void Start()
    {
        isRunning = false;

        startX = transform.position.x;
        currentX = startX;
        targetX = startX;
        currentZ = transform.position.z;
        fixedY = transform.position.y;

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

        if (Keyboard.current.qKey.wasPressedThisFrame)
        {
            currentLane = Mathf.Clamp(currentLane - 1, -1, 1);
            targetX = startX + (currentLane * laneDistance);
        }

        if (Keyboard.current.eKey.wasPressedThisFrame)
        {
            currentLane = Mathf.Clamp(currentLane + 1, -1, 1);
            targetX = startX + (currentLane * laneDistance);
        }
    }

    private void MoveRunner()
    {
        currentZ += forwardSpeed * Time.deltaTime;
        currentX = Mathf.MoveTowards(currentX, targetX, laneChangeSpeed * Time.deltaTime);

        transform.position = new Vector3(currentX, fixedY, currentZ);
    }
}
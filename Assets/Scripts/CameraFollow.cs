using UnityEngine;

public class RunnerCameraFollow : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private float height = 7.5f;
    [SerializeField] private float distance = 6.5f;
    [SerializeField] private float followSpeed = 8f;
    [SerializeField] private float rotationSpeed = 6f;

    [Header("Look")]
    [SerializeField] private float lookHeight = 1.0f;
    [SerializeField] private bool followLaneX = true;

    private RunnerController runner;

    private void Awake()
    {
        if (target != null)
            runner = target.GetComponent<RunnerController>();
    }

    private void LateUpdate()
    {
        if (target == null) return;

        Vector3 followPosition = target.position;

        if (runner != null && runner.IsHitReacting)
            followPosition = runner.HitLockedPosition;

        Vector3 desiredPosition = new Vector3(
            followLaneX ? followPosition.x : transform.position.x,
            followPosition.y + height,
            followPosition.z - distance
        );

        transform.position = Vector3.Lerp(
            transform.position,
            desiredPosition,
            followSpeed * Time.deltaTime
        );

        Vector3 lookTarget = new Vector3(
            followLaneX ? followPosition.x : transform.position.x,
            followPosition.y + lookHeight,
            followPosition.z + 1.5f
        );

        Quaternion targetRotation = Quaternion.LookRotation(lookTarget - transform.position);

        transform.rotation = Quaternion.Lerp(
            transform.rotation,
            targetRotation,
            rotationSpeed * Time.deltaTime
        );
    }
}
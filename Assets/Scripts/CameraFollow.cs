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

    private void LateUpdate()
    {
        if (target == null) return;

        Vector3 desiredPosition = new Vector3(
            followLaneX ? target.position.x : transform.position.x,
            target.position.y + height,
            target.position.z - distance
        );

        transform.position = Vector3.Lerp(
            transform.position,
            desiredPosition,
            followSpeed * Time.deltaTime
        );

        Vector3 lookTarget = new Vector3(
            followLaneX ? target.position.x : transform.position.x,
            target.position.y + lookHeight,
            target.position.z + 1.5f
        );

        Quaternion targetRotation = Quaternion.LookRotation(lookTarget - transform.position);

        transform.rotation = Quaternion.Lerp(
            transform.rotation,
            targetRotation,
            rotationSpeed * Time.deltaTime
        );
    }
}
using UnityEngine;

public class RunnerCameraFollow : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private float height = 7.5f;
    [SerializeField] private float distance = 6.5f;
    [SerializeField] private float followSpeed = 10f;
    [SerializeField] private float rotationSpeed = 10f;

    [Header("Look")]
    [SerializeField] private float lookHeight = 0.6f;
    [SerializeField] private bool followLaneX = true;

    private void LateUpdate()
    {
        if (target == null) return;

        Vector3 desiredPosition = transform.position;

        // Kamera arkadan sabit aksiste gelsin, karakterin görsel dönüşünden etkilenmesin
        desiredPosition.z = target.position.z - distance;
        desiredPosition.y = target.position.y + height;

        if (followLaneX)
            desiredPosition.x = target.position.x;

        transform.position = Vector3.Lerp(
            transform.position,
            desiredPosition,
            followSpeed * Time.deltaTime
        );

        Vector3 lookTarget = new Vector3(
            followLaneX ? target.position.x : transform.position.x,
            target.position.y + lookHeight,
            target.position.z
        );

        Quaternion targetRotation = Quaternion.LookRotation(lookTarget - transform.position);

        transform.rotation = Quaternion.Lerp(
            transform.rotation,
            targetRotation,
            rotationSpeed * Time.deltaTime
        );
    }
}
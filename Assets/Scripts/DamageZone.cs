using UnityEngine;
using System.Collections;

public class DamageZone : MonoBehaviour
{
    [SerializeField] private int damageAmount = 1;
    [SerializeField] private float hitCooldown = 0.5f;
    [SerializeField] private bool disableZoneAfterHit = true;

    private bool canHit = true;

    private void OnTriggerEnter(Collider other)
    {
        if (!canHit) return;

        PlayerHealth playerHealth = other.GetComponentInParent<PlayerHealth>();
        if (playerHealth == null) return;

        RunnerController runner = other.GetComponentInParent<RunnerController>();

        canHit = false;

        playerHealth.TakeDamage(damageAmount);

        if (runner != null)
            runner.TriggerHitReaction();

        ObstacleDisappear obstacleDisappear = GetComponentInParent<ObstacleDisappear>();
        if (obstacleDisappear != null)
            obstacleDisappear.BeginDisappear();

        Debug.Log("HIT!");

        if (!disableZoneAfterHit)
            StartCoroutine(HitCooldownRoutine());
        else
            gameObject.SetActive(false);
    }

    private IEnumerator HitCooldownRoutine()
    {
        yield return new WaitForSeconds(hitCooldown);
        canHit = true;
    }

    public void ResetZone()
    {
        StopAllCoroutines();
        canHit = true;

        if (disableZoneAfterHit)
            gameObject.SetActive(true);
    }
}
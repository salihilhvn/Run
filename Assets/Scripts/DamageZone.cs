using UnityEngine;

public class DamageZone : MonoBehaviour
{
    [SerializeField] private int damageAmount = 1;
    [SerializeField] private float hitCooldown = 0.5f; // tekrar vurma süresi
    [SerializeField] private bool destroyAfterHit = false;

    private bool canHit = true;

    private void OnTriggerEnter(Collider other)
    {
        if (!canHit) return;

        PlayerHealth playerHealth = other.GetComponentInParent<PlayerHealth>();
        if (playerHealth == null) return;

        playerHealth.TakeDamage(damageAmount);
        Debug.Log("HIT!");

        if (destroyAfterHit)
        {
            Destroy(gameObject);
        }
        else
        {
            StartCoroutine(HitCooldownRoutine());
        }
    }

    private System.Collections.IEnumerator HitCooldownRoutine()
    {
        canHit = false;
        yield return new WaitForSeconds(hitCooldown);
        canHit = true;
    }
}
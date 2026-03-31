using UnityEngine;
using System.Collections;

public class ObstacleDisappear : MonoBehaviour
{
    [Header("Dissolve")]
    [SerializeField] private Renderer[] targetRenderers;
    [SerializeField] private string dissolvePropertyName = "_DissolveAmount";
    [SerializeField] private float dissolveDuration = 1.0f;

    [Header("Disable")]
    [SerializeField] private Collider[] collidersToDisable;
    [SerializeField] private bool disableObjectAtEnd = true;

    private bool isDisappearing = false;

    private void Awake()
    {
        if (targetRenderers == null || targetRenderers.Length == 0)
            targetRenderers = GetComponentsInChildren<Renderer>(true);

        if (collidersToDisable == null || collidersToDisable.Length == 0)
            collidersToDisable = GetComponentsInChildren<Collider>(true);
    }

    public void BeginDisappear()
    {
        if (isDisappearing) return;
        StartCoroutine(DisappearRoutine());
    }

    private IEnumerator DisappearRoutine()
    {
        isDisappearing = true;

        foreach (var col in collidersToDisable)
        {
            if (col != null)
                col.enabled = false;
        }

        float time = 0f;

        while (time < dissolveDuration)
        {
            float t = time / dissolveDuration;

            foreach (var rend in targetRenderers)
            {
                if (rend == null) continue;

                foreach (var mat in rend.materials)
                {
                    if (mat.HasProperty(dissolvePropertyName))
                        mat.SetFloat(dissolvePropertyName, t);
                }
            }

            time += Time.deltaTime;
            yield return null;
        }

        foreach (var rend in targetRenderers)
        {
            if (rend == null) continue;

            foreach (var mat in rend.materials)
            {
                if (mat.HasProperty(dissolvePropertyName))
                    mat.SetFloat(dissolvePropertyName, 1f);
            }
        }

        if (disableObjectAtEnd)
            gameObject.SetActive(false);
    }

    public void ResetObstacle()
    {
        StopAllCoroutines();
        isDisappearing = false;

        gameObject.SetActive(true);

        foreach (var col in collidersToDisable)
        {
            if (col != null)
                col.enabled = true;
        }

        foreach (var rend in targetRenderers)
        {
            if (rend == null) continue;

            foreach (var mat in rend.materials)
            {
                if (mat.HasProperty(dissolvePropertyName))
                    mat.SetFloat(dissolvePropertyName, 0f);
            }
        }

        DamageZone[] damageZones = GetComponentsInChildren<DamageZone>(true);
        foreach (var zone in damageZones)
        {
            if (zone != null)
                zone.ResetZone();
        }
    }
}
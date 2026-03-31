using UnityEngine;

public class HitAnimationRelay : MonoBehaviour
{
    [SerializeField] private RunnerController runnerController;

    private void Awake()
    {
        if (runnerController == null)
            runnerController = GetComponentInParent<RunnerController>();
    }

    public void EndHitReaction()
    {
        if (runnerController != null)
            runnerController.EndHitReaction();
    }
}
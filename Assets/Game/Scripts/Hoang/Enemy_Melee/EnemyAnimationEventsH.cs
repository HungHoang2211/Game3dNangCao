using UnityEngine;

public class EnemyAnimationEventsH : MonoBehaviour
{
    private Enemy_Hoang enemy;

    private void Awake()
    {
        enemy = GetComponentInParent<Enemy_Hoang>();
    }

    public void AnimationTrigger() => enemy.AnimationTrigger();

    public void StartManualMovement() => enemy.ActivateManualMovement(true);
    public void StopManualMovement() => enemy.ActivateManualMovement(false);

    public void StartManualRotation() => enemy.ActivateManualRotation(true);
    public void StopManualRotation() => enemy.ActivateManualRotation(false);

    public void AbilityEvent() => enemy.AbilityTrigger();
}

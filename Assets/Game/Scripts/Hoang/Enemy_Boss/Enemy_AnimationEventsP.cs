using UnityEngine;

public class Enemy_AnimationEventsP : MonoBehaviour
{
    private Enemy_Phat enemy;
    private Enemy_BossP enemyBoss;

    private void Awake()
    {
        enemy = GetComponentInParent<Enemy_Phat>();
    }

    public void AnimationTrigger() => enemy.AnimationTrigger();

    public void StartManualMovement() => enemy.ActivateManualMovement(true);

    public void StopManualMovement() => enemy.ActivateManualMovement(false);

    public void StartManualRotation() => enemy.ActivateManualRotation(true);
    public void StopManualRotation() => enemy.ActivateManualRotation(false);

    public void AbilityEvent() => enemy.AbilityTrigger();

    public void BossJumpImpact()
    {
        if (enemyBoss == null)
            enemyBoss = GetComponentInParent<Enemy_BossP>();

        enemyBoss?.JumpImpact();
    }
}

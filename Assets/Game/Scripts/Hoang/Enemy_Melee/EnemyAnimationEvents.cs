using UnityEngine;

public class EnemyAnimationEvents : MonoBehaviour
{
    private Enemy_Hoang enemy;
    private void Awake()
    {
        enemy = GetComponentInParent<Enemy_Hoang>();
    }
    public void AnimationTrigger() => enemy.AnimationTrigger();
    public void StartManualMovement() => enemy.ActivateManualMovement(true);
    public void StopManualMovement() => enemy.ActivateManualMovement(false);
}

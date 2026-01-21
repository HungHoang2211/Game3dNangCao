using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAttack : MonoBehaviour
{
    public float attackRange = 1.5f;
    public int damage = 10;
    public float attackCooldown = 0.5f;
    public Transform attackPoint;
    public LayerMask enemyLayer;

    float nextAttackTime;

    public void OnAttack()
    {
        if (Time.time < nextAttackTime) return;
        nextAttackTime = Time.time + attackCooldown;

        Collider[] hits = Physics.OverlapSphere(
            attackPoint.position,
            attackRange,
            enemyLayer
        );

        foreach (Collider hit in hits)
        {
            hit.SendMessage("TakeDamage", damage, SendMessageOptions.DontRequireReceiver);
        }

        Debug.Log("Attack!");
    }

    void OnDrawGizmosSelected()
    {
        if (attackPoint == null) return;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }
}

using UnityEngine;

public class Shield_Hoang : MonoBehaviour
{
    private Enemy_MeleeH enemy;
    [SerializeField] private int durability;

    private void Awake()
    {
        enemy = GetComponentInParent<Enemy_MeleeH>();
    }

    public void ReduceDurability()
    {
        durability--;

        if (durability <= 0)
        {
            enemy.anim.SetFloat("ChaseIndex", 0); // Enables default chase animation
            Destroy(gameObject);
        }
    }
}
using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    public int hp = 30;

    public void TakeDamage(int damage)
    {
        hp -= damage;
        Debug.Log("Emeny HP: " + hp);
        if (hp <= 0)
        {
            Destroy(transform.root.gameObject);
            Debug.Log("Enemy Dead!");
        }
    }
}

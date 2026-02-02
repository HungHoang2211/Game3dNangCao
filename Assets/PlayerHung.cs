using UnityEngine;
using System.Collections;

public class PlayerHung : MonoBehaviour, IDamageableHung
{
    [SerializeField]
    private int Health = 300;
    public void TakeDamage(int Damage)
    {
        Health -= Damage;

        if (Health <= 0)
        {
            gameObject.SetActive(false);
        }
    }

    public Transform GetTransform()
    {
        return transform;
    }
}

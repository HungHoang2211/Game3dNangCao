using UnityEngine;

public class WeaponStat : MonoBehaviour
{
    public string weaponName;
    public int damage = 10;
    public float attackSpeed = 1.5f;
    public float attackRange = 3f;
    public virtual void Attack()
    {
        Debug.Log("Weapon attack");
    }
}

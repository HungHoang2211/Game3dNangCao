using UnityEngine;

[CreateAssetMenu(fileName = "WeaponStatus", menuName = "Scriptable Objects/WeaponStatus")]
public class WeaponStatus : ScriptableObject
{
    [Header("Weapon Infor")]
    public string weaponName;
    
    [Header("Weapon Stat")]
    public float attackSpeed = 1.5f;
    public float attackRange = 3f;
    public int damage = 10;

    [Header("Weapon Prefab")]
    public GameObject weaponPrefab;


}

using UnityEngine;

[CreateAssetMenu(fileName = "ItemStatus", menuName = "Scriptable Objects/ItemStatus")]
public class ItemStatus : ScriptableObject
{
    public string itemName;
    public Sprite itemIcon;
    public int damage;
    public int hp;
    public float attackSpeed;
    public float attackRange;
    public GameObject itemPrefab;
}
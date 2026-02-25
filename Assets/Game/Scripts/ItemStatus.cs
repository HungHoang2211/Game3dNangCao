using UnityEngine;

[CreateAssetMenu(fileName = "ItemStatus", menuName = "Scriptable Objects/ItemStatus")]
public class ItemStatus : ScriptableObject
{
    [Header("Item Infor")]
    public string itemName;
    public Sprite itemIcon;

    [Header("Item Stat")]
    public float attackSpeed = 1.5f;
    public float attackRange = 3f;
    public int damage = 10;
    public int hp;

    [Header("Item Prefab")]
    public GameObject itemPrefab;


}

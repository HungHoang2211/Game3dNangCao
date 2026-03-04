using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Attach to enemy prefab. Spawns loot on ground when enemy dies.
/// 
/// FIXES:
/// - Caches position BEFORE enemy Destroy
/// - Uses Raycast to place items ON the ground (no falling)
/// - No physics needed on the drop prefab
/// 
/// SETUP:
/// 1. Add component to enemy prefab
/// 2. Assign LootTable (Create via Assets > Create > Loot > Loot Table)
/// 3. Assign droppedItemPrefab (empty prefab with DroppedItemWorld only)
/// </summary>
public class EnemyLootDrop : MonoBehaviour
{
    [Header("Loot Configuration")]
    [SerializeField] private LootTable lootTable;

    [Header("Drop Prefab")]
    [Tooltip("Empty prefab with DroppedItemWorld script ONLY - visuals are auto-created")]
    [SerializeField] private GameObject droppedItemPrefab;

    [Header("Drop Settings")]
    [SerializeField] private float dropRadius = 1.5f;
    [SerializeField] private float dropHeightAboveGround = 0.3f;

    private bool hasDropped = false;

    public void DropLoot()
    {
        if (hasDropped) return;
        hasDropped = true;

        if (lootTable == null || droppedItemPrefab == null)
        {
            Debug.LogWarning("[EnemyLootDrop] Missing LootTable or Prefab!");
            return;
        }

        // Cache position NOW before enemy gets destroyed
        Vector3 deathPosition = transform.position;

        List<LootResult> results = lootTable.RollLoot();

        if (results.Count == 0)
        {
            Debug.Log("[EnemyLootDrop] No loot dropped.");
            return;
        }

        for (int i = 0; i < results.Count; i++)
        {
            SpawnDrop(results[i], i, results.Count, deathPosition);
        }
    }

    private void SpawnDrop(LootResult loot, int index, int total, Vector3 deathPos)
    {
        // Spread items in circle
        Vector3 offset = Vector3.zero;
        if (total > 1)
        {
            float angle = (360f / total) * index;
            float rad = angle * Mathf.Deg2Rad;
            offset = new Vector3(Mathf.Cos(rad), 0f, Mathf.Sin(rad)) * dropRadius * 0.5f;
        }

        // Raycast DOWN to find ground surface
        Vector3 rayOrigin = deathPos + offset + Vector3.up * 10f;
        Vector3 spawnPos = deathPos + offset;

        if (Physics.Raycast(rayOrigin, Vector3.down, out RaycastHit hit, 30f))
        {
            spawnPos = hit.point + Vector3.up * dropHeightAboveGround;
        }

        // Instantiate at ground position
        GameObject dropObj = Instantiate(droppedItemPrefab, spawnPos, Quaternion.identity);

        DroppedItemWorld dropped = dropObj.GetComponent<DroppedItemWorld>();
        if (dropped != null)
        {
            dropped.Initialize(loot.item, loot.quantity);
        }
        else
        {
            Debug.LogError("[EnemyLootDrop] Prefab missing DroppedItemWorld!");
            Destroy(dropObj);
        }
    }
}

using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Attach to any enemy prefab to enable loot drops on death.
/// Works with IEnemy system - listens for death and spawns DroppedItemWorld objects.
/// 
/// SETUP:
/// 1. Add this component to enemy prefab (alongside EnemyKienAdapter/EnemyVuAdapter/etc.)
/// 2. Assign a LootTable (create via Assets > Create > Loot > Loot Table)
/// 3. Assign droppedItemPrefab (a prefab with DroppedItemWorld component)
/// 4. Enemy dies > loot spawns on ground > player walks over to pick up
/// </summary>
public class EnemyLootDrop : MonoBehaviour
{
    [Header("Loot Configuration")]
    [Tooltip("Loot table defining what this enemy drops")]
    [SerializeField] private LootTable lootTable;

    [Header("Drop Prefab")]
    [Tooltip("Prefab with DroppedItemWorld component (the 3D pickup object)")]
    [SerializeField] private GameObject droppedItemPrefab;

    [Header("Drop Spread")]
    [Tooltip("Items scatter within this radius from enemy position")]
    [SerializeField] private float dropRadius = 1.5f;

    [Tooltip("Height offset when spawning drops")]
    [SerializeField] private float dropHeightOffset = 0.5f;

    [Header("Drop Animation")]
    [Tooltip("Force to launch items upward")]
    [SerializeField] private float launchForce = 3f;

    [Tooltip("Random horizontal spread force")]
    [SerializeField] private float spreadForce = 1.5f;

    private bool hasDropped = false;

    /// <summary>
    /// Call this when enemy dies to spawn loot.
    /// Typically called from your enemy adapter's Die() method.
    /// </summary>
    public void DropLoot()
    {
        if (hasDropped) return; // Prevent double drops
        hasDropped = true;

        if (lootTable == null)
        {
            Debug.LogWarning($"[EnemyLootDrop] No LootTable assigned on {gameObject.name}!");
            return;
        }

        if (droppedItemPrefab == null)
        {
            Debug.LogError($"[EnemyLootDrop] No droppedItemPrefab assigned on {gameObject.name}!");
            return;
        }

        // Roll loot
        List<LootResult> results = lootTable.RollLoot();

        if (results.Count == 0)
        {
            Debug.Log($"[EnemyLootDrop] {gameObject.name} dropped nothing this time.");
            return;
        }

        Debug.Log($"[EnemyLootDrop] {gameObject.name} dropping {results.Count} item(s)!");

        // Spawn each dropped item
        for (int i = 0; i < results.Count; i++)
        {
            SpawnDroppedItem(results[i], i, results.Count);
        }
    }

    private void SpawnDroppedItem(LootResult loot, int index, int totalDrops)
    {
        // Calculate spawn position with spread
        Vector3 spawnPos = transform.position + Vector3.up * dropHeightOffset;

        // Spread items in a circle if multiple drops
        if (totalDrops > 1)
        {
            float angle = (360f / totalDrops) * index;
            float rad = angle * Mathf.Deg2Rad;
            Vector3 offset = new Vector3(Mathf.Cos(rad), 0f, Mathf.Sin(rad)) * dropRadius * 0.5f;
            spawnPos += offset;
        }

        // Instantiate the pickup object
        GameObject dropObj = Instantiate(droppedItemPrefab, spawnPos, Quaternion.identity);

        // Initialize with item data
        DroppedItemWorld droppedItem = dropObj.GetComponent<DroppedItemWorld>();

        if (droppedItem != null)
        {
            droppedItem.Initialize(loot.item, loot.quantity);
        }
        else
        {
            Debug.LogError("[EnemyLootDrop] droppedItemPrefab missing DroppedItemWorld component!");
            Destroy(dropObj);
            return;
        }

        // Apply physics launch (if has Rigidbody)
        Rigidbody rb = dropObj.GetComponent<Rigidbody>();
        if (rb != null)
        {
            Vector3 randomDir = new Vector3(
                Random.Range(-spreadForce, spreadForce),
                launchForce,
                Random.Range(-spreadForce, spreadForce)
            );
            rb.AddForce(randomDir, ForceMode.Impulse);
        }

        Debug.Log($"[EnemyLootDrop] Spawned {loot.quantity}x {loot.item.itemName} at {spawnPos}");
    }
}

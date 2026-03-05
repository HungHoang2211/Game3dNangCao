using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Spawns enemies and maintains max count per type.
/// When an enemy dies, spawner detects it and respawns after a delay.
///
/// SETUP:
/// 1. Create empty GameObject named "EnemySpawner"
/// 2. Add this script
/// 3. Add entries to spawnEntries: assign prefab, max count, spawn area size
/// 4. Position the spawner where you want enemies to appear
/// 5. Play - enemies spawn and respawn automatically
/// </summary>
public class EnemySpawner : MonoBehaviour
{
    [Header("Spawn Entries")]
    public List<SpawnEntry> spawnEntries = new List<SpawnEntry>();

    [Header("Spawn Area")]
    [Tooltip("Enemies spawn randomly within this radius from spawner position")]
    [SerializeField] private float spawnRadius = 10f;

    [Tooltip("Check for ground with raycast")]
    [SerializeField] private float groundCheckHeight = 20f;

    [Header("Timing")]
    [Tooltip("Delay before respawning a dead enemy")]
    [SerializeField] private float respawnDelay = 5f;

    [Tooltip("Time between each individual spawn (prevents all spawning at once)")]
    [SerializeField] private float spawnInterval = 0.5f;

    // Runtime tracking
    private Dictionary<int, List<GameObject>> aliveEnemies = new Dictionary<int, List<GameObject>>();
    private float nextSpawnTime = 0f;

    // ============================================
    // INITIALIZATION
    // ============================================

    private void Start()
    {
        // Initialize tracking for each entry
        for (int i = 0; i < spawnEntries.Count; i++)
        {
            aliveEnemies[i] = new List<GameObject>();
        }

        // Initial spawn - fill up to max
        for (int i = 0; i < spawnEntries.Count; i++)
        {
            SpawnEntry entry = spawnEntries[i];
            for (int j = 0; j < entry.maxCount; j++)
            {
                SpawnEnemy(i);
            }
        }
    }

    // ============================================
    // UPDATE - CHECK DEAD AND RESPAWN
    // ============================================

    private void Update()
    {
        if (Time.time < nextSpawnTime) return;

        for (int i = 0; i < spawnEntries.Count; i++)
        {
            // Clean up destroyed/dead enemies from list
            CleanDeadEnemies(i);

            // Check if need to spawn more
            int alive = aliveEnemies[i].Count;
            int max = spawnEntries[i].maxCount;

            if (alive < max)
            {
                SpawnEnemy(i);
                nextSpawnTime = Time.time + spawnInterval;

                // Only spawn one per frame to spread out spawns
                return;
            }
        }
    }

    /// <summary>
    /// Remove null (destroyed) entries from alive list
    /// </summary>
    private void CleanDeadEnemies(int entryIndex)
    {
        List<GameObject> list = aliveEnemies[entryIndex];

        for (int i = list.Count - 1; i >= 0; i--)
        {
            if (list[i] == null)
            {
                list.RemoveAt(i);
            }
        }
    }

    // ============================================
    // SPAWN
    // ============================================

    private void SpawnEnemy(int entryIndex)
    {
        SpawnEntry entry = spawnEntries[entryIndex];

        if (entry.prefab == null)
        {
            Debug.LogWarning("[EnemySpawner] Prefab is null!");
            return;
        }

        // Random position within radius
        Vector3 spawnPos = GetRandomSpawnPosition();

        // Spawn
        GameObject enemy = Instantiate(entry.prefab, spawnPos, Quaternion.identity);

        // Track it
        aliveEnemies[entryIndex].Add(enemy);

        int alive = aliveEnemies[entryIndex].Count;
        Debug.Log($"[EnemySpawner] Spawned {entry.prefab.name} ({alive}/{entry.maxCount})");
    }

    /// <summary>
    /// Get random position on ground within spawn radius
    /// </summary>
    private Vector3 GetRandomSpawnPosition()
    {
        Vector2 randomCircle = Random.insideUnitCircle * spawnRadius;
        Vector3 randomPos = transform.position + new Vector3(randomCircle.x, 0f, randomCircle.y);

        // Raycast to find ground
        Vector3 rayOrigin = randomPos + Vector3.up * groundCheckHeight;

        if (Physics.Raycast(rayOrigin, Vector3.down, out RaycastHit hit, groundCheckHeight * 2f))
        {
            return hit.point;
        }

        // Fallback: use spawner Y position
        randomPos.y = transform.position.y;
        return randomPos;
    }

    // ============================================
    // GIZMOS - VISUALIZE SPAWN AREA IN EDITOR
    // ============================================

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(1f, 0f, 0f, 0.3f);
        Gizmos.DrawWireSphere(transform.position, spawnRadius);

        Gizmos.color = new Color(1f, 1f, 0f, 0.1f);
        Gizmos.DrawSphere(transform.position, spawnRadius);
    }
}

/// <summary>
/// Configuration for one enemy type in the spawner
/// </summary>
[System.Serializable]
public class SpawnEntry
{
    [Tooltip("Enemy prefab to spawn")]
    public GameObject prefab;

    [Tooltip("Maximum alive at the same time")]
    public int maxCount = 10;
}

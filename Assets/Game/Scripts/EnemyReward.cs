using UnityEngine;

/// <summary>
/// Attach to enemy prefab alongside adapter.
/// When enemy dies: gives EXP to player + spawns gold pile on ground.
///
/// SETUP:
/// 1. Add to enemy prefab
/// 2. Set expReward, minGold, maxGold
/// 3. Assign goldDropPrefab (empty GameObject with DroppedGold script)
/// 4. In enemy adapter Die(): call GetComponent<EnemyReward>().GiveReward()
/// </summary>
public class EnemyReward : MonoBehaviour
{
    [Header("EXP Reward")]
    [Tooltip("EXP given to player when this enemy dies")]
    [SerializeField] private int expReward = 25;

    [Header("Gold Drop")]
    [Tooltip("Minimum gold dropped")]
    [SerializeField] private int minGold = 5;

    [Tooltip("Maximum gold dropped")]
    [SerializeField] private int maxGold = 20;

    [Tooltip("Prefab with DroppedGold script")]
    [SerializeField] private GameObject goldDropPrefab;

    [Header("Drop Position")]
    [SerializeField] private float dropHeightAboveGround = 0.3f;

    private bool hasGivenReward = false;

    /// <summary>
    /// Call from enemy adapter Die() method
    /// </summary>
    public void GiveReward()
    {
        if (hasGivenReward) return;
        hasGivenReward = true;

        GiveExp();
        SpawnGoldDrop();
    }

    private void GiveExp()
    {
        if (expReward <= 0) return;

        PlayerStats playerStats = Object.FindAnyObjectByType<PlayerStats>();

        if (playerStats != null)
        {
            playerStats.AddExp(expReward);
            Debug.Log($"[EnemyReward] Player gained {expReward} EXP from {gameObject.name}");
        }
        else
        {
            Debug.LogWarning("[EnemyReward] PlayerStats not found!");
        }
    }

    private void SpawnGoldDrop()
    {
        int goldAmount = Random.Range(minGold, maxGold + 1);

        if (goldAmount <= 0) return;

        if (goldDropPrefab == null)
        {
            // No prefab assigned - just add gold directly
            if (CurrencyManager.Instance != null)
            {
                CurrencyManager.Instance.AddGold(goldAmount);
                Debug.Log($"[EnemyReward] Direct gold: +{goldAmount}");
            }
            return;
        }

        // Cache position before enemy is destroyed
        Vector3 deathPos = transform.position;

        // Raycast to find ground
        Vector3 rayOrigin = deathPos + Vector3.up * 10f;
        Vector3 spawnPos = deathPos;

        if (Physics.Raycast(rayOrigin, Vector3.down, out RaycastHit hit, 30f))
        {
            spawnPos = hit.point + Vector3.up * dropHeightAboveGround;
        }

        // Spawn gold pile
        GameObject goldObj = Instantiate(goldDropPrefab, spawnPos, Quaternion.identity);

        DroppedGold dropped = goldObj.GetComponent<DroppedGold>();
        if (dropped != null)
        {
            dropped.Initialize(goldAmount);
        }
        else
        {
            Debug.LogError("[EnemyReward] goldDropPrefab missing DroppedGold script!");
            Destroy(goldObj);
        }
    }
}

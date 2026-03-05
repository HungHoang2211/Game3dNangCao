using UnityEngine;

/// <summary>
/// Gold pile pickup in 3D world. Auto-creates coin visual.
/// Player walks into trigger to collect gold.
///
/// PREFAB SETUP:
/// 1. Create empty GameObject "GoldDropPrefab"
/// 2. Add this DroppedGold script
/// 3. (Optional) Add a child with coin model/sprite - if not, script creates yellow cylinder
/// 4. Save as Prefab. DONE.
/// </summary>
public class DroppedGold : MonoBehaviour
{
    [Header("Gold Data (Set at runtime)")]
    private int goldAmount = 0;

    [Header("Pickup Settings")]
    [SerializeField] private string playerTag = "Player";
    [SerializeField] private float pickupRadius = 2f;
    [SerializeField] private float pickupDelay = 0.3f;
    [SerializeField] private float lifetime = 120f;

    [Header("Visual Settings")]
    [SerializeField] private float floatAmplitude = 0.1f;
    [SerializeField] private float floatSpeed = 3f;
    [SerializeField] private float rotateSpeed = 120f;

    [Header("Custom Visual (Optional)")]
    [Tooltip("If assigned, uses this as visual instead of auto-generated coin")]
    [SerializeField] private GameObject customVisual;

    private float spawnTime;
    private bool isPickedUp = false;
    private Vector3 groundPosition;
    private GameObject visualObj;

    // ============================================
    // INITIALIZATION
    // ============================================

    public void Initialize(int amount)
    {
        goldAmount = amount;
        spawnTime = Time.time;
        groundPosition = transform.position;

        SetupCollider();
        CreateVisual();

        gameObject.name = "GoldDrop_" + amount;

        Debug.Log($"[DroppedGold] Spawned gold pile: {amount} gold");
    }

    private void Start()
    {
        if (spawnTime == 0f)
            spawnTime = Time.time;

        if (lifetime > 0f)
            Destroy(gameObject, lifetime);
    }

    // ============================================
    // COLLIDER SETUP
    // ============================================

    private void SetupCollider()
    {
        Collider[] existing = GetComponents<Collider>();
        foreach (var c in existing)
            Destroy(c);

        SphereCollider trigger = gameObject.AddComponent<SphereCollider>();
        trigger.isTrigger = true;
        trigger.radius = pickupRadius;
    }

    // ============================================
    // VISUAL - AUTO CREATE COIN PILE
    // ============================================

    private void CreateVisual()
    {
        if (customVisual != null)
        {
            customVisual.SetActive(true);
            visualObj = customVisual;
            return;
        }

        // Auto-create: stack of 3 small yellow cylinders (coin pile)
        visualObj = new GameObject("CoinPile");
        visualObj.transform.SetParent(transform);
        visualObj.transform.localPosition = Vector3.zero;

        for (int i = 0; i < 3; i++)
        {
            GameObject coin = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            coin.transform.SetParent(visualObj.transform);

            // Stack coins with slight random offset
            float yOffset = i * 0.06f;
            float xOffset = Random.Range(-0.05f, 0.05f);
            float zOffset = Random.Range(-0.05f, 0.05f);

            coin.transform.localPosition = new Vector3(xOffset, yOffset, zOffset);
            coin.transform.localScale = new Vector3(0.25f, 0.03f, 0.25f);

            // Random slight rotation for natural look
            coin.transform.localRotation = Quaternion.Euler(0f, Random.Range(0f, 360f), 0f);

            // Remove collider from visual
            Collider col = coin.GetComponent<Collider>();
            if (col != null) Destroy(col);

            // Gold color
            Renderer rend = coin.GetComponent<Renderer>();
            if (rend != null)
            {
                rend.material.color = new Color(1f, 0.85f, 0f); // Gold yellow
            }
        }
    }

    // ============================================
    // UPDATE - FLOAT + ROTATE
    // ============================================

    private void Update()
    {
        if (isPickedUp || visualObj == null) return;

        // Float
        Vector3 pos = groundPosition;
        pos.y += Mathf.Sin(Time.time * floatSpeed) * floatAmplitude;
        transform.position = pos;

        // Rotate
        visualObj.transform.Rotate(Vector3.up, rotateSpeed * Time.deltaTime, Space.World);
    }

    // ============================================
    // PICKUP
    // ============================================

    private void OnTriggerEnter(Collider other)
    {
        TryPickup(other);
    }

    private void OnTriggerStay(Collider other)
    {
        TryPickup(other);
    }

    private void TryPickup(Collider other)
    {
        if (isPickedUp) return;
        if (goldAmount <= 0) return;
        if (Time.time - spawnTime < pickupDelay) return;
        if (!other.CompareTag(playerTag)) return;

        if (CurrencyManager.Instance == null)
        {
            Debug.LogError("[DroppedGold] CurrencyManager not found!");
            return;
        }

        isPickedUp = true;

        CurrencyManager.Instance.AddGold(goldAmount);
        Debug.Log($"[DroppedGold] Player picked up {goldAmount} gold!");

        // Show message via InventoryManager if available
        InventoryManager inventory = Object.FindAnyObjectByType<InventoryManager>();
        if (inventory != null)
        {
            inventory.ShowMessage("+" + goldAmount + " Gold");
        }

        Destroy(gameObject);
    }

    // ============================================
    // GETTER
    // ============================================

    public int GetGoldAmount() => goldAmount;
}

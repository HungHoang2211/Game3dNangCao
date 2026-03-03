using UnityEngine;
using TMPro;

/// <summary>
/// 3D world pickup item. Spawned by EnemyLootDrop when enemy dies.
/// Player walks into trigger collider to pick up into inventory.
/// 
/// PREFAB SETUP:
/// 1. Create empty GameObject, name it "DroppedItemPrefab"
/// 2. Add this script (DroppedItemWorld)
/// 3. Add SphereCollider (isTrigger = true, radius ~1.0)
/// 4. Add Rigidbody (useGravity = true, for bounce effect)
/// 5. (Optional) Add a child with SpriteRenderer or 3D model for visual
/// 6. (Optional) Add a child with TextMeshPro for quantity label
/// 7. Save as Prefab
/// 
/// HOW IT WORKS:
/// - EnemyLootDrop spawns this prefab and calls Initialize(item, qty)
/// - Item falls to ground with physics
/// - When player enters trigger -> auto pickup into InventoryManager
/// - If inventory full -> item stays on ground with warning message
/// </summary>
public class DroppedItemWorld : MonoBehaviour
{
    [Header("Item Data (Set at runtime)")]
    [SerializeField] private ItemObject item;
    [SerializeField] private int quantity = 1;

    [Header("Pickup Settings")]
    [Tooltip("Tag of the player GameObject")]
    [SerializeField] private string playerTag = "Player";

    [Tooltip("Delay before item can be picked up (prevents instant grab)")]
    [SerializeField] private float pickupDelay = 0.5f;

    [Tooltip("Auto destroy after this many seconds (0 = never)")]
    [SerializeField] private float lifetime = 60f;

    [Header("Visual (Optional)")]
    [Tooltip("SpriteRenderer to show item icon")]
    [SerializeField] private SpriteRenderer iconRenderer;

    [Tooltip("TextMeshPro for quantity display")]
    [SerializeField] private TextMeshPro quantityText;

    [Header("Floating Animation")]
    [SerializeField] private bool enableFloating = true;
    [SerializeField] private float floatAmplitude = 0.15f;
    [SerializeField] private float floatSpeed = 2f;
    [SerializeField] private float rotateSpeed = 90f;

    private float spawnTime;
    private bool isPickedUp = false;
    private Vector3 basePosition;
    private bool isGrounded = false;
    private Rigidbody rb;

    // ============================================
    // INITIALIZATION
    // ============================================

    /// <summary>
    /// Called by EnemyLootDrop after instantiation
    /// </summary>
    public void Initialize(ItemObject droppedItem, int qty)
    {
        item = droppedItem;
        quantity = qty;
        spawnTime = Time.time;

        UpdateVisual();

        Debug.Log($"[DroppedItemWorld] Initialized: {qty}x {droppedItem.itemName}");
    }

    private void Start()
    {
        spawnTime = Time.time;
        rb = GetComponent<Rigidbody>();

        if (lifetime > 0f)
        {
            Destroy(gameObject, lifetime);
        }
    }

    private void Update()
    {
        if (!enableFloating || !isGrounded) return;

        // Float up and down
        Vector3 pos = basePosition;
        pos.y += Mathf.Sin(Time.time * floatSpeed) * floatAmplitude;
        transform.position = pos;

        // Slow rotation
        transform.Rotate(Vector3.up, rotateSpeed * Time.deltaTime);
    }

    // ============================================
    // VISUAL
    // ============================================

    private void UpdateVisual()
    {
        if (item == null) return;

        // Show item icon as sprite
        if (iconRenderer != null && item.icon != null)
        {
            iconRenderer.sprite = item.icon;
        }

        // Show quantity text
        if (quantityText != null)
        {
            if (quantity > 1)
            {
                quantityText.text = $"x{quantity}";
                quantityText.gameObject.SetActive(true);
            }
            else
            {
                quantityText.gameObject.SetActive(false);
            }
        }

        gameObject.name = $"Drop_{item.itemName}_x{quantity}";
    }

    // ============================================
    // PHYSICS - DETECT LANDING
    // ============================================

    private void OnCollisionEnter(Collision collision)
    {
        if (!isGrounded)
        {
            isGrounded = true;
            basePosition = transform.position;

            if (rb != null)
            {
                rb.isKinematic = true;
            }
        }
    }

    // ============================================
    // PICKUP LOGIC
    // ============================================

    private void OnTriggerEnter(Collider other)
    {
        if (isPickedUp) return;
        if (item == null) return;
        if (Time.time - spawnTime < pickupDelay) return;
        if (!other.CompareTag(playerTag)) return;

        TryPickup(other.gameObject);
    }

    private void OnTriggerStay(Collider other)
    {
        if (isPickedUp) return;
        if (item == null) return;
        if (Time.time - spawnTime < pickupDelay) return;
        if (!other.CompareTag(playerTag)) return;

        TryPickup(other.gameObject);
    }

    private void TryPickup(GameObject player)
    {
        // Find InventoryManager
        InventoryManager inventory = player.GetComponent<InventoryManager>();

        if (inventory == null)
        {
            inventory = player.GetComponentInParent<InventoryManager>();
        }

        if (inventory == null)
        {
            inventory = Object.FindAnyObjectByType<InventoryManager>();
        }

        if (inventory == null)
        {
            Debug.LogError("[DroppedItemWorld] Cannot find InventoryManager!");
            return;
        }

        // Try to add to inventory
        bool success = inventory.AddItem(item, quantity);

        if (success)
        {
            Debug.Log($"[DroppedItemWorld] Player picked up {quantity}x {item.itemName}");

            isPickedUp = true;
            inventory.ShowMessage($"Picked up {quantity}x {item.itemName}");
            Destroy(gameObject);
        }
        else
        {
            Debug.LogWarning($"[DroppedItemWorld] Inventory full! Cannot pick up {item.itemName}");
            inventory.ShowMessage("Inventory full!");
        }
    }

    // ============================================
    // PUBLIC GETTERS
    // ============================================

    public ItemObject GetItem() => item;
    public int GetQuantity() => quantity;
}

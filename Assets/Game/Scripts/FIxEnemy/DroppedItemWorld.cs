using UnityEngine;

/// <summary>
/// Item pickup in 3D world. Auto-creates its own visual from item icon.
/// No Rigidbody needed - placed directly on ground by EnemyLootDrop.
/// 
/// PREFAB SETUP:
/// 1. Create empty GameObject named DroppedItemPrefab
/// 2. Add this DroppedItemWorld script
/// 3. Save as Prefab. DONE.
///    Script auto-adds collider and creates sprite visual at runtime.
/// </summary>
public class DroppedItemWorld : MonoBehaviour
{
    [Header("Item Data (Set at runtime - do not edit)")]
    private ItemObject item;
    private int quantity = 1;

    [Header("Pickup Settings")]
    [SerializeField] private string playerTag = "Player";
    [SerializeField] private float pickupRadius = 1.5f;
    [SerializeField] private float pickupDelay = 0.8f;
    [SerializeField] private float lifetime = 120f;

    [Header("Visual Settings")]
    [SerializeField] private float spriteScale = 0.8f;
    [SerializeField] private float floatAmplitude = 0.15f;
    [SerializeField] private float floatSpeed = 2f;
    [SerializeField] private float rotateSpeed = 60f;

    private float spawnTime;
    private bool isPickedUp = false;
    private Vector3 groundPosition;
    private GameObject visualObj;
    private SpriteRenderer spriteRenderer;

    // ============================================
    // INITIALIZATION (called by EnemyLootDrop)
    // ============================================

    public void Initialize(ItemObject droppedItem, int qty)
    {
        item = droppedItem;
        quantity = qty;
        spawnTime = Time.time;
        groundPosition = transform.position;

        SetupCollider();
        CreateVisual();

        gameObject.name = "Drop_" + droppedItem.itemName;
    }

    private void Start()
    {
        if (spawnTime == 0f)
            spawnTime = Time.time;

        if (lifetime > 0f)
            Destroy(gameObject, lifetime);
    }

    // ============================================
    // AUTO SETUP COLLIDER
    // ============================================

    private void SetupCollider()
    {
        // Remove existing colliders to avoid conflicts
        Collider[] existing = GetComponents<Collider>();
        foreach (var c in existing)
            Destroy(c);

        // Add trigger sphere for player pickup detection only
        SphereCollider trigger = gameObject.AddComponent<SphereCollider>();
        trigger.isTrigger = true;
        trigger.radius = pickupRadius;
        trigger.center = Vector3.zero;
    }

    // ============================================
    // AUTO CREATE VISUAL FROM ITEM ICON
    // ============================================

    private void CreateVisual()
    {
        if (item == null) return;

        visualObj = new GameObject("Visual");
        visualObj.transform.SetParent(transform);
        visualObj.transform.localPosition = Vector3.zero;

        if (item.icon != null)
        {
            // Use item icon as billboard sprite
            visualObj.transform.localScale = Vector3.one * spriteScale;
            spriteRenderer = visualObj.AddComponent<SpriteRenderer>();
            spriteRenderer.sprite = item.icon;
            spriteRenderer.sortingOrder = 100;
        }
        else
        {
            // Fallback: colored cube when no icon assigned
            GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cube.transform.SetParent(visualObj.transform);
            cube.transform.localPosition = Vector3.zero;
            cube.transform.localScale = Vector3.one * 0.3f;

            // Remove cube's collider (we have our own trigger)
            Collider cubeCol = cube.GetComponent<Collider>();
            if (cubeCol != null) Destroy(cubeCol);

            Renderer rend = cube.GetComponent<Renderer>();
            if (rend != null)
            {
                Color c = Color.yellow;
                if (item.itemType == ItemType.Weapon) c = Color.red;
                else if (item.itemType == ItemType.Armor) c = Color.blue;
                else if (item.itemType == ItemType.Consumable) c = Color.green;
                rend.material.color = c;
            }
        }
    }

    // ============================================
    // UPDATE: FLOAT + ROTATE + BILLBOARD
    // ============================================

    private void Update()
    {
        if (isPickedUp || visualObj == null) return;

        // Float up/down at ground position
        Vector3 pos = groundPosition;
        pos.y += Mathf.Sin(Time.time * floatSpeed) * floatAmplitude;
        transform.position = pos;

        // Rotate slowly
        visualObj.transform.Rotate(Vector3.up, rotateSpeed * Time.deltaTime, Space.World);

        // Billboard: sprite faces camera
        if (spriteRenderer != null && Camera.main != null)
        {
            visualObj.transform.LookAt(Camera.main.transform);
            visualObj.transform.Rotate(0f, 180f, 0f);
        }
    }

    // ============================================
    // PICKUP ON TRIGGER
    // ============================================

    private void OnTriggerEnter(Collider other)
    {
        AttemptPickup(other);
    }

    private void OnTriggerStay(Collider other)
    {
        AttemptPickup(other);
    }

    private void AttemptPickup(Collider other)
    {
        if (isPickedUp || item == null) return;
        if (Time.time - spawnTime < pickupDelay) return;
        if (!other.CompareTag(playerTag)) return;

        InventoryManager inventory = other.GetComponent<InventoryManager>();
        if (inventory == null)
            inventory = other.GetComponentInParent<InventoryManager>();
        if (inventory == null)
            inventory = Object.FindAnyObjectByType<InventoryManager>();
        if (inventory == null)
        {
            Debug.LogError("[DroppedItemWorld] InventoryManager not found!");
            return;
        }

        bool success = inventory.AddItem(item, quantity);

        if (success)
        {
            isPickedUp = true;
            inventory.ShowMessage("+" + quantity + " " + item.itemName);
            Destroy(gameObject);
        }
        else
        {
            inventory.ShowMessage("Inventory full!");
        }
    }

    public ItemObject GetItem() => item;
    public int GetQuantity() => quantity;
}

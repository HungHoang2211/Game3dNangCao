using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// FireBall skill that upgrades with player level.
/// Higher level = more projectiles in a spread pattern.
///
/// Level 0-4:  1 fireball
/// Level 5-9:  3 fireballs (fan spread)
/// Level 10+:  5 fireballs (wider fan)
///
/// Customize thresholds in Inspector.
/// </summary>
public class PlayerFireBall : MonoBehaviour
{
    [Header("FireBall")]
    public GameObject fireBallPrefab;
    public Transform firePoint;

    [Header("Cooldown")]
    public float cooldown = 8f;

    [Header("Skill Upgrade Thresholds")]
    [Tooltip("Level required for 3 fireballs")]
    [SerializeField] private int level3Balls = 5;

    [Tooltip("Level required for 5 fireballs")]
    [SerializeField] private int level5Balls = 10;

    [Header("Spread Settings")]
    [Tooltip("Angle between each fireball (degrees)")]
    [SerializeField] private float spreadAngle = 15f;

    [Header("Cooldown UI (Optional)")]
    [SerializeField] private Image cooldownOverlay;
    [SerializeField] private TextMeshProUGUI cooldownText;
    [SerializeField] private Button fireButton;

    [Header("Skill Level UI (Optional)")]
    [SerializeField] private TextMeshProUGUI skillLevelText;

    private float cooldownTimer = 0f;
    private bool isOnCooldown = false;
    private int currentProjectileCount = 1;

    // References
    private ExperienceManager experienceManager;
    private PlayerStats playerStats;

    // ============================================
    // INITIALIZATION
    // ============================================

    private void Start()
    {
        if (fireButton != null)
            fireButton.onClick.AddListener(FireButton);

        // Find references
        experienceManager = FindAnyObjectByType<ExperienceManager>();
        playerStats = FindAnyObjectByType<PlayerStats>();

        // Listen for level up
        if (playerStats != null)
            playerStats.OnLevelUp += OnLevelUp;

        // Calculate initial projectile count
        UpdateProjectileCount(GetCurrentLevel());
    }

    private void OnDestroy()
    {
        if (playerStats != null)
            playerStats.OnLevelUp -= OnLevelUp;
    }

    // ============================================
    // LEVEL UP
    // ============================================

    private void OnLevelUp(int newLevel)
    {
        int oldCount = currentProjectileCount;
        UpdateProjectileCount(newLevel);

        if (currentProjectileCount > oldCount)
        {
            Debug.Log($"[PlayerFireBall] SKILL UPGRADED! Now shoots {currentProjectileCount} fireballs!");

            InventoryManager inv = FindAnyObjectByType<InventoryManager>();
            if (inv != null)
                inv.ShowMessage($"FireBall upgraded! x{currentProjectileCount}");
        }
    }

    private void UpdateProjectileCount(int level)
    {
        if (level >= level5Balls)
            currentProjectileCount = 5;
        else if (level >= level3Balls)
            currentProjectileCount = 3;
        else
            currentProjectileCount = 1;

        // Update UI
        if (skillLevelText != null)
            skillLevelText.text = "x" + currentProjectileCount;
    }

    private int GetCurrentLevel()
    {
        if (experienceManager != null)
            return experienceManager.GetCurrentLevel();

        if (playerStats != null)
            return playerStats.GetCurrentLevel();

        return 0;
    }

    // ============================================
    // UPDATE COOLDOWN
    // ============================================

    void Update()
    {
        if (!isOnCooldown) return;

        cooldownTimer -= Time.deltaTime;

        if (cooldownTimer <= 0f)
        {
            isOnCooldown = false;
            cooldownTimer = 0f;

            if (fireButton != null) fireButton.interactable = true;
            if (cooldownOverlay != null) cooldownOverlay.fillAmount = 0f;
            if (cooldownText != null) cooldownText.text = "";
        }
        else
        {
            if (cooldownOverlay != null)
                cooldownOverlay.fillAmount = cooldownTimer / cooldown;

            if (cooldownText != null)
                cooldownText.text = Mathf.CeilToInt(cooldownTimer).ToString();
        }
    }

    // ============================================
    // FIRE
    // ============================================

    public void FireButton()
    {
        if (isOnCooldown) return;

        Shoot();
        StartCooldown();
    }

    void Shoot()
    {
        if (fireBallPrefab == null || firePoint == null) return;

        if (currentProjectileCount <= 1)
        {
            // Single fireball - straight ahead
            Instantiate(fireBallPrefab, firePoint.position, firePoint.rotation);
        }
        else
        {
            // Multiple fireballs in fan spread
            float totalSpread = spreadAngle * (currentProjectileCount - 1);
            float startAngle = -totalSpread / 2f;

            for (int i = 0; i < currentProjectileCount; i++)
            {
                float angle = startAngle + (spreadAngle * i);
                Quaternion rotation = firePoint.rotation * Quaternion.Euler(0f, angle, 0f);

                Instantiate(fireBallPrefab, firePoint.position, rotation);
            }
        }

        Debug.Log($"[PlayerFireBall] Fired {currentProjectileCount} fireball(s)!");
    }

    void StartCooldown()
    {
        isOnCooldown = true;
        cooldownTimer = cooldown;

        if (fireButton != null) fireButton.interactable = false;
        if (cooldownOverlay != null) cooldownOverlay.fillAmount = 1f;
    }

    // ============================================
    // PUBLIC
    // ============================================

    public int GetProjectileCount() => currentProjectileCount;
}

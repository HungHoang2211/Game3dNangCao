using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerFireBall : MonoBehaviour
{
    [Header("FireBall")]
    public GameObject fireBallPrefab;
    public Transform firePoint;

    [Header("Sound")]
    public AudioSource audioSource;
    public AudioClip fireBallSound;

    [Header("Cooldown")]
    [Tooltip("Seconds between each fireball")]
    public float cooldown = 8f;

    [Header("Cooldown UI (Optional)")]
    [Tooltip("Image with Fill type set to Filled for cooldown overlay")]
    [SerializeField] private Image cooldownOverlay;

    [Tooltip("Text showing remaining seconds")]
    [SerializeField] private TextMeshProUGUI cooldownText;

    [Tooltip("The fire button to disable during cooldown")]
    [SerializeField] private Button fireButton;

    private float cooldownTimer = 0f;
    private bool isOnCooldown = false;

    void Update()
    {
        if (!isOnCooldown) return;

        cooldownTimer -= Time.deltaTime;

        if (cooldownTimer <= 0f)
        {
            // Cooldown finished
            isOnCooldown = false;
            cooldownTimer = 0f;

            if (fireButton != null) fireButton.interactable = true;
            if (cooldownOverlay != null) cooldownOverlay.fillAmount = 0f;
            if (cooldownText != null) cooldownText.text = "";
        }
        else
        {
            // Update UI
            if (cooldownOverlay != null)
                cooldownOverlay.fillAmount = cooldownTimer / cooldown;

            if (cooldownText != null)
                cooldownText.text = Mathf.CeilToInt(cooldownTimer).ToString();
        }
    }

    public void FireButton()
    {
        if (isOnCooldown) return;

        Shoot();
        StartCooldown();
    }

    void Shoot()
    {
        if (fireBallPrefab == null || firePoint == null) return;

        Instantiate(fireBallPrefab, firePoint.position, firePoint.rotation);

        if (audioSource != null && fireBallSound != null)
        {
            audioSource.PlayOneShot(fireBallSound);
        }

        Debug.Log("[PlayerFireBall] Fire!");
    }

    void StartCooldown()
    {
        isOnCooldown = true;
        cooldownTimer = cooldown;

        if (fireButton != null) fireButton.interactable = false;
        if (cooldownOverlay != null) cooldownOverlay.fillAmount = 1f;
    }
}

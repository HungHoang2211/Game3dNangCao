using UnityEngine;

/// <summary>
/// Portal that loads a new scene when player enters.
/// Appears only when quest is completed.
/// Player data preserved via GameDataPersist.
///
/// SETUP:
/// 1. Create portal GameObject with collider (Is Trigger = true)
/// 2. Add this script
/// 3. Set targetSceneName (must be in Build Settings!)
/// 4. Assign portalVisual (child VFX)
/// </summary>
public class Portal : MonoBehaviour
{
    [Header("Target Scene")]
    [Tooltip("Scene name to load (must be in Build Settings)")]
    [SerializeField] private string targetSceneName = "Map2";

    [Header("Visual")]
    [Tooltip("Portal VFX - hidden until quest complete")]
    [SerializeField] private GameObject portalVisual;

    [Header("Settings")]
    [SerializeField] private bool requireQuest = true;

    private bool isActive = false;

    private void Start()
    {
        SetPortalActive(false);

        if (requireQuest && QuestManager.Instance != null)
        {
            if (QuestManager.Instance.IsQuestCompleted())
            {
                ActivatePortal();
            }
            else
            {
                QuestManager.Instance.OnQuestCompleted += ActivatePortal;
            }
        }
        else if (!requireQuest)
        {
            SetPortalActive(true);
        }
    }

    private void OnDestroy()
    {
        if (QuestManager.Instance != null)
            QuestManager.Instance.OnQuestCompleted -= ActivatePortal;
    }

    private void ActivatePortal()
    {
        SetPortalActive(true);
        Debug.Log($"[Portal] Portal opened! Leads to {targetSceneName}");
    }

    private void SetPortalActive(bool active)
    {
        isActive = active;

        if (portalVisual != null)
            portalVisual.SetActive(active);

        Collider col = GetComponent<Collider>();
        if (col != null)
            col.enabled = active;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!isActive) return;
        if (!other.CompareTag("Player")) return;

        Debug.Log($"[Portal] Loading scene: {targetSceneName}");

        if (GameDataPersist.Instance != null)
        {
            GameDataPersist.Instance.LoadSceneWithData(targetSceneName);
        }
        else
        {
            Debug.LogError("[Portal] GameDataPersist not found! Data will be lost!");
            UnityEngine.SceneManagement.SceneManager.LoadScene(targetSceneName);
        }
    }
}

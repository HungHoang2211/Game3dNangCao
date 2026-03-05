using UnityEngine;
using TMPro;
using System;

/// <summary>
/// UPDATED: Added OnQuestCompleted event so portal knows when to appear.
/// Also added isCompleted flag for checking.
/// </summary>
public class QuestManager : MonoBehaviour
{
    public static QuestManager Instance;

    [Header("Quest Setting")]
    public int killTarget = 5;
    private int currentKill = 0;
    private bool isCompleted = false;

    [Header("UI")]
    public TextMeshProUGUI questText;
    public GameObject completeText;

    // Event - portal listens to this
    public event Action OnQuestCompleted;

    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        UpdateUI();

        if (completeText != null)
            completeText.SetActive(false);
    }

    public void OnEnemyKilled()
    {
        if (isCompleted) return;

        currentKill++;
        UpdateUI();

        Debug.Log("Kill: " + currentKill);

        if (currentKill >= killTarget)
        {
            CompleteQuest();
        }
    }

    void UpdateUI()
    {
        if (questText != null)
        {
            questText.text = "Kill Enemies (" + currentKill + "/" + killTarget + ")";
        }
    }

    void CompleteQuest()
    {
        if (isCompleted) return;
        isCompleted = true;

        if (completeText != null)
        {
            completeText.SetActive(true);
        }

        Debug.Log("[QuestManager] Quest completed! Portal opening...");

        // Fire event - portal will listen and appear
        OnQuestCompleted?.Invoke();
    }

    /// <summary>
    /// Check if quest is done (for portal or other scripts)
    /// </summary>
    public bool IsQuestCompleted()
    {
        return isCompleted;
    }

    /// <summary>
    /// Reset quest (called when map resets)
    /// </summary>
    public void ResetQuest()
    {
        currentKill = 0;
        isCompleted = false;
        UpdateUI();

        if (completeText != null)
            completeText.SetActive(false);

        Debug.Log("[QuestManager] Quest reset");
    }
}

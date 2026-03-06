using UnityEngine;
using TMPro;
using System;

/// <summary>
/// Quest: Kill specific boss enemy.
/// Enemy adapters call OnEnemyKilled(enemyName) when dying.
/// </summary>
public class QuestManager : MonoBehaviour
{
    public static QuestManager Instance;

    [Header("Quest Setting")]
    [Tooltip("Name to match (case-insensitive, checks Contains)")]
    public string bossName = "Boss";

    private bool isCompleted = false;

    [Header("UI")]
    public TextMeshProUGUI questText;
    public GameObject completeText;

    // Event - Portal listens to this
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

    /// <summary>
    /// Called by enemy adapter Die() with enemy GameObject name.
    /// </summary>
    public void OnEnemyKilled(string enemyName)
    {
        if (isCompleted) return;

        Debug.Log($"[QuestManager] Enemy killed: {enemyName}");

        // Check if killed enemy is the boss
        if (enemyName.ToLower().Contains(bossName.ToLower()))
        {
            CompleteQuest();
        }
    }

    // Keep old method working for non-boss enemies (no effect on quest)
    public void OnEnemyKilled()
    {
        // Does nothing for boss quest, but won't break existing enemy adapters
    }

    void UpdateUI()
    {
        if (questText != null)
        {
            if (isCompleted)
            {
                questText.text = $"Defeat {bossName} (Completed!)";
            }
            else
            {
                questText.text = $"Defeat {bossName}";
            }
        }
    }

    void CompleteQuest()
    {
        if (isCompleted) return;
        isCompleted = true;

        Debug.Log("[QuestManager] Boss defeated! Quest completed!");

        UpdateUI();

        if (completeText != null)
            completeText.SetActive(true);

        OnQuestCompleted?.Invoke();
    }

    public bool IsQuestCompleted()
    {
        return isCompleted;
    }

    public void ResetQuest()
    {
        isCompleted = false;
        UpdateUI();

        if (completeText != null)
            completeText.SetActive(false);
    }
}

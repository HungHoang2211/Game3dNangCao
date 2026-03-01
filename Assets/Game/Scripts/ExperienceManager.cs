using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ExperienceManager : MonoBehaviour
{
    [Header("Experience")]
    [SerializeField] AnimationCurve experienceCurve;

    int currentLevel, totalExperience;
    int previousLevelsExperience, nextLevelsExperience;

    [Header("Interface")]
    [SerializeField] TextMeshProUGUI levelText;
    [SerializeField] Slider experienceSlider;

    [Header("Player Stats Integration")]
    [SerializeField] private PlayerStats playerStats;

    void Start()
    {
        Debug.Log("[ExperienceManager] START");

        // Auto-find PlayerStats if not assigned
        if (playerStats == null)
        {
            playerStats = FindAnyObjectByType<PlayerStats>();

            if (playerStats != null)
            {
                Debug.Log("[ExperienceManager] Auto-found PlayerStats ✅");
            }
            else
            {
                Debug.LogError("[ExperienceManager] PlayerStats NOT FOUND! Please assign in Inspector.");
            }
        }
        else
        {
            Debug.Log($"[ExperienceManager] PlayerStats assigned: {playerStats.gameObject.name} ✅");
        }

        UpdateLevel();
    }

    void Update()
    {
        UpdateInterface();

        // DEBUG: Click to add EXP
        if (Input.GetMouseButtonDown(0))
        {
            AddExperience(5);
        }
    }

    public void AddExperience(int amount)
    {
        totalExperience += amount;
        Debug.Log($"[ExperienceManager] Added {amount} EXP. Total: {totalExperience}");

        CheckForLevelUp();
        UpdateInterface();
    }

    public void CheckForLevelUp()
    {
        if (totalExperience >= nextLevelsExperience)
        {
            currentLevel++;

            Debug.Log($"[ExperienceManager] ⬆️ LEVEL UP to {currentLevel}!");

            UpdateLevel();

            // Notify PlayerStats
            if (playerStats != null)
            {
                Debug.Log("[ExperienceManager] Notifying PlayerStats...");
                NotifyPlayerStatsLevelUp();
            }
            else
            {
                Debug.LogError("[ExperienceManager] ❌ Cannot notify PlayerStats - reference is NULL!");
            }
        }
    }

    private void NotifyPlayerStatsLevelUp()
    {
        Debug.Log($"[ExperienceManager] → Calling PlayerStats.SyncLevel({currentLevel})");
        playerStats.SyncLevel(currentLevel);
    }

    public void UpdateLevel()
    {
        previousLevelsExperience = (int)experienceCurve.Evaluate(currentLevel);
        nextLevelsExperience = (int)experienceCurve.Evaluate(currentLevel + 1);

        Debug.Log($"[ExperienceManager] Level {currentLevel} needs {nextLevelsExperience} total EXP");
    }

    void UpdateInterface()
    {
        int start = totalExperience - previousLevelsExperience;
        int end = nextLevelsExperience - previousLevelsExperience;

        if (levelText != null)
        {
            levelText.text = currentLevel.ToString();
        }

        if (experienceSlider != null)
        {
            experienceSlider.maxValue = end;
            experienceSlider.value = start;
        }
    }

    // Public getters
    public int GetCurrentLevel() => currentLevel;
    public int GetTotalExperience() => totalExperience;
    public int GetExpForNextLevel() => nextLevelsExperience - previousLevelsExperience;
}
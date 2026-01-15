using System.Collections;
using System.Collections.Generic;
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

    void Start()
    {
        UpdateLevel();
    }

    void Update()
    {
        UpdateInterface();
        if (Input.GetMouseButtonDown(0))
        {
            AddExperience(5);
            Debug.Log("Add");
        }
    }

    public void AddExperience(int amount)
    {
        totalExperience += amount;
        CheckForLevelUp();
        UpdateInterface();
    }
    public void CheckForLevelUp()
    {
        if (totalExperience >= nextLevelsExperience)
        {
            currentLevel++;
            UpdateLevel();

            // Start level up sequence... Possibly vfx?
        }
    }
    public void UpdateLevel()
    {
        previousLevelsExperience = (int)experienceCurve.Evaluate(currentLevel);
        nextLevelsExperience = (int)experienceCurve.Evaluate(currentLevel + 1);
    }

    void UpdateInterface()
    {
        int start = totalExperience - previousLevelsExperience;
        int end = nextLevelsExperience - previousLevelsExperience;

        levelText.text = currentLevel.ToString();
        experienceSlider.maxValue = end;
        experienceSlider.value = start;
    }
}


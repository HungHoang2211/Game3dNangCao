using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class YouWinUI_H : MonoBehaviour
{
    [Header("UI Elements")]
    public CanvasGroup winPanelCG;
    public RectTransform winTextTransform;
    public CanvasGroup mainMenuButton;
    public ParticleSystem confettiEffect;

    [Header("Settings")]
    public float slowMoFactor = 0.5f;
    public string mainMenuSceneName = "MainMenu";

    void Start()
    {
        winPanelCG.alpha = 0;
        if (mainMenuButton != null)
        {
            mainMenuButton.alpha = 0;
            mainMenuButton.interactable = false;
            mainMenuButton.blocksRaycasts = false;
        }

        winTextTransform.localScale = new Vector3(1.5f, 1.5f, 1.5f);

        // Thêm dòng này - hiện YouWin khi boss chết
        if (QuestManager.Instance != null)
            QuestManager.Instance.OnQuestCompleted += ShowYouWin;
    }

    void OnDestroy()
    {
        if (QuestManager.Instance != null)
            QuestManager.Instance.OnQuestCompleted -= ShowYouWin;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            ShowYouWin();
        }
    }

    public void ShowYouWin()
    {
        StartCoroutine(WinSequence());
    }

    IEnumerator WinSequence()
    {
        if (confettiEffect != null)
        {
            confettiEffect.Play();
        }

        Time.timeScale = slowMoFactor;
        Time.fixedDeltaTime = 0.02f * Time.timeScale;

        float duration = 2.5f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;
            float progress = elapsed / duration;

            winPanelCG.alpha = Mathf.Lerp(0, 1, elapsed / 1.5f);
            winTextTransform.localScale = Vector3.Lerp(new Vector3(1.5f, 1.5f, 1.5f), Vector3.one, progress);

            if (mainMenuButton != null && elapsed > 1.2f)
            {
                mainMenuButton.alpha = Mathf.Lerp(0, 1, (elapsed - 1.2f) / 1f);
            }

            yield return null;
        }

        if (mainMenuButton != null)
        {
            mainMenuButton.interactable = true;
            mainMenuButton.blocksRaycasts = true;
        }
    }

    public void BackToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(mainMenuSceneName);
    }
}
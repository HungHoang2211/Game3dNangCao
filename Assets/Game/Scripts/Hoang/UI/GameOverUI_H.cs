using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

public class GameOverUI : MonoBehaviour
{
    public CanvasGroup panelCanvasGroup;
    public RectTransform textTransform;
    public CanvasGroup restartButtonCG;
    public CanvasGroup mainMenuButtonCG;
    public float slowMoFactor = 0.3f;

    void Start()
    {
        panelCanvasGroup.alpha = 0;
        SetGroupActive(restartButtonCG, false);
        SetGroupActive(mainMenuButtonCG, false);

        textTransform.localScale = new Vector3(1.5f, 1.5f, 1.5f);
    }

    public void ShowGameOver()
    {
        StartCoroutine(EldenSequence());
    }

    IEnumerator EldenSequence()
    {
        Time.timeScale = slowMoFactor;
        Time.fixedDeltaTime = 0.02f * Time.timeScale;

        float duration = 3f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;
            float progress = elapsed / duration;

            panelCanvasGroup.alpha = Mathf.Lerp(0, 1, elapsed / 1.5f);

            float buttonsAlpha = Mathf.Lerp(0, 1, (elapsed - 1.5f) / 1.5f);
            restartButtonCG.alpha = buttonsAlpha;
            mainMenuButtonCG.alpha = buttonsAlpha;

            textTransform.localScale = Vector3.Lerp(new Vector3(1.2f, 1.2f, 1.2f), Vector3.one, progress);

            yield return null;
        }
        SetGroupActive(restartButtonCG, true);
        SetGroupActive(mainMenuButtonCG, true);
        panelCanvasGroup.interactable = true;
        panelCanvasGroup.blocksRaycasts = true;
    }

    void SetGroupActive(CanvasGroup cg, bool active)
    {
        cg.interactable = active;
        cg.blocksRaycasts = active;
    }

    public void RestartGame()
    {
        SFXManager.Instance.ClickButton();
        Time.timeScale = 1f;  // thêm dòng này
        SceneManager.LoadScene("Main");
    }

    public void GoToMainMenu()
    {
        SFXManager.Instance.ClickButton();
        Debug.Log("Press button");
        Time.timeScale = 1f;  // thêm dòng này
        SceneManager.LoadScene("Menu");
    }
}
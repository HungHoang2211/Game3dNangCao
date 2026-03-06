using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    [SerializeField]
    public GameObject settingPanel;
    public void StartGame()
    {
        BGMManager.Instance.PlayTrack(1);
        SceneManager.LoadScene("Main");
    }

    public void OpenSetting()
    {
        SFXManager.Instance.ClickButton();
        settingPanel.SetActive(true);
    }

    public void CloseSetting()
    {
        SFXManager.Instance.ClickButton();
        settingPanel.SetActive(false);
    } 

    public void QuitGame()
    {
        SFXManager.Instance.ClickButton();
        Application.Quit();
    }

    public void BackToMenu()
    {
        SFXManager.Instance.ClickButton();
        SceneManager.LoadScene("Menu");
    }
}

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    [SerializeField]
    public GameObject settingPanel;
    public void StartGame()
    {
        SceneManager.LoadScene("Level1");
    }

    public void OpenSetting()
    {
        settingPanel.SetActive(true);
    }
}

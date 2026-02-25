using UnityEngine;
using TMPro;
public class QuestManager : MonoBehaviour
{
    public static  QuestManager Instance;

    [Header("Quest Setting")]
    public int killTarget = 5;
    private int currentKill = 0;

    [Header("Reward")]
    public WeaponStat rewardWeapon;

    [Header("UI")]
    public TextMeshProUGUI questText;
    public GameObject completeText;

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
        currentKill++;
        UpdateUI();

        Debug.Log("Kill: " + currentKill);

        if(currentKill >= killTarget)
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
        if (completeText != null)
        {
            completeText.SetActive(true);

        }

        //Inventory.Instance.AddItem(rewardWeapon);
    }


}

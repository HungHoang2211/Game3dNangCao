using TMPro;
using UnityEngine;

public class DamagePopup : MonoBehaviour
{
    public float floatSpeed = 1f;
    public float lifeTime = 1f;

    void Update()
    {
        transform.position += Vector3.up * floatSpeed * Time.deltaTime;
        lifeTime -= Time.deltaTime;
        if (lifeTime <= 0)
            Destroy(gameObject);
    }
    [SerializeField] private TMP_Text damageText;
    public void SetText(int damage)
    {
        damageText.text = damage.ToString();
    }
}

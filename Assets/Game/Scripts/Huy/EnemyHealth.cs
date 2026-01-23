using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    public int hp = 30;
    Renderer rend;
    public GameObject damagePopupPrefab;

    void Awake()
    {
        rend = GetComponentInChildren<Renderer>();
    }


    public void TakeDamage(int damage)
    {
        hp -= damage;
        Debug.Log("Emeny HP: " + hp);
        //GameObject popup = Instantiate(
        //    damagePopupPrefab,
        //    transform.position + Vector3.up,
        //    Quaternion.identity
        //);
        //popup.GetComponent<DamagePopup>().SetText(damage);

        if (hp <= 0) {
            Destroy(transform.root.gameObject);
            Debug.Log("Enemy Dead!");
        } 
    }


    System.Collections.IEnumerator HitFlash()
    {
        Color original = rend.material.color;
        rend.material.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        rend.material.color = original;
    }
}

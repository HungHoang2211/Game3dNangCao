using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerFireBall : MonoBehaviour
{
    public GameObject fireBallPrefab;
    public Transform firePoint;

    public float cooldown = 120f; 

    private float cooldownTimer = 0f;

    void Update()
    {
        if (cooldownTimer > 0)
        {
            cooldownTimer -= Time.deltaTime;
        }
    }
    public void FireButton()
    {
        if (cooldownTimer <= 0f)
        {
            Shoot();
            cooldownTimer = cooldown;
        }
    }

    void Shoot()
    {
        Instantiate(fireBallPrefab, firePoint.position, firePoint.rotation);
    }
}
using UnityEngine;

public class PlayerAttackSound : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioClip attackAirSound;
    public AudioClip[] attackEnemySounds;

    public void PlayAirSound()
    {
        audioSource.PlayOneShot(attackAirSound);
    }

    public void PlayEnemyHitSound()
    {
        int index = Random.Range(0, attackEnemySounds.Length);
        audioSource.PlayOneShot(attackEnemySounds[index]);
    }
}
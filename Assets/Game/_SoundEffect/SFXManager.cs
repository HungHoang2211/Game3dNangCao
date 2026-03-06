using UnityEngine;
using UnityEngine.Audio;

/// <summary>
/// SFX Manager using AudioMixer for volume control via slider.
///
/// SETUP:
/// 1. Create AudioMixer (Assets > Create > Audio Mixer) named "MainMixer"
/// 2. In AudioMixer window, create 3 groups under Master:
///    - Music
///    - SFX
/// 3. Expose parameters:
///    - Click Master > Volume > Right-click > Expose > rename to "MasterVolume"
///    - Click Music > Volume > Right-click > Expose > rename to "MusicVolume"
///    - Click SFX > Volume > Right-click > Expose > rename to "SFXVolume"
/// 4. Create empty GameObject "SFXManager", add this script
/// 5. Drag MainMixer and SFX group into Inspector
/// 6. Drag audio clips into Inspector
/// </summary>
public class SFXManager : MonoBehaviour
{
    public static SFXManager Instance { get; private set; }

    [Header("Audio Mixer")]
    [SerializeField] private AudioMixerGroup sfxGroup;

    [Header("Audio Clips")]
    [SerializeField] private AudioClip slashClip;
    [SerializeField] private AudioClip explosionClip;
    [SerializeField] private AudioClip healClip;
    [SerializeField] private AudioClip clickClip;

    [Header("Volume")]
    [Range(0f, 1f)]
    [SerializeField] private float slashVolume = 0.8f;
    [Range(0f, 1f)]
    [SerializeField] private float explosionVolume = 1f;
    [Range(0f, 1f)]
    [SerializeField] private float healVolume = 0.7f;
    [Range(0f, 1f)]
    [SerializeField] private float clickVolume = 0.7f;

    private AudioSource audioSource;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;

        // Route through SFX mixer group
        if (sfxGroup != null)
            audioSource.outputAudioMixerGroup = sfxGroup;
    }

    public void PlaySlash()
    {
        if (slashClip != null)
            audioSource.PlayOneShot(slashClip, slashVolume);
    }
    public void ClickButton()
    {
        if (clickClip != null)
            audioSource.PlayOneShot(clickClip, clickVolume);
    }
    public void PlayExplosion()
    {
        if (explosionClip != null)
            audioSource.PlayOneShot(explosionClip, explosionVolume);
    }

    public void PlayHeal()
    {
        if (healClip != null)
            audioSource.PlayOneShot(healClip, healVolume);
    }

    public void Play(AudioClip clip, float volume = 1f)
    {
        if (clip != null)
            audioSource.PlayOneShot(clip, volume);
    }
}

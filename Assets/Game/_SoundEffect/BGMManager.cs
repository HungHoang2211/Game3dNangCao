using UnityEngine;
using UnityEngine.Audio;

/// <summary>
/// Background music manager. Singleton, survives scene loads.
///
/// SETUP:
/// 1. Create empty GameObject "BGMManager" in first scene
/// 2. Add this script
/// 3. Drag Music group from AudioMixer into musicGroup field
/// 4. Drag background music clips into bgmClips array
/// </summary>
public class BGMManager : MonoBehaviour
{
    public static BGMManager Instance { get; private set; }

    [Header("Audio Mixer")]
    [Tooltip("Music group from AudioMixer")]
    [SerializeField] private AudioMixerGroup musicGroup;

    [Header("Music Clips")]
    [Tooltip("Background music tracks")]
    [SerializeField] private AudioClip[] bgmClips;

    [Header("Settings")]
    [Range(0f, 1f)]
    [SerializeField] private float volume = 0.5f;

    [SerializeField] private bool playOnStart = true;
    [SerializeField] private bool loop = true;

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

        // Setup AudioSource
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;
        audioSource.loop = loop;
        audioSource.volume = volume;

        // Route through Music mixer group
        if (musicGroup != null)
            audioSource.outputAudioMixerGroup = musicGroup;
    }

    private void Start()
    {
        if (playOnStart && bgmClips.Length > 0)
            PlayTrack(0);
    }

    /// <summary>
    /// Play track by index
    /// </summary>
    public void PlayTrack(int index)
    {
        if (index < 0 || index >= bgmClips.Length) return;
        if (bgmClips[index] == null) return;

        audioSource.clip = bgmClips[index];
        audioSource.Play();
    }

    /// <summary>
    /// Play specific clip
    /// </summary>
    public void PlayClip(AudioClip clip)
    {
        if (clip == null) return;

        audioSource.clip = clip;
        audioSource.Play();
    }

    public void Stop() => audioSource.Stop();
    public void Pause() => audioSource.Pause();
    public void Resume() => audioSource.UnPause();
    public bool IsPlaying() => audioSource.isPlaying;
}

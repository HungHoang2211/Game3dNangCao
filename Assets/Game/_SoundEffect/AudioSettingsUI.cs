using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

/// <summary>
/// Connects UI Sliders to AudioMixer volume parameters.
/// Saves settings with PlayerPrefs (persists between sessions).
///
/// SETUP:
/// 1. Add this script to your Settings panel
/// 2. Drag MainMixer into audioMixer field
/// 3. Drag sliders into masterSlider, musicSlider, sfxSlider
/// 4. Make sure exposed parameter names match exactly:
///    "MasterVolume", "MusicVolume", "SFXVolume"
/// </summary>
public class AudioSettingsUI : MonoBehaviour
{
    [Header("Audio Mixer")]
    [SerializeField] private AudioMixer audioMixer;

    [Header("Sliders")]
    //[SerializeField] private Slider masterSlider;
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider sfxSlider;

    [Header("Exposed Parameter Names")]
    //[SerializeField] private string masterParam = "MasterVolume";
    [SerializeField] private string musicParam = "MusicVolume";
    [SerializeField] private string sfxParam = "SFXVolume";

    private void Start()
    {
        // Load saved values (default = 0.75)
        //float savedMaster = PlayerPrefs.GetFloat(masterParam, 0.75f);
        float savedMusic = PlayerPrefs.GetFloat(musicParam, 0.75f);
        float savedSFX = PlayerPrefs.GetFloat(sfxParam, 0.75f);

        //// Set slider values
        //if (masterSlider != null)
        //{
        //    masterSlider.value = savedMaster;
        //    masterSlider.onValueChanged.AddListener(SetMasterVolume);
        //}

        if (musicSlider != null)
        {
            musicSlider.value = savedMusic;
            musicSlider.onValueChanged.AddListener(SetMusicVolume);
        }

        if (sfxSlider != null)
        {
            sfxSlider.value = savedSFX;
            sfxSlider.onValueChanged.AddListener(SetSFXVolume);
        }

        // Apply saved volumes
        //SetMasterVolume(savedMaster);
        SetMusicVolume(savedMusic);
        SetSFXVolume(savedSFX);
    }

    //public void SetMasterVolume(float value)
    //{
    //    audioMixer.SetFloat(masterParam, SliderToDecibel(value));
    //    PlayerPrefs.SetFloat(masterParam, value);
    //}

    public void SetMusicVolume(float value)
    {
        audioMixer.SetFloat(musicParam, SliderToDecibel(value));
        PlayerPrefs.SetFloat(musicParam, value);
    }

    public void SetSFXVolume(float value)
    {
        audioMixer.SetFloat(sfxParam, SliderToDecibel(value));
        PlayerPrefs.SetFloat(sfxParam, value);
    }

    /// <summary>
    /// Convert slider (0-1) to decibel (-80 to 0).
    /// Slider = 0 -> mute (-80dB), Slider = 1 -> full (0dB)
    /// </summary>
    private float SliderToDecibel(float sliderValue)
    {
        if (sliderValue <= 0.0001f)
            return -80f;

        return Mathf.Log10(sliderValue) * 20f;
    }
}

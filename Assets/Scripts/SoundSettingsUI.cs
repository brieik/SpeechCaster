using UnityEngine;
using UnityEngine.UI;

public class SoundSettingsUI : MonoBehaviour
{
    private Slider masterSlider;
    private Slider musicSlider;
    private Slider sfxSlider;
    private Toggle muteToggle;

    void Awake()
    {
        // Try to find sliders and toggle automatically under this panel
        masterSlider = transform.Find("MasterSlider")?.GetComponent<Slider>();
        musicSlider = transform.Find("MusicSlider")?.GetComponent<Slider>();
        sfxSlider = transform.Find("SFXSlider")?.GetComponent<Slider>();
        muteToggle = transform.Find("MuteToggle")?.GetComponent<Toggle>();
    }

    void Start()
    {
        if (AudioManager.Instance == null) return;

        // Setup Master slider
        if (masterSlider != null)
        {
            masterSlider.value = AudioManager.Instance.MasterVolume;
            masterSlider.onValueChanged.RemoveAllListeners();
            masterSlider.onValueChanged.AddListener(AudioManager.Instance.SetMasterVolume);
        }

        // Setup Music slider
        if (musicSlider != null)
        {
            musicSlider.value = AudioManager.Instance.MusicVolume;
            musicSlider.onValueChanged.RemoveAllListeners();
            musicSlider.onValueChanged.AddListener(AudioManager.Instance.SetMusicVolume);
        }

        // Setup SFX slider
        if (sfxSlider != null)
        {
            sfxSlider.value = AudioManager.Instance.SFXVolume;
            sfxSlider.onValueChanged.RemoveAllListeners();
            sfxSlider.onValueChanged.AddListener(AudioManager.Instance.SetSFXVolume);
        }

        // Setup Mute toggle
        if (muteToggle != null)
        {
            muteToggle.isOn = AudioManager.Instance.IsMuted;
            muteToggle.onValueChanged.RemoveAllListeners();
            muteToggle.onValueChanged.AddListener(AudioManager.Instance.ToggleMute);
        }
    }
}

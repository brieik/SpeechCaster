using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("Audio Setup")]
    public AudioSource audioSource;

    [Header("UI Controls")]
    public Slider masterSlider;
    public Slider musicSlider;
    public Slider sfxSlider;
    public Toggle muteToggle;

    [Header("Scene Music")]
    public SceneMusic[] sceneMusicList; // Each scene has its own track

    private bool isMuted = false;
    private float masterVolume = 1f;
    private float musicVolume = 1f;
    private float sfxVolume = 1f;

    [System.Serializable]
    public class SceneMusic
    {
        public string sceneName;
        public AudioClip musicClip;
    }

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        // Load saved settings
        masterVolume = PlayerPrefs.GetFloat("MasterVolume", 1f);
        musicVolume = PlayerPrefs.GetFloat("MusicVolume", 1f);
        sfxVolume = PlayerPrefs.GetFloat("SFXVolume", 1f);
        isMuted = PlayerPrefs.GetInt("IsMuted", 0) == 1;

        if (masterSlider != null)
            masterSlider.value = masterVolume;

        if (musicSlider != null)
            musicSlider.value = musicVolume;

        if (sfxSlider != null)
            sfxSlider.value = sfxVolume;

        if (muteToggle != null)
            muteToggle.isOn = isMuted;

        ApplyVolume();

        // Scene change listener
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void Start()
    {
        PlaySceneMusic(SceneManager.GetActiveScene().name);
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        PlaySceneMusic(scene.name);
    }

    private void PlaySceneMusic(string sceneName)
    {
        AudioClip newClip = GetMusicForScene(sceneName);

        if (newClip == null)
        {
            audioSource.Stop();
            return;
        }

        if (audioSource.clip == newClip && audioSource.isPlaying)
            return; // already playing

        audioSource.clip = newClip;
        audioSource.loop = true;
        audioSource.Play();
        ApplyVolume();
    }

    private AudioClip GetMusicForScene(string sceneName)
    {
        foreach (var entry in sceneMusicList)
        {
            if (entry.sceneName == sceneName)
                return entry.musicClip;
        }
        return null;
    }

    public void SetMasterVolume(float value)
    {
        masterVolume = value;
        SaveSettings();
        ApplyVolume();
    }

    public void SetMusicVolume(float value)
    {
        musicVolume = value;
        SaveSettings();
        ApplyVolume();
    }

    public void SetSFXVolume(float value)
    {
        sfxVolume = value;
        SaveSettings();
        ApplyVolume();
    }

    public void ToggleMute(bool value)
    {
        isMuted = value;
        SaveSettings();
        ApplyVolume();
    }

    private void ApplyVolume()
    {
        float finalMaster = isMuted ? 0 : masterVolume;

        if (audioSource != null)
            audioSource.volume = finalMaster * musicVolume;

        if (SFXManager.Instance != null)
            SFXManager.Instance.UpdateVolume(finalMaster * sfxVolume, isMuted);
    }

    private void SaveSettings()
    {
        PlayerPrefs.SetFloat("MasterVolume", masterVolume);
        PlayerPrefs.SetFloat("MusicVolume", musicVolume);
        PlayerPrefs.SetFloat("SFXVolume", sfxVolume);
        PlayerPrefs.SetInt("IsMuted", isMuted ? 1 : 0);
    }
}

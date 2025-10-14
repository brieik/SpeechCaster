using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    // Public getters for UI scripts
    public float MasterVolume => masterVolume;
    public float MusicVolume => musicVolume;
    public float SFXVolume => sfxVolume;
    public bool IsMuted => isMuted;

    [Header("Audio Setup")]
    public AudioSource musicSource;
    public AudioSource sfxSource;
    public AudioSource bgSource; // for wind background

    [Header("UI Controls")]
    public Slider masterSlider;
    public Slider musicSlider;
    public Slider sfxSlider;
    public Toggle muteToggle;

    [Header("Scene Music")]
    public SceneMusic[] sceneMusicList;

    [Header("SFX Clips")]
    public AudioClip explosionSound;
    public AudioClip witchLaughSound;
    public AudioClip windBackground;

    private bool isMuted = false;
    private float masterVolume = 1f;
    private float musicVolume = 1f;
    private float sfxVolume = 1f;
    private float currentSFXVolume = 1f;
    private Coroutine laughRoutine;

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

        // Add AudioSources if not assigned
        if (musicSource == null) musicSource = gameObject.AddComponent<AudioSource>();
        if (sfxSource == null) sfxSource = gameObject.AddComponent<AudioSource>();
        if (bgSource == null) bgSource = gameObject.AddComponent<AudioSource>();
        bgSource.loop = true;

        // Load saved settings
        masterVolume = PlayerPrefs.GetFloat("MasterVolume", 1f);
        musicVolume = PlayerPrefs.GetFloat("MusicVolume", 1f);
        sfxVolume = PlayerPrefs.GetFloat("SFXVolume", 1f);
        isMuted = PlayerPrefs.GetInt("IsMuted", 0) == 1;

        SetupUIControls();
        ApplyVolume();

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void Start()
    {
        PlaySceneMusic(SceneManager.GetActiveScene().name);
        if (SceneManager.GetActiveScene().name == "MainGame") StartRandomLaughs();
    }

    private void SetupUIControls()
    {
        if (masterSlider != null)
        {
            masterSlider.value = masterVolume;
            masterSlider.onValueChanged.RemoveAllListeners();
            masterSlider.onValueChanged.AddListener(SetMasterVolume);
        }

        if (musicSlider != null)
        {
            musicSlider.value = musicVolume;
            musicSlider.onValueChanged.RemoveAllListeners();
            musicSlider.onValueChanged.AddListener(SetMusicVolume);
        }

        if (sfxSlider != null)
        {
            sfxSlider.value = sfxVolume;
            sfxSlider.onValueChanged.RemoveAllListeners();
            sfxSlider.onValueChanged.AddListener(SetSFXVolume);
        }

        if (muteToggle != null)
        {
            muteToggle.isOn = isMuted;
            muteToggle.onValueChanged.RemoveAllListeners();
            muteToggle.onValueChanged.AddListener(ToggleMute);
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        PlaySceneMusic(scene.name);

        // Stop/start background & random laughs based on scene
        if (scene.name == "MainGame")
        {
            PlayBackground();
            StartRandomLaughs();
        }
        else
        {
            StopBackground();
            StopRandomLaughs();
        }

        SetupUIControls();
    }

    private void PlaySceneMusic(string sceneName)
    {
        AudioClip newClip = null;
        foreach (var entry in sceneMusicList)
            if (entry.sceneName == sceneName) newClip = entry.musicClip;

        if (newClip == null)
        {
            musicSource.Stop();
            return;
        }

        if (musicSource.clip == newClip && musicSource.isPlaying) return;

        musicSource.clip = newClip;
        musicSource.loop = true;
        musicSource.Play();
        ApplyVolume();
    }

    public void PlayExplosion()
    {
        if (explosionSound != null)
            sfxSource.PlayOneShot(explosionSound, currentSFXVolume);
    }

    public void PlayWitchLaugh()
    {
        if (witchLaughSound != null)
            sfxSource.PlayOneShot(witchLaughSound, currentSFXVolume);
    }

    public void PlayBackground()
    {
        if (windBackground != null && !bgSource.isPlaying)
        {
            bgSource.clip = windBackground;
            bgSource.volume = currentSFXVolume * 0.2f; // quieter
            bgSource.Play();
        }
    }

    public void StopBackground()
    {
        if (bgSource.isPlaying) bgSource.Stop();
    }

    private void StartRandomLaughs()
    {
        if (laughRoutine == null)
            laughRoutine = StartCoroutine(RandomLaughCoroutine());
    }

    private void StopRandomLaughs()
    {
        if (laughRoutine != null)
        {
            StopCoroutine(laughRoutine);
            laughRoutine = null;
        }
    }

    private IEnumerator RandomLaughCoroutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(3f, 8f));
            PlayWitchLaugh();
        }
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

        if (musicSource != null)
            musicSource.volume = finalMaster * musicVolume;

        currentSFXVolume = finalMaster * sfxVolume;

        if (bgSource != null)
            bgSource.volume = currentSFXVolume * 0.2f;
    }

    private void SaveSettings()
    {
        PlayerPrefs.SetFloat("MasterVolume", masterVolume);
        PlayerPrefs.SetFloat("MusicVolume", musicVolume);
        PlayerPrefs.SetFloat("SFXVolume", sfxVolume);
        PlayerPrefs.SetInt("IsMuted", isMuted ? 1 : 0);
    }
}

using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class SFXManager : MonoBehaviour
{
    public static SFXManager Instance;

    [Header("Sound Effects")]
    public AudioClip explosionSound;
    public AudioClip witchLaughSound;

    [Header("Background Sound")]
    public AudioClip windBackground;

    private AudioSource sfxSource;
    private AudioSource bgSource;
    private float currentVolume = 1f;
    private Coroutine laughRoutine;

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

        sfxSource = gameObject.AddComponent<AudioSource>();
        bgSource = gameObject.AddComponent<AudioSource>();
        bgSource.loop = true;
        bgSource.volume = 0.2f;

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Only active in MainGame scene
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
    }

    public void PlayBackground()
    {
        if (windBackground != null && !bgSource.isPlaying)
        {
            bgSource.clip = windBackground;
            bgSource.Play();
        }
    }

    public void StopBackground()
    {
        if (bgSource.isPlaying)
            bgSource.Stop();
    }

    public void PlayExplosion()
    {
        if (explosionSound != null)
            sfxSource.PlayOneShot(explosionSound, currentVolume);
    }

    public void PlayWitchLaugh()
    {
        if (witchLaughSound != null)
            sfxSource.PlayOneShot(witchLaughSound, currentVolume);
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
            // Wait for 3–8 seconds randomly before laughing again
            float waitTime = Random.Range(3f, 8f);
            yield return new WaitForSeconds(waitTime);

            PlayWitchLaugh();
        }
    }

    // 🔊 Called from AudioManager
    public void UpdateVolume(float volume, bool muted)
    {
        currentVolume = muted ? 0f : volume;
        bgSource.volume = muted ? 0f : volume * 0.2f;
    }
}

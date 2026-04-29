using System.Collections;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("Player")]
    public AudioClip playerHit;
    public AudioClip playerDeath;
    public AudioClip footstep;

    [Header("Weapons")]
    public AudioClip swordSwing;
    public AudioClip swordHit;

    [Header("Enemies")]
    public AudioClip enemyHit;
    public AudioClip enemyDeath;
    public AudioClip enemyShoot;

    [Header("Boss")]
    public AudioClip bossHit;
    public AudioClip bossDeath;
    public AudioClip bossPhaseTwo;

    [Header("Item Pickup")]
    public AudioClip itemPickup;
    public AudioClip healthPickup;

    [Header("Music")]
    public AudioClip gameMusic;
    public AudioClip mainMenuMusic;

    [Header("UI")]
    public AudioClip buttonClick;

    [Header("Settings")]
    [Range(0f, 1f)] public float masterVolume = 1f;
    [Range(0f, 1f)] public float sfxVolume = 1f;
    [Range(0f, 1f)] public float musicVolume = 0.5f;

    private AudioSource sfxSource;
    private AudioSource musicSource;

    private void Awake()
    {
        Instance = this;

        sfxSource = gameObject.AddComponent<AudioSource>();

        musicSource = gameObject.AddComponent<AudioSource>();
        musicSource.loop = true;
        musicSource.volume = musicVolume;
    }

    public void Play(AudioClip clip, float pitch = 1f)
    {
        if (clip == null) 
        {
            return;
        }

        sfxSource.pitch = pitch;
        sfxSource.PlayOneShot(clip, masterVolume * sfxVolume);
    }
    public void PlayMusic(AudioClip clip, float fadeDuration = 1f)
    {
        if (clip == null || musicSource.clip == clip) 
        {
            return;
        }

        StartCoroutine(FadeToMusic(clip, fadeDuration));
    }


    public void StopMusic()
    {
        musicSource.Stop();
    }

    public void PlayWithVariation(AudioClip clip)
    {
        if (clip == null)
        {
            return;
        }

        float pitch = Random.Range(0.9f, 1.1f);
        Play(clip, pitch);
    }

    private IEnumerator FadeToMusic(AudioClip newClip, float fadeDuration)
    {
        if (musicSource.isPlaying)
        {
            float startVolume = musicSource.volume;
            while (musicSource.volume > 0f)
            {
                musicSource.volume -= startVolume * Time.deltaTime / fadeDuration;
                yield return null;
            }
            musicSource.Stop();
        }

        musicSource.clip = newClip;
        musicSource.Play();
        musicSource.volume = 0f;

        while (musicSource.volume < musicVolume)
        {
            musicSource.volume += musicVolume * Time.deltaTime / fadeDuration;
            yield return null;
        }

        musicSource.volume = musicVolume;
    }
}

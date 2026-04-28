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

    [Header("UI")]
    public AudioClip buttonClick;

    [Header("Settings")]
    [Range(0f, 1f)] public float masterVolume = 1f;
    [Range(0f, 1f)] public float sfxVolume = 1f;

    private AudioSource sfxSource;

    private void Awake()
    {
        Instance = this;
        sfxSource = gameObject.AddComponent<AudioSource>();
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

    public void PlayWithVariation(AudioClip clip)
    {
        if (clip == null)
        {
            return;
        }

        float pitch = Random.Range(0.9f, 1.1f);
        Play(clip, pitch);
    }
}

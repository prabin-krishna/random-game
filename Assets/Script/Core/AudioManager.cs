using System.Collections;
using UnityEngine;
[System.Serializable]
public class GameAudioClips
{
    [Header("Gameplay Sounds")]
    public AudioClip flipSound;
    public AudioClip matchSound;
    public AudioClip mismatchSound;
    public AudioClip gameOverSound;
    public AudioClip youWonSound;
    public AudioClip hideSound;
    
    [Header("Ambient Sounds")]
    public AudioClip ambientSound;
    
    [Header("UI Sounds")]
    public AudioClip buttonClickSound;
}
public class AudioManager : MonoBehaviour
{
    [Header("Audio Sources")]
    [SerializeField] private AudioSource sfxAudioSource;
    [SerializeField] private AudioSource ambientAudioSource;
    [SerializeField] private AudioSource uiAudioSource;
    
    [Header("Audio Clips")]
    [SerializeField] private GameAudioClips audioClips;
    
    [Header("Volume Settings")]
    [Range(0f, 1f)] public float sfxVolume = 0.6f;
    [Range(0f, 1f)] public float ambientVolume = 0.5f;
    [Range(0f, 1f)] public float uiVolume = 0.6f;
    
    private static AudioManager _instance;
    public static AudioManager Instance
    {
        get
        {
            if (_instance == null)
                _instance = FindObjectOfType<AudioManager>();
            return _instance;
        }
    }
    
    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeAudioSources();
        }
        else if (_instance != this)
        {
            Destroy(gameObject);
        }
    }
    
    private void Start()
    {
        PlayAmbientSound();
    }
    
    private void InitializeAudioSources()
    {
        if (sfxAudioSource == null)
        {
            sfxAudioSource = gameObject.AddComponent<AudioSource>();
        }
        sfxAudioSource.volume = sfxVolume;
        sfxAudioSource.loop = false;
        
        if (ambientAudioSource == null)
        {
            ambientAudioSource = gameObject.AddComponent<AudioSource>();
        }
        ambientAudioSource.volume = ambientVolume;
        ambientAudioSource.loop = true;
        
        if (uiAudioSource == null)
        {
            uiAudioSource = gameObject.AddComponent<AudioSource>();
        }
        uiAudioSource.volume = uiVolume;
        uiAudioSource.loop = false;
    }
    
    public void PlayCardFlip()
    {
        PlaySFX(audioClips.flipSound);
    }
    
    public void PlayCardMatch()
    {
        PlaySFX(audioClips.matchSound);
    }
    
    public void PlayCardMismatch()
    {
        PlaySFX(audioClips.mismatchSound);
    }
    
    public void PlayGameOver()
    {
        PlaySFX(audioClips.gameOverSound);
    }
    
    public void PlayYouWon()
    {
        PlaySFX(audioClips.youWonSound);
    }
    
    public void PlayCardHide()
    {
        PlaySFX(audioClips.hideSound);
    }
    
    public void PlayButtonClick()
    {
        PlaySFX(audioClips.buttonClickSound, uiAudioSource);
    }
    
    public void PlayAmbientSound()
    {
        if (audioClips.ambientSound != null && ambientAudioSource != null)
        {
            ambientAudioSource.clip = audioClips.ambientSound;
            ambientAudioSource.Play();
        }
    }
    
    public void StopAmbientSound()
    {
        if (ambientAudioSource != null)
        {
            ambientAudioSource.Stop();
        }
    }
    
    private void PlaySFX(AudioClip clip, AudioSource source = null)
    {
        if (clip == null) return;
        
        AudioSource audioSource = source ?? sfxAudioSource;
        if (audioSource != null)
        {
            audioSource.PlayOneShot(clip);
        }
    }
    
    public void SetSFXVolume(float volume)
    {
        sfxVolume = Mathf.Clamp01(volume);
        if (sfxAudioSource != null)
            sfxAudioSource.volume = sfxVolume;
    }
    
    public void SetAmbientVolume(float volume)
    {
        ambientVolume = Mathf.Clamp01(volume);
        if (ambientAudioSource != null)
            ambientAudioSource.volume = ambientVolume;
    }
    
    public void SetUIVolume(float volume)
    {
        uiVolume = Mathf.Clamp01(volume);
        if (uiAudioSource != null)
            uiAudioSource.volume = uiVolume;
    }
    
    public void PauseAmbient()
    {
        if (ambientAudioSource != null && ambientAudioSource.isPlaying)
        {
            ambientAudioSource.Pause();
        }
    }
    
    public void ResumeAmbient()
    {
        if (ambientAudioSource != null && !ambientAudioSource.isPlaying)
        {
            ambientAudioSource.UnPause();
        }
    }
}
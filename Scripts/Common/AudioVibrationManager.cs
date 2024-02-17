using System;
using UnityEngine;
using UnityEngine.Audio;

public class AudioVibrationManager : MonoBehaviour
{
    public static AudioVibrationManager Instance;

    public float Audio { get; private set; }
    public float Music { get; private set; }

    public bool IsVibrationEnabled { get; private set; }

    public Action VibrationChanged;

    [SerializeField] private string SoundGroup = "Sound";
    [SerializeField] private string MusicGroup = "Music";

    [SerializeField] private AudioMixer _mixer;

    public AudioClip Win;
    public AudioClip SlingShot;
    public AudioClip ShotEmpty;
    public AudioClip ShotFull;
    public AudioClip Wrong;

    [SerializeField] private GameObject _soundPrefab;

    private void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(gameObject);
        else
            Instance = this;

        DontDestroyOnLoad(gameObject);

        Initialize();
    }
    private void Initialize()
    {
        Audio = PlayerPrefs.GetFloat("Audio", 1);
        Music = PlayerPrefs.GetFloat("Music", 1);

        IsVibrationEnabled = Convert.ToBoolean(PlayerPrefs.GetInt("IsVibrationEnabled", 1));
    }
    private void Start()
    {
        ChangeAudio(Audio);
        ChangeMusic(Music);
    }
    public void ChangeAudio(float value)
    {
        if (value > 0 && value <= 1)
        {
            Audio = value;
            _mixer.SetFloat(SoundGroup, Mathf.Log10(value) * 20);
        }
        else if (value == 0)
        {
            Audio = value;
            _mixer.SetFloat(SoundGroup, -80);
        }
    }
    public void ChangeMusic(float value)
    {
        if (value > 0 && value <= 1)
        {
            Music = value;
            _mixer.SetFloat(MusicGroup, Mathf.Log10(value) * 20);
        }
        else if (value == 0)
        {
            Music = value;
            _mixer.SetFloat(MusicGroup, -80);
        }
    }
    public void ToggleVibration()
    {
        IsVibrationEnabled = !IsVibrationEnabled;

        PlayerPrefs.SetInt("IsVibrationEnabled", Convert.ToInt32(IsVibrationEnabled));
    }
    public void PlaySound(AudioClip clip, float pitch, float volume = 1f)
    {
        Instantiate(_soundPrefab).GetComponent<Sound>().PlaySound(clip, pitch, volume);
    }
    public void Save()
    {
        PlayerPrefs.SetFloat("Audio", Audio);
        PlayerPrefs.SetFloat("Music", Music);
    }
}

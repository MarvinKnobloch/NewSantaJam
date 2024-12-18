using UnityEngine;
using UnityEngine.Audio;
using System;

public class AudioController : MonoBehaviour
{
    public static AudioController Instance;

    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource soundSource;

    [SerializeField] private AudioMixer audiomixer;
    [SerializeField] private string masterVolume;
    [SerializeField] private string musicVolume;
    [SerializeField] private string soundVolume;

    [SerializeField] private IngameMusic[] musicClips;
    [SerializeField] private IngameSounds[] soundClips;

    public enum Songs
    {
        empty,
        song1,
        song2,
    }
    public enum Sounds
    {
        empty,
        menuButton,
    }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        if (PlayerPrefs.GetInt("audiohasbeenchange") == 0)
        {
            PlayerPrefs.SetFloat(masterVolume, -10);
            audiomixer.SetFloat(masterVolume, PlayerPrefs.GetFloat(masterVolume));
            PlayerPrefs.SetFloat(musicVolume, -8);
            audiomixer.SetFloat(musicVolume, PlayerPrefs.GetFloat(musicVolume));
            PlayerPrefs.SetFloat(soundVolume, 10);
            audiomixer.SetFloat(soundVolume, PlayerPrefs.GetFloat(soundVolume));
        }
        else
        {
            setvolume(masterVolume, 0);
            setvolume(musicVolume, 0);
            setvolume(soundVolume, 20);
        }
    }
    private void setvolume(string volumename, float maxdb)
    {
        audiomixer.SetFloat(volumename, PlayerPrefs.GetFloat(volumename));
        bool gotvalue = audiomixer.GetFloat(volumename, out float soundvalue);
        if (gotvalue == true)
        {
            if (soundvalue > maxdb)
            {
                audiomixer.SetFloat(volumename, maxdb);
            }
        }
    }

    public void PlaySoundOneshot(int soundNumber)
    {
        soundSource.PlayOneShot(soundClips[soundNumber].clip, soundClips[soundNumber].volume);
    }

    public void SetSong(int songNumber)
    {
        if (musicSource.clip == musicClips[songNumber].clip) return;

        musicSource.volume = musicClips[songNumber].volume;
        musicSource.clip = musicClips[songNumber].clip;
        musicSource.Play();
    }

    [Serializable]
    public struct IngameMusic
    {
        public AudioClip clip;
        public float volume;
    }
    [Serializable]
    public struct IngameSounds
    {
        public AudioClip clip;
        public float volume;
    }
}

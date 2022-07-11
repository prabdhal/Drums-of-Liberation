using UnityEngine;
using UnityEngine.UI;

public class AudioManager : MonoBehaviour
{
    [SerializeField] Light light;
    [SerializeField] AudioSource audioSource;
    [SerializeField] Slider volumeSlider;
    [SerializeField] Toggle muteToggle;
    [SerializeField] Toggle shadowToggle;
    [SerializeField] float musicMaxVolume = 0.05f;

    public int Shadow { get { return shadow; } }
    private int shadow;
    public bool Mute { get { return audioSource.mute; } }
    public float Volume { get { return audioSource.volume; } }
    #region Singleton
    public static AudioManager Instance;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(this);
    }
    #endregion

    private void Start()
    {
        if (light == null)
            light = FindObjectOfType<Light>();
        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();

        InitSoundAndGraphicsSettings();
        AdjustVolumeSlider();
    }

    private void Update()
    {
        AdjustVolumeSlider();
    }

    private void InitSoundAndGraphicsSettings()
    {
        audioSource.mute = MenuDataManager.Mute;
        muteToggle.isOn = MenuDataManager.Mute;

        switch (MenuDataManager.Shadow)
        {
            case 0:
                light.shadows = LightShadows.None;
                shadowToggle.isOn = false;
                break;
            case 2:
                light.shadows = LightShadows.Soft;
                shadowToggle.isOn = true;
                break;
        }

        volumeSlider.value = MenuDataManager.MusicVolume / musicMaxVolume;
    }

    public void ShadowToggle()
    {
        shadow = shadow == 0 ? 2 : 0;

        switch (shadow)
        {
            case 0:
                light.shadows = LightShadows.None;
                break;
            case 2:
                light.shadows = LightShadows.Soft;
                break;
        }
    }

    public void MuteToggle()
    {
        audioSource.mute = !audioSource.mute;
    }

    private void AdjustVolumeSlider()
    {
        float volume = volumeSlider.value;
        AdjustMusicVolume(volume);
    }

    private void AdjustMusicVolume(float volume)
    {
        audioSource.volume = volume * musicMaxVolume;
    }

    private void AdjustSoundVolume()
    {

    }
}

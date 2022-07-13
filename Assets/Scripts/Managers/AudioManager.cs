using UnityEngine;
using UnityEngine.UI;

public class AudioManager : MonoBehaviour
{
    [SerializeField] Light light;
    [SerializeField] AudioSource musicAudioSource;
    [SerializeField] Slider volumeSlider;
    [SerializeField] Toggle muteToggle;
    [SerializeField] Toggle shadowToggle;
    [SerializeField] float musicMaxVolume = 0.05f;

    public AudioSource playerFootStepAudioSource;
    public AudioSource playerSwordSwingAudioSource;
    public AudioSource playerSwordImpactAudioSource;

    public int Shadow { get { return shadow; } }
    private int shadow;
    public bool Mute { get { return musicAudioSource.mute; } }
    public float Volume { get { return musicAudioSource.volume; } }

    public bool isMainMenu = false;

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
        if (musicAudioSource == null)
            musicAudioSource = GetComponent<AudioSource>();
        if (!isMainMenu)
        {
            if (playerFootStepAudioSource == null)
                playerFootStepAudioSource = PlayerManager.Instance.GetComponentInChildren<AudioSource>();
            if (playerFootStepAudioSource == null)
                playerSwordSwingAudioSource = GameObject.FindGameObjectWithTag(StringData.PlayerWeaponTag).GetComponent<AudioSource>();
            if (playerFootStepAudioSource == null)
                playerSwordSwingAudioSource = GameObject.FindGameObjectWithTag(StringData.PlayerWeaponTag).GetComponent<AudioSource>();
            playerFootStepAudioSource.volume = 0.1f;
        }

        InitSoundAndGraphicsSettings();
        AdjustVolumeSlider();
    }

    private void Update()
    {
        AdjustVolumeSlider();
    }

    private void InitSoundAndGraphicsSettings()
    {
        musicAudioSource.mute = MenuDataManager.Mute;
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
        musicAudioSource.mute = !musicAudioSource.mute;
    }

    private void AdjustVolumeSlider()
    {
        float volume = volumeSlider.value;
        AdjustMusicVolume(volume);
    }

    private void AdjustMusicVolume(float volume)
    {
        musicAudioSource.volume = volume * musicMaxVolume;
    }

    private void AdjustSoundVolume()
    {

    }
}

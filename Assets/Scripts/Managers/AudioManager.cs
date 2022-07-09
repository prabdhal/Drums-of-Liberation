using System.Collections;
using System.Collections.Generic;
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

    public bool Shadows { get { return light.shadows.Equals(LightShadows.Soft); } }
    public bool Mute { get { return audioSource.mute; } }
    public float Volume { get { return audioSource.volume; } }
    

    private void Start()
    {
        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();

        muteToggle.isOn = Mute;
        shadowToggle.isOn = Shadows;
        AdjustVolumeSlider();
        volumeSlider.value = Volume / musicMaxVolume;
        Debug.Log("Volume: " + Volume + " musicMaxVolume: " + musicMaxVolume + " volumeSlider: " + Volume / musicMaxVolume);
    }

    private void Update()
    {
        AdjustVolumeSlider();
    }

    public void ShadowToggle()
    {
        if (Shadows)
            light.shadows = LightShadows.None;
        else
            light.shadows = LightShadows.Soft;
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

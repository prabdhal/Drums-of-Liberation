using UnityEngine;

public class FootStepAudio : MonoBehaviour
{
    [SerializeField] AudioSource footStepAudio;
    [SerializeField] AudioClip grassFootStepAudio;
    [SerializeField] AudioClip dirtFootStepAudio;


    public void PlayFootstepSound()
    {
        footStepAudio.Play();
    }
}

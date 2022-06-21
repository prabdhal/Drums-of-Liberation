using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class ScreenFader : MonoBehaviour
{
  [SerializeField]
  private Animator anim;
  [SerializeField]
  private GameObject blackScreen;


  [SerializeField]
  private PlayableDirector director;
  [SerializeField]
  private float timeBeforeEndOfTimeline = 4f;

  private bool fadeToBlack = false;


  private void Start()
  {
    anim = GetComponent<Animator>();
    director = FindObjectOfType<PlayableDirector>();

    blackScreen.SetActive(true);
    FadeToScreen();
  }

  private void Update()
  {
    if (director.duration - director.time <= timeBeforeEndOfTimeline)
      FadeToBlack();
  }

  public void FadeToBlack()
  {
    anim.Play(StringData.BlackScreen);
  }

  public void FadeToScreen()
  {
    anim.Play(StringData.ClearScreen);
  }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class CutSceneOneTimelineManager : MonoBehaviour
{
  [SerializeField]
  private PlayableDirector playableDirector;
  [SerializeField]
  private Animator anim;

  [SerializeField]
  private float endSceneBeforeTimelineTimerInSeconds = 2f;

  private void Start()
  {
    playableDirector = GetComponent<PlayableDirector>();
    anim.Play(StringData.ClearScreen);
    Play();
  }

  private void Update()
  {
    if (playableDirector.duration - playableDirector.time <= endSceneBeforeTimelineTimerInSeconds)
      anim.Play(StringData.BlackScreen);
  }

  private void Play()
  {
    playableDirector.Play();
  }
}

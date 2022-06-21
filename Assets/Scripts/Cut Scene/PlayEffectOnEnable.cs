using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayEffectOnEnable : MonoBehaviour
{
  [SerializeField]
  private ParticleSystem effect;

  private void OnEnable()
  {
    effect.Play();
  }
}

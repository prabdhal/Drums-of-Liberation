using System;
using UnityEngine;


[Serializable]
public class Pursue
{
  [Tooltip("The detection range at which the user can detection the target.")]
  public float detectionRange = 20f;
  [Tooltip("The detection radius at which the user can detection the target.")]
  [Range(0,360)]
  public float detectionRadius = 60f;
  [Tooltip("Pursuing movement speed.")]
  public float speedRatioMultiplier = 1.5f;
  [Tooltip("Targets to purse.")]
  public LayerMask targetLayer;
  [Tooltip("The time the user spends searching for the target at the last seen location.")]
  public float startSearchingTimer = 10f;
  [HideInInspector] public float currSearchingTimer = 10f;
}
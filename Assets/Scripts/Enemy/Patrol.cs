using System;
using UnityEngine;


[Serializable]
public class Patrol
{
  public Transform[] waypoints;
  [HideInInspector] public int currWP = 0;
  [Tooltip("The stopping distance between the user and the waypoint object.")]
  public float stoppingDistance = 2f;
  [Tooltip("The minimum amount of time the user will remain idle at a waypoint object.")]
  public float waitTimerMin = 5f;
  [Tooltip("The maximum amount of time the user will remain idle at a waypoint object.")]
  public float waitTimerMax = 10f;
  [Tooltip("The current amount of time the user is idle at a waypoint object.")]
  [HideInInspector] public float currWaitTimer = 10f;
}

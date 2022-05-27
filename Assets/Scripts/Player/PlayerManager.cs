using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
  #region Singleton
  public static PlayerManager Instance;

  private void Awake()
  {
    if (Instance == null)
      Instance = this;
    else
      Destroy(this);
  }
  #endregion

  public bool IsGrounded { get { return isGrounded; } }
  private bool isGrounded;
  public bool IsInteracting { get { return isInteracting; } }
  private bool isInteracting;

}

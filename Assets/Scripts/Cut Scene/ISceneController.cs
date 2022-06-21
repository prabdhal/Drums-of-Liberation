using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ISceneController 
{
  public bool CanContinueToNextScene { get; set; }
  public int CurSceneIdx { get; set; }
  public void GetNextScene();
}

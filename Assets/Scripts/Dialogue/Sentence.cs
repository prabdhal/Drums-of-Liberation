using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Sentence 
{
  public Sprite icon;
  [TextArea(3, 5)]
  public string sentence;
}

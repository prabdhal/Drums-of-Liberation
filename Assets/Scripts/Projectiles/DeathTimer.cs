using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathTimer : MonoBehaviour
{
  [SerializeField]
  private float deathTimer = 5f;
  private float curTimer = 0;


  private void Start()
  {
    curTimer = deathTimer;
  }

  private void Update()
  {
    if (curTimer <= 0)
      Destroy(gameObject);
    else
      curTimer -= Time.deltaTime;
  }
}

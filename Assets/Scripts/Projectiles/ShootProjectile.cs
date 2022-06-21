using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootProjectile : MonoBehaviour
{
  [SerializeField]
  private GameObject prefab;
  [SerializeField]
  private Transform firePoint;


  private void Shoot()
  {
    GameObject proj = Instantiate(prefab, firePoint.position, firePoint.rotation);
    Debug.Log("Fired");
  }
}

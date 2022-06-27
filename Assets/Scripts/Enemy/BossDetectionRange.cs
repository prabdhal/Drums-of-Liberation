using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossDetectionRange : MonoBehaviour
{
    public bool PlayerIsDetected { get { return playerIsDetected; } }
    private bool playerIsDetected = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(StringData.PlayerTag))
        {
            playerIsDetected = true;
        }
    }
}

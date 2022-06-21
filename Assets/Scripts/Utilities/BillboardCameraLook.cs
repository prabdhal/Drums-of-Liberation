using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BillboardCameraLook : MonoBehaviour
{
    private Camera cam;

    private void Start()
    {
        cam = Camera.main;
    }
    private void Update()
    {
        transform.LookAt(cam.transform);    
    }
}

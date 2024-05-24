using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField]
    private Camera[] cameras;

    private Camera currentCamera;

    // Start is called before the first frame update
    void Start()
    {
        currentCamera = cameras[0];
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F1))
        {
            currentCamera.enabled = false;
            currentCamera = cameras[0];
            currentCamera.enabled = true;
        }
        else if (Input.GetKeyDown(KeyCode.F2))
        {
            currentCamera.enabled = false;
            currentCamera = cameras[1];
            currentCamera.enabled = true;
        }
        else if (Input.GetKeyDown(KeyCode.F3))
        {
            currentCamera.enabled = false;
            currentCamera = cameras[2];
            currentCamera.enabled = true;
        }
        else if (Input.GetKeyDown(KeyCode.F4))
        {
            currentCamera.enabled = false;
            currentCamera = cameras[3];
            currentCamera.enabled = true;
        }
        else if (Input.GetKeyDown(KeyCode.F5))
        {
            currentCamera.enabled = false;
            currentCamera = cameras[4];
            currentCamera.enabled = true;
        }
        else if (Input.GetKeyDown(KeyCode.F6))
        {
            currentCamera.enabled = false;
            currentCamera = cameras[5];
            currentCamera.enabled = true;
        }
    }
}

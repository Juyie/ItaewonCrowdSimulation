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
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            currentCamera.enabled = false;
            currentCamera = cameras[0];
            currentCamera.enabled = true;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            currentCamera.enabled = false;
            currentCamera = cameras[1];
            currentCamera.enabled = true;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            currentCamera.enabled = false;
            currentCamera = cameras[2];
            currentCamera.enabled = true;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            currentCamera.enabled = false;
            currentCamera = cameras[3];
            currentCamera.enabled = true;
        }
    }
}

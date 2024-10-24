using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraZoom : MonoBehaviour
{
    Camera cam;

    // Start is called before the first frame update
    void Start()
    {
        cam = GetComponent<Camera>();
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKey(KeyCode.Equals))
        {
            cam.fieldOfView += 5 * Time.deltaTime;
        }

        if (Input.GetKey(KeyCode.Minus))
        {
            cam.fieldOfView -= 5 * Time.deltaTime;
        }
    }
}

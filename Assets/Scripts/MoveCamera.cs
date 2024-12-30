using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveCamera : MonoBehaviour
{
    public float rotateYSpeed = 250.0f;

    private void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        if (Input.GetMouseButton(1))
        {
            float x = Input.GetAxis("Mouse Y") * rotateYSpeed * Time.deltaTime * -1;
            gameObject.transform.eulerAngles = new Vector3(gameObject.transform.eulerAngles.x + x, gameObject.transform.eulerAngles.y, gameObject.transform.eulerAngles.z);
        }
    }
}

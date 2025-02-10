using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class MoveCamera : MonoBehaviour
{
    [SerializeField]
    private Camera cam;

    [SerializeField]
    private GameObject player;

    [SerializeField]
    private SteamVR_Input_Sources head;

    [SerializeField]
    private SteamVR_Action_Pose camRot;

    private Quaternion rotBase;
    private Quaternion camBase;

    private void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        camBase = cam.transform.rotation;
        rotBase = camRot.GetLocalRotation(head);
    }

    void Update()
    {
        Quaternion rotationOffset = camRot.GetLocalRotation(head)* Quaternion.Inverse(rotBase);
        rotationOffset = new Quaternion(-rotationOffset.x, rotationOffset.y, -rotationOffset.z, rotationOffset.w);
        Vector3 eulerRotation = rotationOffset.eulerAngles;
        cam.transform.rotation = camBase * rotationOffset;
        player.transform.rotation = camBase * Quaternion.Euler(0, eulerRotation.y, 0);
    }
}

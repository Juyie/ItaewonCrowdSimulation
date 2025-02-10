using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class VRCharacterController : MonoBehaviour
{
    [SerializeField]
    private GameObject player;

    [SerializeField]
    private float playerSpeed = 0.8f;
    
    [SerializeField]
    private SteamVR_Input_Sources waist;

    [SerializeField]
    private SteamVR_Action_Pose waistPos;

    private Vector3 posBefore;
    private Vector3 posNow;
    private Vector3 posDiff;
    private Vector3 corrPos;

    // Start is called before the first frame update
    void Start()
    {
        posBefore = waistPos.GetLocalPosition(waist);
    }

    // Update is called once per frame
    void Update()
    {
        posNow = waistPos.GetLocalPosition(waist);
        posDiff = posNow - posBefore;
        corrPos = new Vector3(-posDiff.x, 0, -posDiff.z);

        player.transform.Translate(corrPos * playerSpeed);

        posBefore = posNow;

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
    }
}

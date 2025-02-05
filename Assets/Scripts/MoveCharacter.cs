using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class MoveCharacter : MonoBehaviour
{
    public float playerSpeed = 0.8f;
    public float rotateXSpeed = 20.0f;

    [SerializeField]
    private SteamVR_Input_Sources leftHand;

    [SerializeField]
    private SteamVR_Input_Sources rightHand;

    [SerializeField]
    private SteamVR_Action_Vector2 moveDir;

    [SerializeField]
    private SteamVR_Action_Boolean movePadTouchec;

    [SerializeField]
    private SteamVR_Action_Vector2 camDir;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //Vector3 move = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        Vector3 move = new Vector3(moveDir.GetAxis(leftHand).x, 0f, moveDir.GetAxis(leftHand).y);
        gameObject.transform.Translate(move * Time.deltaTime * playerSpeed);
        Debug.Log(movePadTouchec.GetStateDown(leftHand) + " " + moveDir.GetActive(leftHand));

        float y = Input.GetAxis("Mouse X") * rotateXSpeed * Time.deltaTime;
        gameObject.transform.eulerAngles = new Vector3(gameObject.transform.eulerAngles.x, gameObject.transform.eulerAngles.y + y, gameObject.transform.eulerAngles.z);
    
        if(Input.GetKeyDown(KeyCode.Escape)){
            Application.Quit();
        }
    }
}

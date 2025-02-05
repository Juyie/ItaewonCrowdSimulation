using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Valve.VR;

public class VRPushForce : MonoBehaviour
{
    [SerializeField]
    private float pushForce = 0.1f;

    [SerializeField]
    private SteamVR_Input_Sources leftHand;

    [SerializeField]
    private SteamVR_Input_Sources rightHand;

    [SerializeField]
    private SteamVR_Action_Boolean trigger;

    private void OnTriggerStay(Collider other)
    {
        Rigidbody rb = other.GetComponent<Rigidbody>();
        if(rb != null && other.tag == "agent")
        {
            if (Input.GetButtonDown("Fire1") || trigger.GetState(leftHand) || trigger.GetState(rightHand))
            {
                if (rb.GetComponent<NavMeshAgent>().enabled)
                {
                    rb.GetComponent<NavMeshAgent>().velocity += gameObject.transform.forward * pushForce * Time.fixedDeltaTime;
                }
                else
                {
                    rb.AddForce(gameObject.transform.forward * pushForce, ForceMode.Impulse);
                }
            }
        }
    }

}

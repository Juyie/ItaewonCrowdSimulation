using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class VRPushForce : MonoBehaviour
{
    [SerializeField]
    private float pushForce = 0.1f;

    private void OnTriggerStay(Collider other)
    {
        Rigidbody rb = other.GetComponent<Rigidbody>();
        if(rb != null && other.tag == "agent")
        {
            if (Input.GetMouseButtonDown(0))
            {
                if (rb.GetComponent<NavMeshAgent>().enabled)
                {
                    Debug.Log("RVO");
                    rb.GetComponent<NavMeshAgent>().velocity += gameObject.transform.forward * pushForce * Time.fixedDeltaTime;
                }
                else
                {
                    Debug.Log("SPH");
                    rb.AddForce(gameObject.transform.forward * pushForce, ForceMode.Impulse);
                }
            }
        }
    }

}

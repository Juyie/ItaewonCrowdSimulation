using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PrefVelocityAndPressureVelocity : MonoBehaviour
{
    private NavMeshAgent navAgent;
    private Rigidbody agentRb;

    // Start is called before the first frame update
    void Start()
    {
        navAgent = GetComponent<NavMeshAgent>();
        agentRb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        /*
        if(Mathf.Abs(Vector3.Distance(navAgent.desiredVelocity, agentRb.velocity)) > 0.1f)
        {
            Debug.Log("Desired: " + navAgent.desiredVelocity + ", Real: " + agentRb.velocity);
        }
        */
        //Debug.Log("Desired: " + navAgent.desiredVelocity + ", Real: " + navAgent.velocity);
    }
}

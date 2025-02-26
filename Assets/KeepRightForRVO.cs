using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class KeepRightForRVO : MonoBehaviour
{
    private NavMeshAgent agent;
    private float offset = 100.0f;

    private NavMeshPath path;
    private int currentCornerIndex = 0;

    private bool calcRightPath = false;
    private float strength = 1.0f;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    void Update()
    {
        /*
        if (agent.enabled && agent.isOnNavMesh && !agent.pathPending)
        {
            if (!calcRightPath)
            {
                CalculatePathWithRightOffset();
            }
            else
            {
                if (currentCornerIndex < path.corners.Length && Vector3.Distance(agent.transform.position, path.corners[currentCornerIndex]) < 0.5f)
                {
                    currentCornerIndex++;
                    if (currentCornerIndex < path.corners.Length)
                    {
                        agent.SetDestination(path.corners[currentCornerIndex]);
                        Debug.Log("Set new dst: " + path.corners[currentCornerIndex]);
                    }
                }
            }
        }
        */

        agent.velocity = agent.desiredVelocity - CalculateRightOffset() * strength;
        //agent.velocity = CalculateRightOffset2();    
    }

    private Vector3 CalculateRightOffset()
    {
        Vector3 right = Vector3.Cross(agent.desiredVelocity, agent.transform.up);
        if(Mathf.Abs(agent.desiredVelocity.x) > Mathf.Abs(agent.desiredVelocity.z))
        {
            if (agent.desiredVelocity.x * right.x > 0)
            {
                return right;
            }
        }
        else if(Mathf.Abs(agent.desiredVelocity.x) < Mathf.Abs(agent.desiredVelocity.z))
        {
            if(agent.desiredVelocity.z * right.z > 0)
            {
                return right;
            }
        }

        return Vector3.zero;
    }

    private Vector3 CalculateRightOffset2()
    {
        Vector3 rightOffset = Vector3.Cross(agent.desiredVelocity, Vector3.up);
        if (Mathf.Abs(agent.desiredVelocity.x) > Mathf.Abs(agent.desiredVelocity.z))
        {
            if (agent.desiredVelocity.x * rightOffset.x > 0)
            {
                Quaternion rightRotation = Quaternion.Euler(0, 5f, 0);
                return rightRotation * agent.desiredVelocity;
            }
        }
        else if (Mathf.Abs(agent.desiredVelocity.x) < Mathf.Abs(agent.desiredVelocity.z))
        {
            if (agent.desiredVelocity.x * rightOffset.x < 0)
            {
                Quaternion rightRotation = Quaternion.Euler(0, 5f, 0);
                return rightRotation * agent.desiredVelocity;
            }
        }

        return agent.desiredVelocity;
    }
}

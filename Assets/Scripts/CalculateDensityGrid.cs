using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class CalculateDensityGrid : MonoBehaviour
{
    private GridAgentList agentList;

    private float area;
    private int criteriaAgentNum;

    private bool turnOn = false;

    // Start is called before the first frame update
    void Start()
    {
        agentList = GetComponent<GridAgentList>();
        area = transform.localScale.x * transform.localScale.z * 100;
        criteriaAgentNum = Mathf.FloorToInt(area * 4);
    }

    // Update is called once per frame
    void Update()
    {
        if (agentList.GetListLength() >= criteriaAgentNum)
        {
            turnOn = true;
            agentList.TurnOnRagdolls();
        }
        /*
        else
        {
            agentList.TurnOffRagdolls();
        }
        */
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("agent") && !agentList.FindAgent(other.gameObject) && other.GetComponent<NavMeshAgent>().velocity.sqrMagnitude < 0.01f)
        {
            agentList.AddAgent(other.gameObject);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (turnOn)
        {
            if (other.CompareTag("agent") && !agentList.FindAgent(other.gameObject))
            {
                agentList.AddAgent(other.gameObject);
            }
        }
        else
        {
            if (other.CompareTag("agent") && !agentList.FindAgent(other.gameObject) && other.GetComponent<NavMeshAgent>().velocity.sqrMagnitude < 0.01f)
            {
                agentList.AddAgent(other.gameObject);
            }
            else if (other.CompareTag("agent") && agentList.FindAgent(other.gameObject))
            {
                agentList.RemoveAgent(other.gameObject);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("agent") && agentList.FindAgent(other.gameObject))
        {
            agentList.RemoveAgent(other.gameObject);
        }
    }
}

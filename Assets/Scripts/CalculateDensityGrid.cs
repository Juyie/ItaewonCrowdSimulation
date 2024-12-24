using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEditor;

public class CalculateDensityGrid : MonoBehaviour
{
    private GridAgentList agentList;

    private float area;
    private int criteriaAgentNumSPH;
    private int criteriaAgentNumSPHZombie;
    private int criteriaAgentNumRagdoll;

    [HideInInspector]
    public bool turnOn = false;
    [HideInInspector]
    public bool mySPH = false;
    [HideInInspector]
    public bool allSPH = false;

    public GridController controller;

    // Start is called before the first frame update
    void Start()
    {
        agentList = GetComponent<GridAgentList>();
        area = transform.localScale.x * transform.localScale.z * 100;
        criteriaAgentNumSPH = Mathf.FloorToInt(area * 4);
        criteriaAgentNumSPHZombie = Mathf.FloorToInt(area * 12);
        criteriaAgentNumRagdoll = Mathf.FloorToInt(area * 12);
        if(controller == null)
        {
            controller = GameObject.FindWithTag("GridController").GetComponent<GridController>();
        }
    }

    // List
    // Update is called once per frame
    void Update()
    {
        if (agentList.GetListLength() >= criteriaAgentNumRagdoll && turnOn && allSPH)
        {
            agentList.TurnOnRagdollDensity();
        }
        if (agentList.GetListLength() >= criteriaAgentNumSPHZombie && turnOn)
        {
            agentList.TurnOnSPHZombie();

            if(agentList.GetListLength() < criteriaAgentNumRagdoll)
            {
                agentList.TurnOffRagdollDensity();
            }
        }
        else if (agentList.GetListLength() >= criteriaAgentNumSPH && turnOn)
        {
            agentList.TurnOnSPH();
            mySPH = true;

            if (agentList.GetListLength() < criteriaAgentNumSPHZombie)
            {
                agentList.TurnOffSPHZombie();
            }
            else if (agentList.GetListLength() < criteriaAgentNumRagdoll)
            {
                agentList.TurnOffRagdollDensity();
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (this.enabled)
        {
            if (other.CompareTag("agent") && !agentList.FindAgent(other.gameObject) && other.GetComponent<PlayerMovement>().velocity.sqrMagnitude < controller.stepSize)
            {
                agentList.AddAgent(other.gameObject);
            }
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (this.enabled)
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
                if (other.CompareTag("agent") && !agentList.FindAgent(other.gameObject) && other.GetComponent<PlayerMovement>().velocity.sqrMagnitude < controller.stepSize)
                {
                    agentList.AddAgent(other.gameObject);
                }
                else if (other.CompareTag("agent") && agentList.FindAgent(other.gameObject))
                {
                    agentList.RemoveAgent(other.gameObject);
                }
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (this.enabled)
        {
            if (other.CompareTag("agent") && agentList.FindAgent(other.gameObject))
            {
                agentList.RemoveAgent(other.gameObject);
            }
            
        }
    }
}

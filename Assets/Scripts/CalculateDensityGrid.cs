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
    private int criteriaAgentNumRagdoll;
    private int criteriaSatisfyNum;

    public bool checkNeighbor = false;
    public bool satisfy = false;
    public bool turnOn = false;

    // weight calculation ver.
    private float criteriaWeight = 0.95f;
    private float criteriaWeightNum;

    public GridController controller;

    // Start is called before the first frame update
    void Start()
    {
        agentList = GetComponent<GridAgentList>();
        area = transform.localScale.x * transform.localScale.z * 100;
        criteriaAgentNumSPH = Mathf.FloorToInt(area * 2);
        criteriaAgentNumRagdoll = Mathf.FloorToInt(area * 10);
        criteriaSatisfyNum = Mathf.FloorToInt(area * 3);
        criteriaWeightNum = Mathf.FloorToInt(area * 4);
    }

    // List
    // Update is called once per frame
    void Update()
    {
        if (controller.Step)
        {
            if (agentList.GetListLength() >= criteriaSatisfyNum)
            {
                satisfy = true;
                //GetComponent<MeshRenderer>().material.color = Color.red;
            }
            if (checkNeighbor)
            {
                if(agentList.GetListLength() >= criteriaAgentNumSPH && turnOn && satisfy)
                {
                    agentList.TurnOnSPH();
                }
                else if (agentList.GetListLength() >= criteriaAgentNumRagdoll && turnOn && satisfy)
                {
                    agentList.TurnOnRagdolls();
                }
            }
            else
            {
                if (agentList.GetListLength() >= criteriaAgentNumRagdoll)
                {
                    turnOn = true;
                    agentList.TurnOnRagdolls();
                }
                else if (agentList.GetListLength() >= criteriaAgentNumSPH)
                {
                    turnOn = true;
                    agentList.TurnOnSPH();
                }
            }
        }
        else if(controller.Exponential)
        {
            agentList.SetExponentialNum(controller.exponentialSize);
            if (agentList.GetSpeedPerAgent() >= criteriaWeight * criteriaWeightNum)
            {
                agentList.TurnOnRagdollsDic();
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (controller.Step)
        {
            if (other.CompareTag("agent") && !agentList.FindAgent(other.gameObject) && other.GetComponent<NavMeshAgent>().velocity.sqrMagnitude < controller.stepSize)
            {
                agentList.AddAgent(other.gameObject);
            }
        }
        else if(controller.Exponential)
        {
            if (other.CompareTag("agent") && !agentList.FindAgentDic(other.gameObject))
            {
                agentList.AddAgentDic(other.gameObject, other.GetComponent<NavMeshAgent>().desiredVelocity.magnitude);
                Debug.Log(other.GetComponent<NavMeshAgent>().desiredVelocity.magnitude);
            }
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (controller.Step)
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
                if (other.CompareTag("agent") && !agentList.FindAgent(other.gameObject) && other.GetComponent<NavMeshAgent>().velocity.sqrMagnitude < controller.stepSize)
                {
                    agentList.AddAgent(other.gameObject);
                }
                else if (other.CompareTag("agent") && agentList.FindAgent(other.gameObject))
                {
                    agentList.RemoveAgent(other.gameObject);
                }
            }
        }
        else if(controller.Exponential) 
        {
            if (other.CompareTag("agent") && agentList.FindAgentDic(other.gameObject))
            {
                agentList.FixAgentDic(other.gameObject, other.GetComponent<NavMeshAgent>().desiredVelocity.magnitude);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (controller.Step)
        {
            if (other.CompareTag("agent") && agentList.FindAgent(other.gameObject))
            {
                agentList.RemoveAgent(other.gameObject);
            }
        }
        else if(controller.Exponential)
        {
            if (other.CompareTag("agent") && agentList.FindAgentDic(other.gameObject))
            {
                agentList.RemoveAgentDic(other.gameObject);
            }
        }
    }
    
    
    /*
    // Dictionary
    void Update()
    {
        //Debug.Log(agentList.GetSpeedPerAgent());
        if (agentList.GetSpeedPerAgent() >= criteriaWeight * criteriaWeightNum)
        {
            agentList.TurnOnRagdollsDic();
        }

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("agent") && !agentList.FindAgentDic(other.gameObject))
        {
            agentList.AddAgentDic(other.gameObject, other.GetComponent<NavMeshAgent>().desiredVelocity.magnitude);
            Debug.Log(other.GetComponent<NavMeshAgent>().desiredVelocity.magnitude);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("agent") && agentList.FindAgentDic(other.gameObject))
        {
            agentList.FixAgentDic(other.gameObject, other.GetComponent<NavMeshAgent>().desiredVelocity.magnitude);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("agent") && agentList.FindAgentDic(other.gameObject))
        {
            agentList.RemoveAgentDic(other.gameObject);
        }
    }
    */
}

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
    private int criteriaSatisfyNum;

    public bool checkNeighbor = false;
    public bool satisfy = false;
    public bool turnOn = false;
    public bool mySPH = false;
    public bool allSPH = false;
    public bool suffSPH = false;

    public bool turnOnRagdoll = false;

    // weight calculation ver.
    private float criteriaWeight = 0.95f;
    private float criteriaWeightNum;

    public GridController controller;

    // Start is called before the first frame update
    void Start()
    {
        agentList = GetComponent<GridAgentList>();
        area = transform.localScale.x * transform.localScale.z * 100;
        criteriaAgentNumSPH = Mathf.FloorToInt(area * 4);
        criteriaAgentNumSPHZombie = Mathf.FloorToInt(area * 12);
        criteriaAgentNumRagdoll = Mathf.FloorToInt(area * 12);
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
            else
            {
                satisfy = false;
                //GetComponent<MeshRenderer>().material.color = Color.white;
            }

            if (checkNeighbor)
            {
                if (agentList.GetListLength() >= criteriaAgentNumRagdoll && turnOn && satisfy && allSPH && suffSPH)
                {
                    //agentList.TurnOnRagdolls();
                    agentList.TurnOnRagdollDensity();
                }
                else if(agentList.GetListLength() >= criteriaAgentNumSPHZombie && turnOn && satisfy)
                {
                    agentList.TurnOnSPHZombie();

                    if(agentList.GetListLength() < criteriaAgentNumRagdoll)
                    {
                        agentList.TurnOffRagdollDensity();
                    }
                }
                else if (agentList.GetListLength() >= criteriaAgentNumSPH && turnOn && satisfy)
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
                /*
                else if (agentList.GetListLength() < criteriaAgentNumSPH)
                {
                    agentList.TurnOffSPH();
                    mySPH = false;
                }
                */
            }
            else
            {
                if (agentList.GetListLength() >= criteriaAgentNumRagdoll)
                {
                    turnOn = true;
                    //agentList.TurnOnRagdolls();
                }
                else if (agentList.GetListLength() >= criteriaAgentNumSPH)
                {
                    turnOn = true;
                    agentList.TurnOnSPH();
                }
                else
                {
                    turnOn = false;
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
        if (this.enabled)
        {
            if (controller.Step)
            {
                if (other.CompareTag("agent") && !agentList.FindAgent(other.gameObject) && other.GetComponent<PlayerMovement>().velocity.sqrMagnitude < controller.stepSize)
                {
                    agentList.AddAgent(other.gameObject);
                }
            }
            else if (controller.Exponential)
            {
                if (other.CompareTag("agent") && !agentList.FindAgentDic(other.gameObject))
                {
                    agentList.AddAgentDic(other.gameObject, other.GetComponent<NavMeshAgent>().desiredVelocity.magnitude);
                    Debug.Log(other.GetComponent<NavMeshAgent>().desiredVelocity.magnitude);
                }
            }
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (this.enabled)
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
            else if (controller.Exponential)
            {
                if (other.CompareTag("agent") && agentList.FindAgentDic(other.gameObject))
                {
                    agentList.FixAgentDic(other.gameObject, other.GetComponent<NavMeshAgent>().desiredVelocity.magnitude);
                }
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (this.enabled)
        {
            if (controller.Step)
            {
                if (other.CompareTag("agent") && agentList.FindAgent(other.gameObject))
                {
                    agentList.RemoveAgent(other.gameObject);
                }
            }
            else if (controller.Exponential)
            {
                if (other.CompareTag("agent") && agentList.FindAgentDic(other.gameObject))
                {
                    agentList.RemoveAgentDic(other.gameObject);
                }
            }
        }
    }
}

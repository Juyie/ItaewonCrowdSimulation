using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class CalculateDensity : MonoBehaviour
{
    [SerializeField]
    private Text densityUIText;

    [SerializeField]
    private bool useUI = false;

    [SerializeField]
    private OnOffRagdoll onOffRagdoll;

    [SerializeField]
    private AgentList agentList;

    private int totalAgentNum = 1;
    private int agentNum = 1;

    private float densityEval = 0.0f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        /*
        if(densityEval / totalAgentNum > 0.0f && agentNum > 64 && !onOffRagdoll.GetIsRagdollOn())
        {
            onOffRagdoll.TurnOnRagdoll();
        }
        else
        {
            onOffRagdoll.TurnOffRagdoll();
        }
        */
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("agent") && !agentList.FindAgent(other.name) && other.GetComponent<NavMeshAgent>().velocity.sqrMagnitude < 0.5f)
        {
            agentList.AddAgent(other.name);
        }

        /*
        if (other.CompareTag("agent"))
        {
            agentNum++;
            totalAgentNum++;
            float velocity = other.GetComponent<NavMeshAgent>().velocity.sqrMagnitude;
            densityEval += (2.2f - velocity) / 2.2f;
        }
        */
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("agent") && agentList.FindAgent(other.name))
        {
            agentList.RemoveAgent(other.name);
        }

        /*
        if (other.CompareTag("agent"))
        {
            agentNum--;
        }
        */
    }

    private void DisplayUI()
    {
        if (useUI)
        {
            densityUIText.text = agentNum.ToString();
        }
    }
}

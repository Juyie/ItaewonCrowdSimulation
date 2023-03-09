using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CalculateDensity : MonoBehaviour
{
    [SerializeField]
    private Text densityUIText;

    [SerializeField]
    private bool useUI = false;

    [SerializeField]
    private OnOffRagdoll onOffRagdoll;

    private int agentNum = 1;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(agentNum >= 64)
        {
            onOffRagdoll.TurnOnRagdoll();
        }
        else
        {
            onOffRagdoll.TurnOffRagdoll();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "agent")
        {
            agentNum++;
            DisplayUI();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "agent")
        {
            agentNum--;
            DisplayUI();
        }
    }

    private void DisplayUI()
    {
        if (useUI)
        {
            densityUIText.text = agentNum.ToString();
        }
    }
}

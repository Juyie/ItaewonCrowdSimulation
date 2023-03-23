using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControllAgentNumber : MonoBehaviour
{
    [SerializeField]
    private SpawnAgents[] spawnAgents;

    [SerializeField]
    private DisplayAgentNumber displayAgentNumber;

    [SerializeField]
    private int maxAgentNum;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(displayAgentNumber.agentNumber >= maxAgentNum)
        {
            foreach (SpawnAgents spawnAgent in spawnAgents)
            {
                spawnAgent.enabled = false;
            }
        }
        else
        {
            foreach (SpawnAgents spawnAgent in spawnAgents)
            {
                spawnAgent.enabled = true;
            }
        }
    }
}

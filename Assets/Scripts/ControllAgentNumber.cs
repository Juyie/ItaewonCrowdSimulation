using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControllAgentNumber : MonoBehaviour
{
    [SerializeField]
    private SpawnAgents[] spawnAgents;

    [SerializeField]
    private DisplayAgentNumber displayAgentNumber;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(displayAgentNumber.agentNumber >= 15000)
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

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class DebuggingNavMeshAgent : MonoBehaviour
{
    private NavMeshAgent agent;

    private void Awake()
    {
        
    }

    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    // Update is called once per frame
    void Update()
    {
        agent.isStopped = true;
    }
}

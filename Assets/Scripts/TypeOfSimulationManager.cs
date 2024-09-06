using System.Collections;
using System.Collections.Generic;
using Unity.Transforms;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;

public class TypeOfSimulationManager : MonoBehaviour
{
    [SerializeField]
    private float stepSize;

    [SerializeField]
    private int criteriaAgentNumSPH;

    [SerializeField]
    private int criteriaAgentNumRagdoll;

    private bool start = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        /*
        if ((Input.GetKeyDown(KeyCode.Alpha2)))// || recorderWindow.IsRecording()) && !start)
        {
            start = true;
        }
        */
        if (start)
        {
            for (int i = 0; i < NavagentSpawner.Instance.RVOGameObject.Length; i++)
            {
                GameObject agent = NavagentSpawner.Instance.RVOGameObject[i];
                if (NavagentSpawner.Instance.TypeOfSimulation[int.Parse(agent.name.Substring(23))] == 0) // RVO agent
                {
                    if (agent.GetComponent<SPHProperties>().density >= criteriaAgentNumSPH && agent.GetComponent<SPHProperties>().velocity.magnitude < stepSize)
                    {
                        TurnOnSPH(agent);
                    }
                }
                else if (NavagentSpawner.Instance.TypeOfSimulation[int.Parse(agent.name.Substring(23))] == 1) // SPH agent
                {
                    if (agent.GetComponent<SPHProperties>().density < criteriaAgentNumSPH)
                    {
                        TurnOffSPH(agent);
                    }
                    else if (agent.GetComponent<SPHProperties>().density >= criteriaAgentNumRagdoll && agent.GetComponent<SPHProperties>().velocity.magnitude < stepSize)
                    {
                        //TurnOnRagdolls(agent);
                    }
                }
                else if (NavagentSpawner.Instance.TypeOfSimulation[int.Parse(agent.name.Substring(23))] == 2) // Ragdoll agent
                {
                    if (agent.GetComponent<SPHProperties>().density < criteriaAgentNumRagdoll)
                    {
                        //TurnOffRagdolls(agent);
                    }
                }
            }
        }
    }

    public void TurnOnSPH(GameObject agent)
    {
        agent.GetComponent<NavMeshAgent>().enabled = false;
        agent.GetComponent<NavMeshObstacle>().enabled = true;
        agent.GetComponent<SPHProperties>().position = agent.transform.position;
        agent.transform.parent = GameObject.Find("SPHAgents").transform;
        NavagentSpawner.Instance.TypeOfSimulation[int.Parse(agent.name.Substring(23))] = 1; // SPH
        agent.transform.GetChild(0).GetChild(0).GetComponent<SkinnedMeshRenderer>().material.color = Color.blue;
    }

    public void TurnOffSPH(GameObject agent)
    {
        agent.GetComponent<NavMeshAgent>().enabled = true;
        agent.GetComponent<NavMeshObstacle>().enabled = false;
        agent.GetComponent<SPHProperties>().position = agent.transform.position;
        agent.transform.parent = GameObject.Find("RVOAgents").transform;
        NavagentSpawner.Instance.TypeOfSimulation[int.Parse(agent.name.Substring(23))] = 0; // RVO
        agent.transform.GetChild(0).GetChild(0).GetComponent<SkinnedMeshRenderer>().material.color = Color.white;
    }

    public void TurnOnRagdolls(GameObject agent)
    {
        agent.GetComponent<OnOffRagdoll>().TurnOnRagdoll();
        agent.transform.parent = GameObject.Find("RagdollAgents").transform;
        NavagentSpawner.Instance.TypeOfSimulation[int.Parse(agent.name.Substring(23))] = 2; // Ragdoll
        agent.transform.GetChild(0).GetChild(0).GetComponent<SkinnedMeshRenderer>().material.color = Color.red;
    }

    public void TurnOffRagdolls(GameObject agent)
    {
        agent.GetComponent<OnOffRagdoll>().TurnOffRagdoll();
        NavagentSpawner.Instance.TypeOfSimulation[int.Parse(agent.name.Substring(23))] = 1; // SPH
        for (int i = 0; i < agent.transform.childCount; i++)
        {
            if(agent.transform.GetChild(i).GetComponent<SkinnedMeshRenderer>() != null)
            {
                agent.transform.GetChild(i).GetComponent<SkinnedMeshRenderer>().material.color = Color.white;
            }
            agent.transform.GetChild(0).GetComponent<SkinnedMeshRenderer>().material.color = Color.blue;
        }
    }
}

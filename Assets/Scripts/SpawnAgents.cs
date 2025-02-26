using DataStructures.ViliWonka.KDTree;
using System.Collections;
using System.Collections.Generic;
using Unity.Entities.UniversalDelegates;
using UnityEngine;
using UnityEngine.AI;

public class SpawnAgents : MonoBehaviour
{
    [SerializeField]
    private Transform spawnTf;

    [SerializeField]
    private Transform goalTf;

    [SerializeField]
    private float intervalTime;

    private DisplayAgentNumber displayAgentNumber;

    private bool isReady = true;
    private bool isOn = false;
    private int count = 0;

    // Start is called before the first frame update
    void Awake()
    {
        isOn = true;
        //Invoke("TurnOn", 10f);
        if(spawnTf == null)
        {
            spawnTf = GetComponent<Transform>();
        }
    }

    private void Start()
    {
        displayAgentNumber = GameObject.FindWithTag("Canvas").GetComponent<DisplayAgentNumber>();
        count = displayAgentNumber.agentNumber;
    }

    // Update is called once per frame
    void Update()
    {
        if (SaveAgentsData.GetMakeAgentDone() && isReady && isOn)
        {
            count = displayAgentNumber.agentNumber;
            if (count < NavagentSpawner.Instance.RVOGameObject.Length)
            {
                StartCoroutine(SpawnWaitAgent());
            }
        }
        
        count = displayAgentNumber.agentNumber;
    }
    IEnumerator SpawnWaitAgent()
    {
        isReady = false;
        yield return new WaitForSeconds(intervalTime);
        count = displayAgentNumber.agentNumber;
        if (GameObject.Find("WaitAgents").transform.GetChild(0).gameObject != null)
        {
            GameObject newAgent = GameObject.Find("WaitAgents").transform.GetChild(0).gameObject;
            newAgent.transform.parent = GameObject.Find("RVOAgents").transform;
            newAgent.transform.position = spawnTf.position;
            newAgent.GetComponent<PlayerMovement>().startTrans = spawnTf;
            newAgent.GetComponent<PlayerMovement>().target = goalTf;
            newAgent.GetComponent<NavMeshAgent>().enabled = true;
            newAgent.GetComponent<PlayerMovement>().enabled = true;
            SPHProperties sp = newAgent.GetComponent<SPHProperties>();
            sp.position = newAgent.transform.position;
            sp.goalPosition = goalTf.transform.position;

            NavagentSpawner.Instance.RVOPointCloud[int.Parse(newAgent.name.Substring(23))] = sp.position;
            NavagentSpawner.Instance.TypeOfSimulation[int.Parse(newAgent.name.Substring(23))] = 0;

            displayAgentNumber.agentNumber++;
            isReady = true;
        }
    }
}

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
    private GameObject agent;

    [SerializeField]
    private float intervalTime;

    [SerializeField]
    private DisplayAgentNumber displayAgentNumber;

    [SerializeField]
    private SPHManagerSingleThread SPHManager;

    private bool isReady = true;
    private Vector3 randPos;
    private bool isOn = false;
    private int count = 0;

    // Start is called before the first frame update
    void Awake()
    {
        //isOn = true;
    }

    private void Start()
    {
        count = displayAgentNumber.agentNumber;
    }

    // Update is called once per frame
    void Update()
    {
        if (isReady && isOn)
        {
            //StartCoroutine(Spawn());
            count = displayAgentNumber.agentNumber;
            if (count < 4500)
            {
                //StartCoroutine(SpawnWaitAgent());
                StartCoroutine(Spawn());
            }
        }
        
        count = displayAgentNumber.agentNumber;

        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            isOn = true;
        }

        /*
        if (Input.GetKeyDown(KeyCode.Return) || count > 3800)
        {
            isOn = !isOn;
        }
        */

        /*
        if(RVOAgents.Length != GameObject.Find("RVOAgents").transform.childCount)
        {
            UpdateRVO();
        }
        */
    }

    private void UpdateRVO()
    {
        /*
        RVOAgents = new RVOAgent[GameObject.Find("RVOAgents").transform.childCount];

        for (int i = 0; i < GameObject.Find("RVOAgents").transform.childCount; i++)
        {
            GameObject go = GameObject.Find("RVOAgents").transform.GetChild(i).gameObject;
            SPHProperties sp = go.GetComponent<SPHProperties>();

            RVOAgents[i].density = sp.density;
            RVOAgents[i].pressure = sp.pressure;
            RVOAgents[i].go = go;
        }
        */
    }

    IEnumerator Spawn()
    {
        isReady = false;
        yield return new WaitForSeconds(intervalTime);
        count = displayAgentNumber.agentNumber;
        displayAgentNumber.agentNumber++;
        GameObject newAgent = Instantiate(agent);
        randPos = new Vector3(Random.Range(-2f, 2f), 0, 0);
        newAgent.name += count.ToString();
        newAgent.transform.position = spawnTf.position + randPos;
        newAgent.transform.rotation = agent.transform.rotation;
        newAgent.transform.parent = GameObject.Find("RVOAgents").transform;
        newAgent.GetComponent<NavMeshAgent>().enabled = true;
        newAgent.GetComponent<PlayerMovement>().enabled = true;
        SPHProperties sp = newAgent.GetComponent<SPHProperties>();
        sp.position = newAgent.transform.position;
        newAgent.SetActive(true);

        NavagentSpawner.Instance.RVOGameObject[count] = newAgent;
        NavagentSpawner.Instance.RVOPointCloud[count] = sp.position;
        NavagentSpawner.Instance.TypeOfSimulation[count] = 0;

        // color code
        //Color randColor = Random.ColorHSV(0, 1, 1, 1);
        //newAgent.transform.GetChild(0).GetComponent<SkinnedMeshRenderer>().material.color = randColor;
        //newAgent.transform.GetChild(3).GetComponent<SkinnedMeshRenderer>().material.color = randColor;

        isReady = true;
    }

    IEnumerator SpawnWaitAgent()
    {
        isReady = false;
        yield return new WaitForSeconds(intervalTime);
        count = displayAgentNumber.agentNumber;
        GameObject newAgent = GameObject.Find("WaitAgents").transform.GetChild(0).gameObject;
        newAgent.transform.parent = GameObject.Find("RVOAgents").transform;
        randPos = new Vector3(Random.Range(-2f, 2f), 0, 0);
        newAgent.transform.position = spawnTf.position + randPos;
        newAgent.transform.rotation = agent.transform.rotation;
        newAgent.GetComponent<NavMeshAgent>().enabled = true;
        newAgent.GetComponent<PlayerMovement>().enabled = true;
        SPHProperties sp = newAgent.GetComponent<SPHProperties>();
        sp.position = newAgent.transform.position;
        newAgent.SetActive(true);

        NavagentSpawner.Instance.RVOGameObject[int.Parse(newAgent.name.Substring(23))] = newAgent;
        NavagentSpawner.Instance.RVOPointCloud[int.Parse(newAgent.name.Substring(23))] = sp.position;
        NavagentSpawner.Instance.TypeOfSimulation[int.Parse(newAgent.name.Substring(23))] = 0;

        displayAgentNumber.agentNumber++;
        isReady = true;
    }

    IEnumerator SetReady()
    {
        yield return new WaitForSeconds(1);
        isReady = true;
    }
}

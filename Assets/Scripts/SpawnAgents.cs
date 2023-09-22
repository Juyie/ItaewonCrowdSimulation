using System.Collections;
using System.Collections.Generic;
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

    public struct RVOAgent
    {
        public float density;
        public float pressure;
        public GameObject go;
    }

    public RVOAgent[] RVOAgents;

    private bool isReady = true;
    private Vector3 randPos;
    private bool isOn = true;
    private int count = 0;

    // Start is called before the first frame update
    void Awake()
    {
        isOn = true;
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
            StartCoroutine(Spawn());
            isReady = false;
        }

        if (Input.GetKeyDown(KeyCode.Return))
        {
            isOn = !isOn;
        }

        /*
        if(RVOAgents.Length != GameObject.Find("RVOAgents").transform.childCount)
        {
            UpdateRVO();
        }
        */
    }

    private void UpdateRVO()
    {
        RVOAgents = new RVOAgent[GameObject.Find("RVOAgents").transform.childCount];

        for (int i = 0; i < GameObject.Find("RVOAgents").transform.childCount; i++)
        {
            GameObject go = GameObject.Find("RVOAgents").transform.GetChild(i).gameObject;
            SPHProperties sp = go.GetComponent<SPHProperties>();

            RVOAgents[i].density = sp.density;
            RVOAgents[i].pressure = sp.pressure;
            RVOAgents[i].go = go;
        }
    }

    IEnumerator Spawn()
    {
        isReady = false;
        yield return new WaitForSeconds(intervalTime);
        count = displayAgentNumber.agentNumber;
        displayAgentNumber.agentNumber++;
        GameObject newAgent = Instantiate(agent);
        randPos = new Vector3(Random.Range(-3f, 3f), 0, Random.Range(-16f, 16f));
        newAgent.name += count.ToString();
        newAgent.transform.position = spawnTf.position + randPos;
        newAgent.transform.rotation = agent.transform.rotation;
        newAgent.transform.parent = GameObject.Find("RVOAgents").transform;
        newAgent.GetComponent<NavMeshAgent>().enabled = true;
        newAgent.GetComponent<PlayerMovement>().enabled = true;
        SPHProperties sp = newAgent.GetComponent<SPHProperties>();
        sp.position = newAgent.transform.position;
        newAgent.SetActive(true);

        NavagentSpawner.Instance.RVOAgents[count].density = 1.0f;
        NavagentSpawner.Instance.RVOAgents[count].pressure = 0.0f;
        NavagentSpawner.Instance.RVOAgents[count].go = newAgent;

        isReady = true;
    }

    IEnumerator SetReady()
    {
        yield return new WaitForSeconds(1);
        isReady = true;
    }
}

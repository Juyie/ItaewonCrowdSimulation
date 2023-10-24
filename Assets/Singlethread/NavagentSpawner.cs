using System.Collections;
using System.Collections.Generic;
using Unity.Entities.UniversalDelegates;
using UnityEngine;
using UnityEngine.AI;

public class NavagentSpawner : MonoBehaviour
{
    private static NavagentSpawner instance;
    public static NavagentSpawner Instance
    {
        get { return instance; }
    }

    public SPHManagerSingleThread SPHManager;

    private Vector3 goalPos1 = new Vector3(0.0f, 0.0f, 19.0f);
    private Vector3 goalPos2 = new Vector3(0.0f, 0.0f, -19.0f);

    [SerializeField]
    private GameObject charPrefab = null;

    public int amount = 46 * 3;

    public struct RVOAgent
    {
        public float density;
        public float pressure;

        public GameObject go;
    }
    public RVOAgent[] RVOAgents = new RVOAgent[5000];

    // Start is called before the first frame update
    void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }

        //RVOAgents = new RVOAgent[1];
        //InitNavAgent();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateRVO();
    }

    private void UpdateRVO()
    {
        for (int i = 0; i < RVOAgents.Length; i++)
        {
            if (RVOAgents[i].go != null)
            {
                SPHProperties sp = RVOAgents[i].go.GetComponent<SPHProperties>();

                //RVOAgents[i].density = sp.density;
                sp.density = RVOAgents[i].density;
                //RVOAgents[i].pressure = sp.pressure;
                sp.pressure = RVOAgents[i].pressure;
            }
        }
    }

    private void InitNavAgent()
    {
        for(int i = 0; i < amount/2; i++)
        {
            GameObject newAgent = Instantiate(charPrefab);
            newAgent.name += i;
            //newAgent.transform.position = new Vector3(-20.0f + (i % 46) * 0.2f, 20.0f, -19.5f + (i / 46) % 46 * 0.5f);
            //newAgent.transform.position = new Vector3(0.0f, 12.0f, 0.0f);
            newAgent.GetComponent<PlayerMovement>().target = GameObject.Find("SimpleTarget1").transform;
            newAgent.transform.parent = GameObject.Find("RVOAgents").transform;
            newAgent.GetComponent<NavMeshAgent>().enabled = true;
            newAgent.GetComponent<PlayerMovement>().enabled = true;
            SPHProperties sp = newAgent.GetComponent<SPHProperties>();
            sp.position = newAgent.transform.position;
            newAgent.SetActive(true);
        }
        for (int j = amount/2; j < amount; j++)
        {
            GameObject newAgent = Instantiate(charPrefab);
            newAgent.name += j;
            newAgent.transform.position = new Vector3(-20.0f + (j % 46) * 0.2f, 20.0f, 19.5f - (j / 46) % 46 * 0.5f);
            newAgent.GetComponent<PlayerMovement>().target = GameObject.Find("SimpleTarget1").transform;
            newAgent.transform.parent = GameObject.Find("RVOAgents").transform;
            newAgent.GetComponent<NavMeshAgent>().enabled = true;
            newAgent.GetComponent<PlayerMovement>().enabled = true;
            SPHProperties sp = newAgent.GetComponent<SPHProperties>();
            sp.position = newAgent.transform.position;
            newAgent.SetActive(true);
        }
    }
}

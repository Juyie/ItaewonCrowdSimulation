using System.Collections;
using System.Collections.Generic;
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
        /*
        if(RVOAgents.Length != GameObject.Find("RVOAgents").transform.childCount && SPHManager.RVO_SPH)
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

    private void InitNavAgent()
    {
        for(int i = 0; i < amount/2; i++)
        {
            GameObject go = Instantiate(charPrefab);
            go.transform.parent = GameObject.Find("RVOAgents").transform;
            go.transform.position = new Vector3(-4.5f + (i % 46) * 0.2f, 0.0f, -19.5f + (i / 46) % 46 * 0.5f);
            go.transform.GetChild(3).GetComponent<SkinnedMeshRenderer>().material.color = Color.red;
            go.GetComponent<NavagentController>().goalPos = goalPos1;
            RVOAgents[i].density = 0.0f;
            RVOAgents[i].pressure = 0.0f;
            RVOAgents[i].go = go;
            SPHProperties sp = go.GetComponent<SPHProperties>();
            sp.position = go.transform.position;
            sp.goalPosition = goalPos1;
        }
        for (int j = amount/2; j < amount; j++)
        {
            GameObject go = Instantiate(charPrefab);
            go.transform.parent = GameObject.Find("RVOAgents").transform;
            go.transform.position = new Vector3(-4.5f + (j % 46) * 0.2f, 0.0f, 19.5f - (j / 46) % 46 * 0.5f);
            go.transform.GetChild(3).GetComponent<SkinnedMeshRenderer>().material.color = Color.green;
            go.GetComponent<NavagentController>().goalPos = goalPos2;
            RVOAgents[j].density = 0.0f;
            RVOAgents[j].pressure = 0.0f;
            RVOAgents[j].go = go;
            SPHProperties sp = go.GetComponent<SPHProperties>();
            sp.position = go.transform.position;
            sp.goalPosition = goalPos2;
        }
    }
}

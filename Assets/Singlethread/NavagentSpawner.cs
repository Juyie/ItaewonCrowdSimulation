using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NavagentSpawner : MonoBehaviour
{
    public SPHManagerSingleThread SPHManager;

    private Vector3 goalPos = new Vector3(0.0f, 0.0f, 19.0f);

    [SerializeField]
    private GameObject charPrefab = null;

    public int amount = 46 * 3;

    public struct RVOAgent
    {
        public float density;
        public float pressure;

        public GameObject go;
    }
    public RVOAgent[] RVOAgents;

    // Start is called before the first frame update
    void Awake()
    {
        RVOAgents = new RVOAgent[amount];
        InitNavAgent();
    }

    // Update is called once per frame
    void Update()
    {
    }

    private void InitNavAgent()
    {
        for(int i = 0; i < amount; i++)
        {
            GameObject go = Instantiate(charPrefab);
            go.transform.parent = GameObject.Find("Agents").transform;
            go.transform.position = new Vector3(-4.5f + (i % 46) * 0.2f, 0.0f, -19.5f + (i / 46) % 46 * 0.5f);
            go.transform.GetChild(3).GetComponent<SkinnedMeshRenderer>().material.color = Color.red;
            go.GetComponent<NavMeshAgent>().destination = goalPos;
            RVOAgents[i].density = 0.0f;
            RVOAgents[i].pressure = 0.0f;
            RVOAgents[i].go = go;
        }
    }
}

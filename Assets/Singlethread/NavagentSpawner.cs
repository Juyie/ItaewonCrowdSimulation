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

    // Start is called before the first frame update
    void Start()
    {
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
        }
    }
}

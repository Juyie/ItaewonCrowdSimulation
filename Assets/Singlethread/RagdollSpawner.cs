using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RagdollSpawner : MonoBehaviour
{
    public SPHManagerSingleThread SPHManager;

    public struct RagdollAgent
    {
        public float density;
        public float pressure;

        public GameObject go;
    }
    public RagdollAgent[] RagdollAgents;

    private void Awake()
    {
        RagdollAgents = new RagdollAgent[0];
    }

    // Update is called once per frame
    void Update()
    {
        if(RagdollAgents.Length != GameObject.Find("RagdollAgents").transform.childCount)
        {
            UpdateRagdoll();
        }
    }

    private void UpdateRagdoll()
    {
        RagdollAgents = new RagdollAgent[GameObject.Find("RagdollAgents").transform.childCount];

        for (int i = 0; i < GameObject.Find("RagdollAgents").transform.childCount; i++)
        {
            GameObject go = GameObject.Find("RagdollAgents").transform.GetChild(i).gameObject;
            SPHProperties sp = go.GetComponent<SPHProperties>();

            RagdollAgents[i].density = sp.density;
            RagdollAgents[i].pressure = sp.pressure;
            RagdollAgents[i].go = go;
        }
    }
}

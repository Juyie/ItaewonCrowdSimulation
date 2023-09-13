using System.Collections;
using System.Collections.Generic;
using Unity.Physics.Systems;
using UnityEngine;

public class GridController : MonoBehaviour
{
    [SerializeField]
    private GameObject[] grids;

    public bool Step = true;
    public bool Exponential = false;
    public float stepSize = 0.01f;
    public float exponentialSize = 3.0f;

    public DisplayAgentNumber agentNumber;
    public bool setGrid = false;

    private bool once = true;

    // Start is called before the first frame update
    void Awake()
    {
        if (setGrid)
        {
            StartSimulation();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Step)
        {
            Exponential = false;
        }
        else
        {
            Exponential = true;
        }

        if(agentNumber.agentNumber > 1800 && once)
        {
            SaveAgentsData.Instance.JsonSave();
            once = false;
        }

        if (agentNumber.agentNumber > 1820)
        {
            StartSimulation();
        }
    }

    private void StartSimulation()
    {
        for(int i = 0;  i < grids.Length; i++)
        {
            grids[i].GetComponent<CalculateDensityGrid>().enabled = true;
        }
    }
}

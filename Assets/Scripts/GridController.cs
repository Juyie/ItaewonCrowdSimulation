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
    private bool on = true;
    private int count = 0;

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
        /*
        if (Step)
        {
            Exponential = false;
        }
        else
        {
            Exponential = true;
        }
        */

        if(agentNumber.agentNumber >= 4500 && once)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                SaveAgentsData.Instance.JsonSave();
                once = false;
            }
        }

        //if (Input.GetKeyDown(KeyCode.Alpha2) && on)
        if(agentNumber.agentNumber >= 2000 && on)
        {
            StartSimulation();
            on = false;
        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            on = false;
            StopSimulation();
        }
        CountSPH();
    }

    private void StartSimulation()
    {
        for(int i = 0;  i < grids.Length; i++)
        {
            grids[i].GetComponent<CalculateDensityGrid>().enabled = true;
        }
    }

    private void StopSimulation()
    {
        for (int i = 0; i < grids.Length; i++)
        {
            grids[i].GetComponent<CalculateDensityGrid>().enabled = false;
        }
    }

    private void CountSPH()
    {
        count = 0;
        for (int i = 0; i < grids.Length; i++)
        {
            if (grids[i].GetComponent<CalculateDensityGrid>().mySPH)
            {
                count++;
            }
        }

        if(count > 0)
        {
            for (int i = 0; i < grids.Length; i++)
            {
                grids[i].GetComponent<CalculateDensityGrid>().suffSPH = true;
            }
        }
    }
}

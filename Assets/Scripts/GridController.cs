using System.Collections;
using System.Collections.Generic;
using Unity.Physics.Systems;
using UnityEngine;

public class GridController : MonoBehaviour
{
    [SerializeField]
    private GameObject[] grids;

    public float stepSize = 0.01f;

    [HideInInspector]
    public DisplayAgentNumber agentNumber;

    private bool once = true;
    private bool on = true;
    private int count = 0;

    private void Awake()
    {
        StartSimulation();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            StopSimulation();
        }
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
}

using DataStructures.ViliWonka.KDTree;
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

    public GameObject[] RVOGameObject;
    public Vector3[] RVOPointCloud;
    public KDTree RVOKDTree;
    public int maxPointsPerLeafNode = 32;
    public int[] TypeOfSimulation;
    public int dataLength = 600;

    // Start is called before the first frame update
    void Awake()
    {
        RVOGameObject = new GameObject[dataLength];
        RVOPointCloud = new Vector3[dataLength];
        TypeOfSimulation = new int[dataLength];

        if (instance == null)
        {
            instance = this;
        }
    }

    private void Update()
    {
        if (SaveAgentsData.GetMakeAgentDone())
        {
            UpdateKDTree();
        }
    }

    private void UpdateKDTree()
    {
        for(int i = 0; i < RVOGameObject.Length; i++)
        {
            RVOPointCloud[i] = RVOGameObject[i].transform.position;
        }

        RVOKDTree.Rebuild();
    }
}

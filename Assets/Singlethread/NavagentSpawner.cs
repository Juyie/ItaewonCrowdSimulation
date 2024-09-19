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

    // Start is called before the first frame update
    void Awake()
    {
        RVOGameObject = new GameObject[2160];
        RVOPointCloud = new Vector3[2160];
        TypeOfSimulation = new int[2160];

        if (instance == null)
        {
            instance = this;
        }
    }

    private void Update()
    {
        UpdateKDTree();
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

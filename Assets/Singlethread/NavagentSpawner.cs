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
        RVOGameObject = new GameObject[100+1];
        RVOPointCloud = new Vector3[100+1];
        TypeOfSimulation = new int[100+1];

        if (instance == null)
        {
            instance = this;
        }

        if (GameObject.FindWithTag("Player") != null)
        {
            GameObject player = GameObject.FindWithTag("Player");
            RVOGameObject[100] = player;
            RVOPointCloud[100] = player.transform.position;
            TypeOfSimulation[100] = 0;
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

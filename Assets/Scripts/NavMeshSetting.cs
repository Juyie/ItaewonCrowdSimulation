using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NavMeshSetting : MonoBehaviour
{
    [SerializeField]
    private bool Default;

    [SerializeField]
    private float avoidanceTime = 2.0f;

    [SerializeField]
    private int pathfindingFrame = 100;
    // Start is called before the first frame update
    void Start()
    {
        if (!Default)
        {
            NavMesh.avoidancePredictionTime = avoidanceTime;
            NavMesh.pathfindingIterationsPerFrame = pathfindingFrame;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

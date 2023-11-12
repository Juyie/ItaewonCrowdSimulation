using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NavMeshSetting : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        NavMesh.avoidancePredictionTime = 1.0f;
        NavMesh.pathfindingIterationsPerFrame = 5000;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

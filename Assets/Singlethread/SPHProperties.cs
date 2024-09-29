using System.Collections;
using System.Collections.Generic;
using Unity.Transforms;
using UnityEngine;

public class SPHProperties : MonoBehaviour
{
    public Vector3 position;
    public Vector3 goalPosition;

    public Vector3 velocity;
    public Vector3 forcePhysic;
    public Vector3 forceHeading;
    public float totalForce;

    public float density;
    public float pressure;

    public int parameterID;

    public bool ragdollDensity = false;
    public bool SPHZombieDensity = false;

    public float goalForce = 50.0f;
    public float goalForceBefore;

    // Start is called before the first frame update
    void Start()
    {
        goalForce = Random.Range(50.0f, 100.0f);
        goalForceBefore = goalForce;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

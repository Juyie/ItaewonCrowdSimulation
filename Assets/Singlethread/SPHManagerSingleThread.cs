using DataStructures.ViliWonka.KDTree;
using Google.Protobuf.WellKnownTypes;
using Microsoft.Win32;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.Entities.UniversalDelegates;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.AI;

public class SPHManagerSingleThread : MonoBehaviour
{
    private static SPHManagerSingleThread instance = null;

    public static SPHManagerSingleThread Instance
    {
        get { return instance; }
    }

    public struct SPHParticle
    {
        public Vector3 position;
        public Vector3 goalPosition;

        public Vector3 velocity;
        public Vector3 forcePhysic;
        public Vector3 forceHeading;

        public float density;
        public float pressure;

        public int parameterID;

        public GameObject go;

        public void Init(Vector3 _position, Vector3 _goalPosition, int _parameterID, GameObject _go)
        {
            go = _go;
            SPHProperties sp = go.GetComponent<SPHProperties>();
            sp.position = _position;
            sp.parameterID = _parameterID;
            sp.goalPosition = _goalPosition;

            sp.velocity = Vector3.zero;
            sp.forcePhysic = Vector3.zero;
            sp.forceHeading = Vector3.zero;
            sp.density = 0.0f;
            sp.pressure = 0.0f;

            go.GetComponent<Animator>().enabled = true;
            go.GetComponent<Animator>().SetBool("isWalking", false);
        }
    }

    [System.Serializable]
    public struct SPHParameters
    {
#pragma warning disable 0649 // This line removes the warning saying that the variable is never assigned to. You can't assign a variable in a struct...
        public float particleRadius;
        public float smoothingRadius;
        public float smoothingRadiusSq;
        public float restDensity;
        public float gravityMult;
        public float particleMass;
        public float particleViscosity;
        public float particleDrag;
#pragma warning restore 0649
    }

    private struct SPHCollider
    {
        public GameObject go;
        public Vector3 position;
        public Vector3 right;
        public Vector3 up;
        public Vector3 scale;

        public void Init(Transform _transform)
        {
            go = _transform.gameObject;
            position = _transform.position;
            right = _transform.right;
            up = _transform.up;
            scale = new Vector3(_transform.lossyScale.x / 2f, _transform.lossyScale.y / 2f, _transform.lossyScale.z / 2f);
        }
    }


    // Consts
    private static Vector3 GRAVITY = new Vector3(0.0f, -9.81f, 0.0f);
    private const float GAS_CONST = 100.0f;
    private float maxAcceleration = 5.0f;
    public float maxVelocity = 0.6f;

    [Header("Parameters")]
    [SerializeField] private int parameterID = 0;
    [SerializeField] public SPHParameters[] parameters = null;

    [Header("Properties")]
    [SerializeField] private int amount = 250;
    [SerializeField] private int rowSize = 16;

    private GameObject[] RVOGameObject;
    private Vector3[] RVOPointCloud;
    private KDTree RVOKDTree;
    private KDQuery query;
    private int[] TypeOfSimulation;
    private float tempForce = 4000f;
    private bool makeAgentDone = false;

    [Header("Interaction")]
    [SerializeField] public bool RVO_SPH;
    [SerializeField] public bool SPH_RAGDOLL;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    private void Start()
    {
        query = new KDQuery();
    }

    private void FixedUpdate()
    {
        if (SaveAgentsData.GetMakeAgentDone())
        {
            RVOGameObject = NavagentSpawner.Instance.RVOGameObject;
            RVOPointCloud = NavagentSpawner.Instance.RVOPointCloud;
            RVOKDTree = NavagentSpawner.Instance.RVOKDTree;
            TypeOfSimulation = NavagentSpawner.Instance.TypeOfSimulation;

            ComputeDensityPressure();
            ComputeForces();
            Integrate();

            CheckVelocityForAnimation();

            if (SPH_RAGDOLL)
            {
                for (int i = 0; i < RVOGameObject.Length; i++)
                {
                    if (TypeOfSimulation[i] == 1)
                    {
                        if (RVOGameObject[i].GetComponent<SPHProperties>().forcePhysic.magnitude > tempForce && RVOGameObject[i].GetComponent<SPHProperties>().ragdollDensity == true && RVOGameObject[i].GetComponent<SPHProperties>().goalForce == 0)
                        {
                            TurnOnRagdolls(RVOGameObject[i]);
                        }
                    }
                }
            }
        }
    }

    private void Integrate()
    {
        for (int i = 0; i < RVOGameObject.Length; i++)
        {
            // Only SPH agents
            if (TypeOfSimulation[i] == 1)
            {
                SPHProperties sp = RVOGameObject[i].GetComponent<SPHProperties>();
                
                RaycastHit raydata;
                int layerMask = 1 << LayerMask.NameToLayer("NavCollider");
                float yPos = sp.position.y;
                if (Physics.Raycast(sp.gameObject.transform.position, -sp.gameObject.transform.up, out raydata, 10.0f, layerMask))
                {
                    yPos = raydata.point.y;
                }
                else if (Physics.Raycast(sp.gameObject.transform.position, sp.gameObject.transform.up, out raydata, 10.0f, layerMask))
                {
                    yPos = raydata.point.y;
                }

                
                Vector3 maxAcc = Vector3.ClampMagnitude(sp.forcePhysic / parameters[sp.parameterID].particleMass, maxAcceleration);
                RVOGameObject[i].GetComponent<Rigidbody>().velocity += maxAcc * Time.fixedDeltaTime;
                RVOGameObject[i].GetComponent<Rigidbody>().velocity = Vector3.ClampMagnitude(RVOGameObject[i].GetComponent<Rigidbody>().velocity, maxVelocity);

                sp.velocity = RVOGameObject[i].GetComponent<Rigidbody>().velocity;
                RVOGameObject[i].transform.position = new Vector3(RVOGameObject[i].transform.position.x, yPos, RVOGameObject[i].transform.position.z);
                sp.position = RVOGameObject[i].transform.position;
            }
        }
    }


    private void ComputeDensityPressure()
    {
        for (int i = 0; i < RVOGameObject.Length; i++)
        {
            SPHProperties spi = RVOGameObject[i].GetComponent<SPHProperties>();
            spi.density = 0.0f;

            List<int> results = new List<int>();
            query.Radius(RVOKDTree, spi.position, parameters[parameterID].smoothingRadius, results);

            for (int k = 0; k < results.Count; k++)
            {
                Vector2 rij = new Vector2(RVOPointCloud[results[k]].x - spi.position.x, RVOPointCloud[results[k]].z - spi.position.z);
                float distanceSquared = rij.sqrMagnitude;
                float diff = parameters[spi.parameterID].smoothingRadiusSq - distanceSquared;

                spi.density += parameters[spi.parameterID].particleMass * (4.0f / (Mathf.PI * Mathf.Pow(parameters[spi.parameterID].smoothingRadius, 8.0f))) * Mathf.Pow(diff, 3.0f);
            }

            // add the agent itself
            spi.density += parameters[spi.parameterID].particleMass * (4.0f / (Mathf.PI * Mathf.Pow(parameters[spi.parameterID].smoothingRadius, 8.0f))) * Mathf.Pow(parameters[spi.parameterID].smoothingRadiusSq, 3.0f);

            // compute pressure
            spi.pressure = GAS_CONST * (spi.density - parameters[spi.parameterID].restDensity * (parameters[0].particleMass + parameters[1].particleMass) / 2);
        }
    }

    private void ComputeForces()
    {
        for (int i = 0; i < RVOGameObject.Length; i++)
        {
            Vector2 forcePressure = Vector2.zero;
            Vector2 forceViscosity = Vector2.zero;
            SPHProperties spi = RVOGameObject[i].GetComponent<SPHProperties>();

            // Physics
            List<int> results = new List<int>();
            query.Radius(RVOKDTree, spi.position, parameters[parameterID].smoothingRadius, results);

            for (int k = 0; k < results.Count; k++)
            {
                Vector2 rij = new Vector2(RVOPointCloud[results[k]].x - spi.position.x, RVOPointCloud[results[k]].z - spi.position.z);
                float distanceSquared = rij.sqrMagnitude;
                float dist = Mathf.Sqrt(distanceSquared);
                if (dist != 0.0f)
                {
                    float rangeMinDist = parameters[spi.parameterID].smoothingRadius - dist;

                    SPHProperties spk = RVOGameObject[results[k]].GetComponent<SPHProperties>();
                    forcePressure += (parameters[spi.parameterID].particleMass * (spi.pressure + spk.pressure) / (2.0f * spk.density) * (-30.0f / (Mathf.PI * Mathf.Pow(parameters[spi.parameterID].smoothingRadius, 5.0f))) * rangeMinDist * rangeMinDist / dist * new Vector2(spk.position.x - spi.position.x, spk.position.z - spi.position.z));
                    forceViscosity += (parameters[spi.parameterID].particleViscosity * parameters[spi.parameterID].particleMass * (360.0f / (29.0f * Mathf.PI * Mathf.Pow(parameters[spi.parameterID].smoothingRadius, 5.0f))) * rangeMinDist / spk.density * (new Vector2(RVOGameObject[k].GetComponent<PlayerMovement>().velocity.x - spi.velocity.x, RVOGameObject[k].GetComponent<PlayerMovement>().velocity.z - spi.velocity.z)));
                }
            }

            Vector3 forceGravity = GRAVITY * parameters[spi.parameterID].gravityMult * spi.density * parameters[spi.parameterID].particleMass;
            Vector3 goalNorm;
            Vector3 rotation;

            goalNorm = (spi.goalPosition - spi.position).normalized;
            rotation = new Vector3(0.0f, spi.forcePhysic.normalized.y + 180.0f, 0.0f);

            Vector3 forceGoal = goalNorm * spi.goalForce * spi.density;
            
            Vector3 Impulse1 = new Vector3(0.0f, 0.0f, -50.0f);
            Vector3 Impulse2 = new Vector3(0.0f, 0.0f, 50.0f);
            Vector3 Impulse3 = new Vector3(-50.0f, 0.0f, 0.0f);
            Vector3 Impulse4 = new Vector3(-10.0f, 0.0f, 0.0f);
            Vector3 Impulse5 = new Vector3(-300.0f, 0.0f, 0.0f);

            float forceX = forcePressure.x + forceViscosity.x + forceGoal.x + forceGravity.x;
            float forceZ = forcePressure.y + forceViscosity.y +forceGoal.z + forceGravity.z;

            spi.forcePhysic = (new Vector3(forceX, forceGravity.y, forceZ) / spi.density);
        }
    }

    private void CheckVelocityForAnimation()
    {
        for (int i = 0; i < RVOGameObject.Length; i++)
        {
            if (TypeOfSimulation[i] == 1)
            {
                if (RVOGameObject[i].GetComponent<SPHProperties>().velocity.magnitude > 0.1f)
                {
                    RVOGameObject[i].GetComponent<Animator>().SetBool("isWalking", true);
                    RVOGameObject[i].GetComponent<Animator>().speed = RVOGameObject[i].GetComponent<SPHProperties>().velocity.magnitude / maxVelocity;
                }
                else
                {
                    RVOGameObject[i].GetComponent<Animator>().SetBool("isWalking", false);
                }
            }
        }
    }

    public void TurnOnRagdolls(GameObject agent)
    {
        agent.GetComponent<OnOffRagdoll>().TurnOnRagdoll();
        agent.transform.parent = GameObject.Find("RagdollAgents").transform;
        NavagentSpawner.Instance.TypeOfSimulation[int.Parse(agent.name.Substring(23))] = 2;
    }
}

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
    private const float DT = 0.0008f;
    private const float BOUND_DAMPING = -0.5f;
    private Vector3 goalPos1 = new Vector3(0.0f, 0.0f, 19.5f);
    private Vector3 goalPos2 = new Vector3(0.0f, 0.0f, -19.5f);
    private float maxAcceleration = 5.0f;
    public float maxVelocity = 0.6f;

    // Properties
    [Header("Import")]
    [SerializeField] private GameObject character0Prefab = null;

    [Header("Parameters")]
    [SerializeField] private int parameterID = 0;
    [SerializeField] public SPHParameters[] parameters = null;

    [Header("Properties")]
    [SerializeField] private int amount = 250;
    [SerializeField] private int rowSize = 16;

    // Data
    public SPHParticle[] particles = new SPHParticle[600];

    private bool addForce = false;
    private bool addForceFlag = false;
    private bool addForceShort = false;
    private bool addForceFlagShort = true;

    private RagdollSpawner.RagdollAgent[] RagdollAgents;

    private GameObject[] RVOGameObject;
    private Vector3[] RVOPointCloud;
    private KDTree RVOKDTree;
    private KDQuery query;
    private int[] TypeOfSimulation;

    private float friction = 4.0f;
    private float torqueForce = 30.622f;
    // 3500 is too low
    // 4500 is low too
    // 1000is too high
    // 800
    private float tempForce = 4000f;

    [Header("Interaction")]
    [SerializeField] public bool RVO_SPH;
    [SerializeField] public bool SPH_RAGDOLL;

    private void Awake()
    {
        //InitSPH();
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
        RVOGameObject = NavagentSpawner.Instance.RVOGameObject;
        RVOPointCloud = NavagentSpawner.Instance.RVOPointCloud;
        RVOKDTree = NavagentSpawner.Instance.RVOKDTree;
        TypeOfSimulation = NavagentSpawner.Instance.TypeOfSimulation;

        ComputeDensityPressure();
        ComputeForces();
        Integrate();

        //ComputeColliders();
        //ComputeCollisions();
        //ApplyPosition();

        CheckVelocityForAnimation();

        if (RVO_SPH)
        {
        }
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

        /*
        if (Input.GetKeyDown(KeyCode.F))
        {
            addForce = true;
        }
        if (Input.GetKeyUp(KeyCode.F))
        {
            addForce = false;
        }
        if (Input.GetKeyDown(KeyCode.F1))
        {
            PrintPositions();
        }
        */
        /*
        if (particles.Length != GameObject.Find("SPHAgents").transform.childCount)
        {
            UpdateSPH();
        }
        */

        if (addForceFlag)
        {
            addForceFlag = false;
            StartCoroutine(WaitAndAddForce());
        }
        else
        {
            StartCoroutine(TurnOnAddForceFlag());
        }

        if (addForceFlagShort)
        {
            addForceFlagShort = false;
            StartCoroutine(WaitAndAddForceShort());
        }
    }

    private void InitSPH()
    {
        particles = new SPHParticle[amount];

        for (int i = 0; i < amount; i++)
        {
            GameObject go = Instantiate(character0Prefab);
            go.transform.parent = GameObject.Find("SPHAgents").transform;
            go.transform.position = new Vector3(-4.5f + (i % 46) * 0.2f, 0.0f, 19.5f - (i / 46) % 46 * 0.5f);
            go.transform.GetChild(3).GetComponent<SkinnedMeshRenderer>().material.color = Color.blue;
            go.name = "char" + i.ToString();
            go.GetComponent<NavMeshAgent>().enabled = false;
            go.GetComponent<NavMeshObstacle>().enabled = true;

            Vector3 goalPosition = new Vector3(UnityEngine.Random.Range(-4.5f, 4.5f), 0.0f, goalPos2.z);

            particles[i].Init(go.transform.position, goalPosition, parameterID, go);
        }
    }

    private void UpdateSPH()
    {
        particles = new SPHParticle[GameObject.Find("SPHAgents").transform.childCount];

        for (int i = 0; i < GameObject.Find("SPHAgents").transform.childCount; i++)
        {
            GameObject go = GameObject.Find("SPHAgents").transform.GetChild(i).gameObject;
            SPHProperties sp = go.GetComponent<SPHProperties>();

            particles[i].Init(sp.position, sp.goalPosition, sp.parameterID, go);
        }
    }

    private static bool Intersect(SPHCollider collider, Vector3 position, float radius, out Vector3 penetrationNormal, out Vector3 penetrationPosition, out float penetrationLength)
    {
        Vector3 colliderProjection = collider.position - position;

        penetrationNormal = Vector3.Cross(collider.right, collider.up);
        penetrationLength = Mathf.Abs(Vector3.Dot(colliderProjection, penetrationNormal * collider.scale.z)) - (radius / 2.0f);
        penetrationPosition = collider.position - colliderProjection;

        return penetrationLength < 0.0f
            && Mathf.Abs(Vector3.Dot(colliderProjection, collider.right)) < collider.scale.x
            && Mathf.Abs(Vector3.Dot(colliderProjection, collider.up)) < collider.scale.y;
    }


    private static Vector3 DampVelocity(SPHCollider collider, Vector3 velocity, Vector3 penetrationNormal, float drag)
    {
        Vector3 newVelocity = Vector3.Dot(velocity, penetrationNormal) * penetrationNormal * BOUND_DAMPING
                            + Vector3.Dot(velocity, collider.right) * collider.right * drag
                            + Vector3.Dot(velocity, collider.up) * collider.up * drag;
        return newVelocity;
    }

    private static Vector3 DampVelocity(Vector3 velocity, Vector3 penetrationNormal, float drag)
    {
        Vector3 newVelocity = Vector3.Dot(velocity, penetrationNormal) * penetrationNormal * BOUND_DAMPING
                            + Vector3.Dot(velocity, Vector3.right) * Vector3.right * drag
                            + Vector3.Dot(velocity, Vector3.up) * Vector3.up * drag;
        return newVelocity;
    }


    private void ComputeColliders()
    {
        // Get colliders
        GameObject[] collidersGO = GameObject.FindGameObjectsWithTag("SPHCollider");
        SPHCollider[] colliders = new SPHCollider[collidersGO.Length];
        for (int i = 0; i < colliders.Length; i++)
        {
            colliders[i].Init(collidersGO[i].transform);
        }

        for (int i = 0; i < RVOGameObject.Length; i++)
        {
            if (TypeOfSimulation[i] == 1)
            {
                for (int j = 0; j < colliders.Length; j++)
                {
                    // Check collision
                    Vector3 penetrationNormal;
                    Vector3 penetrationPosition;
                    float penetrationLength;

                    SPHProperties sp = RVOGameObject[i].GetComponent<SPHProperties>();
                    if (Intersect(colliders[j], sp.position, parameters[sp.parameterID].particleRadius, out penetrationNormal, out penetrationPosition, out penetrationLength))
                    {
                        //sp.velocity = DampVelocity(colliders[j], sp.velocity, penetrationNormal, 1.0f - parameters[sp.parameterID].particleDrag);
                        //sp.position = penetrationPosition - penetrationNormal * Mathf.Abs(penetrationLength);
                        RVOGameObject[i].GetComponent<Rigidbody>().velocity = DampVelocity(colliders[j], RVOGameObject[i].GetComponent<Rigidbody>().velocity, penetrationNormal, 1.0f - parameters[sp.parameterID].particleDrag);
                        sp.velocity = RVOGameObject[i].GetComponent<Rigidbody>().velocity;
                        RVOGameObject[i].transform.position = penetrationPosition - penetrationNormal * Mathf.Abs(penetrationLength);
                        sp.position = RVOGameObject[i].transform.position;
                    }
                }
            }
        }
    }

    private void ComputeCollisions()
    {
        for (int i = 0; i < RVOGameObject.Length; i++)
        {
            if (TypeOfSimulation[i] == 1)
            {
                SPHProperties spi = RVOGameObject[i].GetComponent<SPHProperties>();
                List<int> results = new List<int>();
                query.Radius(RVOKDTree, spi.position, parameters[parameterID].particleRadius / 2, results);
                for (int j = 0; j < results.Count; j++)
                {
                    SPHProperties spj = RVOGameObject[j].GetComponent<SPHProperties>();

                    Vector3 penetrationNormal = (spj.position - spi.position).normalized;
                    float penetrationLength = -(spj.position - spi.position).magnitude + parameters[parameterID].particleRadius / 2;
                    Debug.Log(penetrationLength);
                    //spi.velocity = DampVelocity(spi.velocity, penetrationNormal, 1.0f - parameters[spi.parameterID].particleDrag);
                    spi.position -= penetrationNormal * Mathf.Abs(penetrationLength) * Time.fixedDeltaTime / results.Count;
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
                //RVOGameObject[i].GetComponent<Rigidbody>().AddForce(maxAcc, ForceMode.Acceleration);
                RVOGameObject[i].GetComponent<Rigidbody>().velocity += maxAcc * Time.fixedDeltaTime;
                RVOGameObject[i].GetComponent<Rigidbody>().velocity = Vector3.ClampMagnitude(RVOGameObject[i].GetComponent<Rigidbody>().velocity, maxVelocity);

                //if (addForce && RVOGameObject[i].transform.position.x >= -5f && RVOGameObject[i].transform.position.x <= 5f && RVOGameObject[i].transform.position.z >= -17f && RVOGameObject[i].transform.position.z <= -15f)
                if(addForce && i % 101 == 0)
                {
                    RVOGameObject[i].GetComponent<Rigidbody>().AddForce(new Vector3(0, 0, -500f), ForceMode.Impulse);
                }

                sp.velocity = RVOGameObject[i].GetComponent<Rigidbody>().velocity;
                RVOGameObject[i].transform.position = new Vector3(RVOGameObject[i].transform.position.x, yPos, RVOGameObject[i].transform.position.z);
                sp.position = RVOGameObject[i].transform.position;
                /*
                Vector3 a = Vector3.ClampMagnitude(sp.forcePhysic / parameters[sp.parameterID].particleMass, maxAcceleration);
                sp.velocity += a * Time.fixedDeltaTime;
                sp.velocity = Vector3.ClampMagnitude(sp.velocity, maxVelocity);
                sp.position += sp.velocity * Time.fixedDeltaTime;
                sp.position.y = yPos;
                RVOGameObject[i].GetComponent<Rigidbody>().position = sp.position;
                */
                //Debug.Log("Pos after: " + sp.position + ", Y pos: " + yPos);
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

            //Vector3 Impulse1 = new Vector3(0.0f, 0.0f, 0.0f);
            //Vector3 Impulse2 = new Vector3(0.0f, 0.0f, 0.0f);
            //Vector3 Impulse3 = new Vector3(0.0f, 0.0f, 0.0f);

            // Apply

            float forceX = forcePressure.x + forceViscosity.x + forceGoal.x + forceGravity.x;
            float forceZ = forcePressure.y + forceViscosity.y +forceGoal.z + forceGravity.z;

            spi.forcePhysic = (new Vector3(forceX, forceGravity.y, forceZ) / spi.density);


            //spi.forcePhysic = new Vector3(forceX, 0.0f, forceZ) / spi.density;

            
            //Debug.Log("Y: " + spi.forcePhysic.y);
            
            /*
            if (addForce && spi.position.x > 10.0f)
            {
                //Debug.Log("Force");
                if (spi.position.z > 8.0f && spi.position.z < 10.0f)
                {
                    spi.forcePhysic += Impulse1;
                }
                else if(spi.position.z < 2.6f && spi.position.z > 0.6f)
                {
                    spi.forcePhysic += Impulse2;
                }
            }
            if (addForce && (spi.position.x > 32.0f))// && spi.position.x < 40.0f || spi.position.x > 42.0f && spi.position.x < 45.0f))
            {
                if (spi.position.z < 7.42 && spi.position.z > 2.74)
                {
                    spi.forcePhysic += Impulse3;
                }

                //if (spi.position.z < 5.8 && spi.position.z > 3.3 && spi.position.x < 34.0f)
                //{
                    //spi.forcePhysic += Impulse5;
                //}
            }

            if (addForceShort)
            {
                if (spi.position.x > 43.5f && spi.position.x < 45.5f && spi.position.z > 2.0f && spi.position.z < 6.0f)
                {
                    spi.forcePhysic += Impulse4;
                }
            }
            */
            //Debug.Log("Force: " + spi.forcePhysic);
        }
    }

    private void ApplyPosition()
    {
        for (int i = 0; i < RVOGameObject.Length; i++)
        {
            if (TypeOfSimulation[i] == 1)
            {
                RVOGameObject[i].transform.position = RVOGameObject[i].GetComponent<SPHProperties>().position;// + new Vector3(0.0f, parameters[0].particleRadius / 2, 0.0f);
            }
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

    private void PrintPositions()
    {
        for (int i = 0; i < RVOGameObject.Length; i++)
        {
            if (TypeOfSimulation[i] == 1)
            {
                if (RVOGameObject[i].GetComponent<SPHProperties>().position.x == RVOGameObject[i].transform.position.x && RVOGameObject[i].GetComponent<SPHProperties>().position.z == RVOGameObject[i].transform.position.z)
                {
                    Debug.Log("Same Position");
                }
                else if (RVOGameObject[i].GetComponent<SPHProperties>().position.x != RVOGameObject[i].transform.position.x || RVOGameObject[i].GetComponent<SPHProperties>().position.z != RVOGameObject[i].transform.position.z)
                {
                    Debug.Log("Diff position. SPH position: " + RVOGameObject[i].GetComponent<SPHProperties>().position + ", GO position: " + RVOGameObject[i].transform.position);
                }
            }
        }
    }

    public void TurnOnRagdolls(GameObject agent)
    {
        agent.GetComponent<OnOffRagdoll>().TurnOnRagdoll();
        agent.transform.parent = GameObject.Find("RagdollAgents").transform;
        NavagentSpawner.Instance.TypeOfSimulation[int.Parse(agent.name.Substring(23))] = 2;
        //agent.GetComponent<SPHProperties>().parameterID = 2;
    }

    public void TurnOnSPHZombies(GameObject agent)
    {
        agent.GetComponent<SPHProperties>().goalForce = 0;
        agent.transform.GetChild(0).GetComponent<SkinnedMeshRenderer>().material.color = Color.green;
    }
    public void TurnOffSPHZombies(GameObject agent)
    {
        agent.GetComponent<SPHProperties>().goalForce = agent.GetComponent<SPHProperties>().goalForceBefore;
        agent.transform.GetChild(0).GetComponent<SkinnedMeshRenderer>().material.color = Color.yellow;
    }

    IEnumerator WaitAndAddForce()
    {
        addForce = true;
        yield return new WaitForSeconds(0.5f);
        addForce = false;
        yield return new WaitForSeconds(9.5f);
        addForceFlag = true;
    }

    IEnumerator WaitAndAddForceShort()
    {
        addForceShort = true;
        yield return new WaitForSeconds(1.0f);
        addForceShort = false;
        yield return new WaitForSeconds(0.5f);
        addForceFlagShort = true;
    }

    IEnumerator TurnOnAddForceFlag()
    {
        yield return new WaitForSeconds(120f);
        addForceFlag = true;
    }
}

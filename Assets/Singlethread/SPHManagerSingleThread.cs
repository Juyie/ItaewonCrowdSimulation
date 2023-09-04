using Microsoft.Win32;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.AI;

public class SPHManagerSingleThread : MonoBehaviour
{
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
        public Vector3 position;
        public Vector3 right;
        public Vector3 up;
        public Vector2 scale;

        public void Init(Transform _transform)
        {
            position = _transform.position;
            right = _transform.right;
            up = _transform.up;
            scale = new Vector2(_transform.lossyScale.x / 2f, _transform.lossyScale.y / 2f);
        }
    }


    // Consts
    private static Vector3 GRAVITY = new Vector3(0.0f, -9.81f, 0.0f);
    private const float GAS_CONST = 2000.0f;
    private const float DT = 0.0008f;
    private const float BOUND_DAMPING = -0.5f;
    public float goalPower = 1000f;
    private Vector3 goalPos1 = new Vector3(0.0f, 0.0f, 19.5f);
    private Vector3 goalPos2 = new Vector3(0.0f, 0.0f, -19.5f);

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
    public SPHParticle[] particles;

    private bool addForce = false;

    private NavagentSpawner.RVOAgent[] RVOagents;
    private RagdollSpawner.RagdollAgent[] RagdollAgents;

    [Header("Interaction")]
    [SerializeField] private bool RVO_SPH;
    [SerializeField] private bool SPH_RAGDOLL;

    private void Awake()
    {
        InitSPH();
    }

    private void Start()
    {
        RVOagents = GameObject.Find("RVOManager").GetComponent<NavagentSpawner>().RVOAgents;
        RagdollAgents = GameObject.Find("RagdollManager").GetComponent<RagdollSpawner>().RagdollAgents;
    }

    private void Update()
    {
        ComputeDensityPressure();
        ComputeForces();
        Integrate();
        ComputeColliders();

        ApplyPosition();

        CheckVelocityForAnimation();

        if (Input.GetKey(KeyCode.Space))
        {
            addForce = true;
        }
        else
        {
            addForce = false;
        }

        /*
        if (Input.GetKey(KeyCode.X))
        {
            //RVOagents = new NavagentSpawner.RVOAgent[0];
            for(int i = 0; i < RVOagents.Length; i++)
            {
                if (RVOagents[i].go.transform.GetChild(3).GetComponent<SkinnedMeshRenderer>().material.color == Color.green)
                {
                    RVOagents[i].go.GetComponent<NavMeshAgent>().enabled = false;
                    RVOagents[i].go.GetComponent<NavMeshObstacle>().enabled = true;
                    RVOagents[i].go.GetComponent<SPHProperties>().position = RVOagents[i].go.transform.position;
                    RVOagents[i].go.transform.parent = GameObject.Find("SPHAgents").transform;
                }
            }
        }
        */

        RVOagents = GameObject.Find("RVOManager").GetComponent<NavagentSpawner>().RVOAgents;
        RagdollAgents = GameObject.Find("RagdollManager").GetComponent<RagdollSpawner>().RagdollAgents;

        if (particles.Length != GameObject.Find("SPHAgents").transform.childCount)
        {
            UpdateSPH();
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
        penetrationLength = Mathf.Abs(Vector3.Dot(colliderProjection, penetrationNormal)) - (radius / 2.0f);
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
        newVelocity = Vector3.Dot(newVelocity, Vector3.forward) * Vector3.forward
                    + Vector3.Dot(newVelocity, Vector3.right) * Vector3.right
                    + Vector3.Dot(newVelocity, Vector3.up) * Vector3.up;
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

        for (int i = 0; i < particles.Length; i++)
        {
            for (int j = 0; j < colliders.Length; j++)
            {
                // Check collision
                Vector3 penetrationNormal;
                Vector3 penetrationPosition;
                float penetrationLength;

                SPHProperties sp = particles[i].go.GetComponent<SPHProperties>();
                if (Intersect(colliders[j], sp.position, parameters[sp.parameterID].particleRadius, out penetrationNormal, out penetrationPosition, out penetrationLength))
                {
                    sp.velocity = DampVelocity(colliders[j], sp.velocity, penetrationNormal, 1.0f - parameters[sp.parameterID].particleDrag);
                    sp.position = penetrationPosition - penetrationNormal * Mathf.Abs(penetrationLength);
                }
            }
        }
    }



    private void Integrate()
    {
        for (int i = 0; i < particles.Length; i++)
        {
            SPHProperties sp = particles[i].go.GetComponent<SPHProperties>();
            sp.velocity += DT * (sp.forcePhysic) / sp.density;
            if(sp.velocity.sqrMagnitude >= 1.5f)
            {
                sp.velocity = sp.velocity.normalized * 1.5f;
            }
            sp.position += DT * (sp.velocity );
        }
    }


    private void ComputeDensityPressure()
    {
        for (int i = 0; i < particles.Length; i++)
        {
            SPHProperties spi = particles[i].go.GetComponent<SPHProperties>();
            spi.density = 0.0f;

            // SPH Agents
            for (int j = 0; j < particles.Length; j++)
            {
                if (particles[i].go != null)
                {
                    SPHProperties spj = particles[j].go.GetComponent<SPHProperties>();
                    float xij = Mathf.Abs(spj.position.x - spi.position.x);
                    float zij = Mathf.Abs(spj.position.z - spi.position.z);
                    if (xij < parameters[spi.parameterID].smoothingRadiusSq && zij < parameters[spi.parameterID].smoothingRadiusSq)
                    {
                        Vector2 rij = new Vector2(spj.position.x - spi.position.x, spj.position.z - spi.position.z);
                        float r2 = rij.sqrMagnitude;

                        if (r2 < parameters[spi.parameterID].smoothingRadiusSq)
                        {
                            spi.density += parameters[spi.parameterID].particleMass * (315.0f / (64.0f * Mathf.PI * Mathf.Pow(parameters[spi.parameterID].smoothingRadius, 9.0f))) * Mathf.Pow(parameters[spi.parameterID].smoothingRadiusSq - r2, 3.0f);
                        }
                    }
                }
            }

            // RVO Agents
            if (RVO_SPH)
            {
                for (int k = 0; k < RVOagents.Length; k++)
                {
                    if (RVOagents[k].go != null)
                    {
                        float xij = Mathf.Abs(RVOagents[k].go.transform.position.x - spi.position.x);
                        float zij = Mathf.Abs(RVOagents[k].go.transform.position.z - spi.position.z);
                        if (xij < parameters[spi.parameterID].smoothingRadiusSq && zij < parameters[spi.parameterID].smoothingRadiusSq)
                        {
                            Vector2 rij = new Vector2(RVOagents[k].go.transform.position.x - spi.position.x, RVOagents[k].go.transform.position.z - spi.position.z);
                            float r2 = rij.sqrMagnitude;

                            if (r2 < parameters[spi.parameterID].smoothingRadiusSq)
                            {
                                spi.density += parameters[spi.parameterID].particleMass * (315.0f / (64.0f * Mathf.PI * Mathf.Pow(parameters[spi.parameterID].smoothingRadius, 9.0f))) * Mathf.Pow(parameters[spi.parameterID].smoothingRadiusSq - r2, 3.0f);
                            }
                        }
                    }
                }
            }

            // Ragdoll Agents
            if (SPH_RAGDOLL)
            {
                for (int l = 0; l < RagdollAgents.Length; l++)
                {
                    if (RagdollAgents[l].go != null)
                    {
                        float xij = Mathf.Abs(RagdollAgents[l].go.transform.position.x - spi.position.x);
                        float zij = Mathf.Abs(RagdollAgents[l].go.transform.position.z - spi.position.z);
                        if (xij < parameters[spi.parameterID].smoothingRadiusSq && zij < parameters[spi.parameterID].smoothingRadiusSq)
                        {
                            Vector2 rij = new Vector2(RagdollAgents[l].go.transform.position.x - spi.position.x, RagdollAgents[l].go.transform.position.z - spi.position.z);
                            float r2 = rij.sqrMagnitude;

                            if (r2 < parameters[spi.parameterID].smoothingRadiusSq)
                            {
                                spi.density += parameters[spi.parameterID].particleMass * (315.0f / (64.0f * Mathf.PI * Mathf.Pow(parameters[spi.parameterID].smoothingRadius, 9.0f))) * Mathf.Pow(parameters[spi.parameterID].smoothingRadiusSq - r2, 3.0f);
                            }
                        }
                    }
                }
            }

            spi.pressure = GAS_CONST * (spi.density - parameters[spi.parameterID].restDensity);
        }
    }



    private void ComputeForces()
    {
        for (int i = 0; i < particles.Length; i++)
        {
            Vector2 forcePressure = Vector2.zero;
            Vector2 forceViscosity = Vector2.zero;
            SPHProperties spi = particles[i].go.GetComponent<SPHProperties>();

            // Physics
            // SPH Agents
            for (int j = 0; j < particles.Length; j++)
            {
                SPHProperties spj = particles[j].go.GetComponent<SPHProperties>();
                if (i == j) continue;

                float xij = Mathf.Abs(spj.position.x - spi.position.x);
                float zij = Mathf.Abs(spj.position.z - spi.position.z);
                if (xij < parameters[spi.parameterID].smoothingRadiusSq && zij < parameters[spi.parameterID].smoothingRadiusSq)
                {
                    Vector2 rij = new Vector2(spj.position.x - spi.position.x, spj.position.z - spi.position.z);
                    float r2 = rij.sqrMagnitude;
                    float r = Mathf.Sqrt(r2);

                    if (r < parameters[spi.parameterID].smoothingRadius)
                    {
                        forcePressure += -rij.normalized * parameters[spi.parameterID].particleMass * (spi.pressure +spj.pressure) / (2.0f * spj.density) * (-45.0f / (Mathf.PI * Mathf.Pow(parameters[spi.parameterID].smoothingRadius, 6.0f))) * Mathf.Pow(parameters[spi.parameterID].smoothingRadius - r, 2.0f);
                        //forceViscosity += parameters[particles[i].parameterID].particleViscosity * parameters[particles[i].parameterID].particleMass * (particles[j].velocity - particles[i].velocity) / particles[j].density * (45.0f / (Mathf.PI * Mathf.Pow(parameters[particles[i].parameterID].smoothingRadius, 6.0f))) * (parameters[particles[i].parameterID].smoothingRadius - r);
                        forceViscosity += parameters[spi.parameterID].particleViscosity * parameters[spi.parameterID].particleMass * (new Vector2(spj.velocity.x - spi.velocity.x, spj.velocity.z - spi.velocity.z)) / spj.density * (45.0f / (Mathf.PI * Mathf.Pow(parameters[spi.parameterID].smoothingRadius, 6.0f))) * (parameters[spi.parameterID].smoothingRadius - r);
                    }
                }
            }

            // RVO Agents
            if (RVO_SPH)
            {
                for (int k = 0; k < RVOagents.Length; k++)
                {
                    if (RVOagents[k].go != null)
                    {
                        float xij = Mathf.Abs(RVOagents[k].go.transform.position.x - spi.position.x);
                        float zij = Mathf.Abs(RVOagents[k].go.transform.position.z - spi.position.z);
                        if (xij < parameters[spi.parameterID].smoothingRadiusSq && zij < parameters[spi.parameterID].smoothingRadiusSq)
                        {
                            //Vector3 rij = RVOagents[k].go.transform.position - particles[i].position;
                            Vector2 rij = new Vector2(RVOagents[k].go.transform.position.x - spi.position.x, RVOagents[k].go.transform.position.z - spi.position.z);
                            float r2 = rij.sqrMagnitude;
                            float r = Mathf.Sqrt(r2);

                            if (r < parameters[spi.parameterID].smoothingRadius)
                            {
                                forcePressure += -rij.normalized * parameters[spi.parameterID].particleMass * (spi.pressure + RVOagents[k].pressure) / (2.0f * RVOagents[k].density) * (-45.0f / (Mathf.PI * Mathf.Pow(parameters[spi.parameterID].smoothingRadius, 6.0f))) * Mathf.Pow(parameters[spi.parameterID].smoothingRadius - r, 2.0f);
                                //forceViscosity += parameters[particles[i].parameterID].particleViscosity * parameters[particles[i].parameterID].particleMass * (RVOagents[k].go.GetComponent<NavMeshAgent>().velocity - particles[i].velocity) / RVOagents[k].density * (45.0f / (Mathf.PI * Mathf.Pow(parameters[particles[i].parameterID].smoothingRadius, 6.0f))) * (parameters[particles[i].parameterID].smoothingRadius - r);
                                forceViscosity += parameters[spi.parameterID].particleViscosity * parameters[spi.parameterID].particleMass * (new Vector2(RVOagents[k].go.GetComponent<NavMeshAgent>().velocity.x - spi.velocity.x, RVOagents[k].go.GetComponent<NavMeshAgent>().velocity.z - spi.velocity.z)) / RVOagents[k].density * (45.0f / (Mathf.PI * Mathf.Pow(parameters[spi.parameterID].smoothingRadius, 6.0f))) * (parameters[spi.parameterID].smoothingRadius - r);
                            }
                        }
                    }
                }
            }

            // Ragdoll Agents
            if (SPH_RAGDOLL)
            {
                for (int l = 0; l < RagdollAgents.Length; l++)
                {
                    if (RagdollAgents[l].go != null)
                    {
                        float xij = Mathf.Abs(RagdollAgents[l].go.transform.position.x - spi.position.x);
                        float zij = Mathf.Abs(RagdollAgents[l].go.transform.position.z - spi.position.z);
                        if (xij < parameters[spi.parameterID].smoothingRadiusSq && zij < parameters[spi.parameterID].smoothingRadiusSq)
                        {
                            //Vector3 rij = RVOagents[k].go.transform.position - particles[i].position;
                            Vector2 rij = new Vector2(RagdollAgents[l].go.transform.position.x - spi.position.x, RagdollAgents[l].go.transform.position.z - spi.position.z);
                            float r2 = rij.sqrMagnitude;
                            float r = Mathf.Sqrt(r2);

                            if (r < parameters[spi.parameterID].smoothingRadius)
                            {
                                forcePressure += -rij.normalized * parameters[spi.parameterID].particleMass * (spi.pressure + RagdollAgents[l].pressure) / (2.0f * RagdollAgents[l].density) * (-45.0f / (Mathf.PI * Mathf.Pow(parameters[spi.parameterID].smoothingRadius, 6.0f))) * Mathf.Pow(parameters[spi.parameterID].smoothingRadius - r, 2.0f);
                                //forceViscosity += parameters[particles[i].parameterID].particleViscosity * parameters[particles[i].parameterID].particleMass * (RVOagents[k].go.GetComponent<NavMeshAgent>().velocity - particles[i].velocity) / RVOagents[k].density * (45.0f / (Mathf.PI * Mathf.Pow(parameters[particles[i].parameterID].smoothingRadius, 6.0f))) * (parameters[particles[i].parameterID].smoothingRadius - r);
                                forceViscosity += parameters[spi.parameterID].particleViscosity * parameters[spi.parameterID].particleMass * (new Vector2(RagdollAgents[l].go.GetComponent<NavMeshAgent>().velocity.x - spi.velocity.x, RagdollAgents[l].go.GetComponent<NavMeshAgent>().velocity.z - spi.velocity.z)) / RagdollAgents[l].density * (45.0f / (Mathf.PI * Mathf.Pow(parameters[spi.parameterID].smoothingRadius, 6.0f))) * (parameters[spi.parameterID].smoothingRadius - r);
                            }
                        }
                    }
                }
            }


            Vector3 forceGravity = GRAVITY * spi.density * parameters[spi.parameterID].gravityMult;
            //Vector3 goalNorm = (particles[i].goalPosition - particles[i].position).normalized;
            Vector3 goalNorm;
            Vector3 rotation;
            /*
            if (i % spawnPositions.Length <= 45)
            {
                Vector3 goalPos = new Vector3(UnityEngine.Random.Range(-4.5f, 4.5f), 0.0f, goalPos1.z);
                goalNorm = (goalPos - particles[i].position).normalized;
                rotation = new Vector3(0.0f, particles[i].forcePhysic.normalized.y, 0.0f);
            }
            else
            {
                Vector3 goalPos = new Vector3(UnityEngine.Random.Range(-4.5f, 4.5f), 0.0f, goalPos2.z);
                goalNorm = (goalPos - particles[i].position).normalized;
                rotation = new Vector3(0.0f, particles[i].forcePhysic.normalized.y + 180.0f, 0.0f);
            }
            */
            goalNorm = (spi.goalPosition - spi.position).normalized;
            rotation = new Vector3(0.0f, spi.forcePhysic.normalized.y + 180.0f, 0.0f);


            //float randPower = UnityEngine.Random.Range(0.5f, 1.5f);
            Vector3 forceGoal = goalNorm * goalPower;// * randPower;

            Vector3 Impulse = new Vector3(0.0f, 0.0f, -100000);

            // Apply
            if (addForce)
            {
                if (spi.position.z >= 10.0f && spi.position.z <= 12.0f)
                {
                    spi.forcePhysic = new Vector3(forcePressure.x + forceViscosity.x, 0.0f, forcePressure.y + forceViscosity.y) + forceGravity + forceGoal + Impulse;
                }
                else
                {
                    spi.forcePhysic = new Vector3(forcePressure.x + forceViscosity.x, 0.0f, forcePressure.y + forceViscosity.y) + forceGravity + forceGoal;
                }
            }
            else
            {
                spi.forcePhysic = new Vector3(forcePressure.x + forceViscosity.x, 0.0f, forcePressure.y + forceViscosity.y) + forceGravity + forceGoal;
            }
            //particles[i].go.transform.rotation = Quaternion.Euler(rotation);
        }
    }



    private void ApplyPosition()
    {
        for (int i = 0; i < particles.Length; i++)
        {
            particles[i].go.transform.position = particles[i].go.GetComponent<SPHProperties>().position + new Vector3(0.0f, -0.5f, 0.0f);
        }
    }

    private void CheckVelocityForAnimation()
    {
        for (int i = 0; i < particles.Length; i++)
        {
            if(particles[i].go.GetComponent<SPHProperties>().velocity.magnitude > 0.1f)
            {
                particles[i].go.GetComponent<Animator>().SetBool("isWalking", true);
            }
            else
            {
                particles[i].go.GetComponent<Animator>().SetBool("isWalking", false);
            }
        }
    }
}

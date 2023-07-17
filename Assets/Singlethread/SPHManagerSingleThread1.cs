using Microsoft.Win32;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SPHManagerSingleThread1 : MonoBehaviour
{
    private Vector3[] spawnPositions = new[]
    {
        new Vector3(-4.5f, 0.0f, -19.5f), new Vector3(-4.3f, 0.0f, -19.5f), new Vector3(-4.1f, 0.0f, -19.5f), new Vector3(-3.9f, 0.0f, -19.5f), new Vector3(-3.7f, 0.0f, -19.5f),
        new Vector3(-3.5f, 0.0f, -19.5f), new Vector3(-3.3f, 0.0f, -19.5f), new Vector3(-3.1f, 0.0f, -19.5f), new Vector3(-2.9f, 0.0f, -19.5f), new Vector3(-2.7f, 0.0f, -19.5f),
        new Vector3(-2.5f, 0.0f, -19.5f), new Vector3(-2.3f, 0.0f, -19.5f), new Vector3(-2.1f, 0.0f, -19.5f), new Vector3(-1.9f, 0.0f, -19.5f), new Vector3(-1.7f, 0.0f, -19.5f),
        new Vector3(-1.5f, 0.0f, -19.5f), new Vector3(-1.3f, 0.0f, -19.5f), new Vector3(-1.1f, 0.0f, -19.5f), new Vector3(-0.9f, 0.0f, -19.5f), new Vector3(-0.7f, 0.0f, -19.5f),
        new Vector3(-0.5f, 0.0f, -19.5f), new Vector3(-0.3f, 0.0f, -19.5f), new Vector3(-0.1f, 0.0f, -19.5f),
        new Vector3(4.5f, 0.0f, -19.5f), new Vector3(4.3f, 0.0f, -19.5f), new Vector3(4.1f, 0.0f, -19.5f), new Vector3(3.9f, 0.0f, -19.5f), new Vector3(3.7f, 0.0f, -19.5f),
        new Vector3(3.5f, 0.0f, -19.5f), new Vector3(3.3f, 0.0f, -19.5f), new Vector3(3.1f, 0.0f, -19.5f), new Vector3(2.9f, 0.0f, -19.5f), new Vector3(2.7f, 0.0f, -19.5f),
        new Vector3(2.5f, 0.0f, -19.5f), new Vector3(2.3f, 0.0f, -19.5f), new Vector3(2.1f, 0.0f, -19.5f), new Vector3(1.9f, 0.0f, -19.5f), new Vector3(1.7f, 0.0f, -19.5f),
        new Vector3(1.5f, 0.0f, -19.5f), new Vector3(1.3f, 0.0f, -19.5f), new Vector3(1.1f, 0.0f, -19.5f), new Vector3(0.9f, 0.0f, -19.5f), new Vector3(0.7f, 0.0f, -19.5f),
        new Vector3(0.5f, 0.0f, -19.5f), new Vector3(0.3f, 0.0f, -19.5f), new Vector3(0.1f, 0.0f, -19.5f),
        new Vector3(-4.5f, 0.0f, 19.5f), new Vector3(-4.3f, 0.0f, 19.5f), new Vector3(-4.1f, 0.0f, 19.5f), new Vector3(-3.9f, 0.0f, 19.5f), new Vector3(-3.7f, 0.0f, 19.5f),
        new Vector3(-3.5f, 0.0f, 19.5f), new Vector3(-3.3f, 0.0f, 19.5f), new Vector3(-3.1f, 0.0f, 19.5f), new Vector3(-2.9f, 0.0f, 19.5f), new Vector3(-2.7f, 0.0f, 19.5f),
        new Vector3(-2.5f, 0.0f, 19.5f), new Vector3(-2.3f, 0.0f, 19.5f), new Vector3(-2.1f, 0.0f, 19.5f), new Vector3(-1.9f, 0.0f, 19.5f), new Vector3(-1.7f, 0.0f, 19.5f),
        new Vector3(-1.5f, 0.0f, 19.5f), new Vector3(-1.3f, 0.0f, 19.5f), new Vector3(-1.1f, 0.0f, 19.5f), new Vector3(-0.9f, 0.0f, 19.5f), new Vector3(-0.7f, 0.0f, 19.5f),
        new Vector3(-0.5f, 0.0f, 19.5f), new Vector3(-0.3f, 0.0f, 19.5f), new Vector3(-0.1f, 0.0f, 19.5f),
        new Vector3(4.5f, 0.0f, 19.5f), new Vector3(4.3f, 0.0f, 19.5f), new Vector3(4.1f, 0.0f, 19.5f), new Vector3(3.9f, 0.0f, 19.5f), new Vector3(3.7f, 0.0f, 19.5f),
        new Vector3(3.5f, 0.0f, 19.5f), new Vector3(3.3f, 0.0f, 19.5f), new Vector3(3.1f, 0.0f, 19.5f), new Vector3(2.9f, 0.0f, 19.5f), new Vector3(2.7f, 0.0f, 19.5f),
        new Vector3(2.5f, 0.0f, 19.5f), new Vector3(2.3f, 0.0f, 19.5f), new Vector3(2.1f, 0.0f, 19.5f), new Vector3(1.9f, 0.0f, 19.5f), new Vector3(1.7f, 0.0f, 19.5f),
        new Vector3(1.5f, 0.0f, 19.5f), new Vector3(1.3f, 0.0f, 19.5f), new Vector3(1.1f, 0.0f, 19.5f), new Vector3(0.9f, 0.0f, 19.5f), new Vector3(0.7f, 0.0f, 19.5f),
        new Vector3(0.5f, 0.0f, 19.5f), new Vector3(0.3f, 0.0f, 19.5f), new Vector3(0.1f, 0.0f, 19.5f),
    };


    private struct SPHParticle
    {
        public Vector3 position;

        public Vector3 velocity;
        public Vector3 forcePhysic;
        public Vector3 forceHeading;

        public float density;
        public float pressure;

        public int parameterID;

        public GameObject go;



        public void Init(Vector3 _position, int _parameterID, GameObject _go)
        {
            position = _position;
            parameterID = _parameterID;
            go = _go;

            velocity = Vector3.zero;
            forcePhysic = Vector3.zero;
            forceHeading = Vector3.zero;
            density = 0.0f;
            pressure = 0.0f;
            go.GetComponent<Animator>().enabled = true;
            go.GetComponent<Animator>().SetBool("isWalking", false);
        }
    }



    [System.Serializable]
    private struct SPHParameters
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
    [SerializeField] private SPHParameters[] parameters = null;

    [Header("Properties")]
    [SerializeField] private int amount = 250;
    [SerializeField] private int rowSize = 16;

    // Data
    private SPHParticle[] particles;

    private bool addForce = false;

    private void Start()
    {
        InitSPH();
    }



    private void Update()
    {
        ComputeDensityPressure();
        ComputeForces();
        Integrate();
        ComputeColliders();

        ApplyPosition();

        CheckVelocityForAnimation();

        if (Input.GetKeyDown(KeyCode.Space))
        {
            addForce = true;
        }
        else
        {
            addForce = false;
        }
    }



    private void InitSPH()
    {
        particles = new SPHParticle[amount];
        
        for (int i = 0; i < amount; i++)
        {
            float jitter = (Random.value * 2f - 0f) * parameters[parameterID].particleRadius * 0.1f;
            float x = (i % rowSize) + Random.Range(-0.1f, 0.1f);
            float y = 0.0f;
            float z = ((i / rowSize) % rowSize) + Random.Range(-0.1f, 0.1f);

            GameObject go = Instantiate(character0Prefab);
            go.transform.parent = GameObject.Find("Agents").transform;
            //go.transform.localScale = Vector3.one * parameters[parameterID].particleRadius;
            if (i % spawnPositions.Length <= 45)
            {
                go.transform.position = new Vector3(spawnPositions[i % spawnPositions.Length].x, spawnPositions[i % spawnPositions.Length].y, spawnPositions[i % spawnPositions.Length].z + Random.Range(0.0f, 20.0f));
                go.transform.GetChild(3).GetComponent<SkinnedMeshRenderer>().material.color = Color.blue;
            }
            else
            {
                go.transform.position = new Vector3(spawnPositions[i % spawnPositions.Length].x, spawnPositions[i % spawnPositions.Length].y, spawnPositions[i % spawnPositions.Length].z + Random.Range(-20.0f, 0.0f));
                go.transform.GetChild(3).GetComponent<SkinnedMeshRenderer>().material.color = Color.blue;
            }
            go.name = "char" + i.ToString();

            particles[i].Init(go.transform.position, parameterID, go);
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
                if (Intersect(colliders[j], particles[i].position, parameters[particles[i].parameterID].particleRadius, out penetrationNormal, out penetrationPosition, out penetrationLength))
                {
                    particles[i].velocity = DampVelocity(colliders[j], particles[i].velocity, penetrationNormal, 1.0f - parameters[particles[i].parameterID].particleDrag);
                    particles[i].position = penetrationPosition - penetrationNormal * Mathf.Abs(penetrationLength);
                }
            }
        }
    }



    private void Integrate()
    {
        for (int i = 0; i < particles.Length; i++)
        {
            particles[i].velocity += DT * (particles[i].forcePhysic) / particles[i].density;
            particles[i].position += DT * (particles[i].velocity );
        }
    }


    private void ComputeDensityPressure()
    {
        for (int i = 0; i < particles.Length; i++)
        {
            particles[i].density = 0.0f;

            // Circle
            /*
            for (int j = 0; j < particles.Length; j++)
            {
                Vector3 rij = particles[j].position - particles[i].position;
                float r2 = rij.sqrMagnitude;

                if (r2 < parameters[particles[i].parameterID].smoothingRadiusSq)
                {
                    particles[i].density += parameters[particles[i].parameterID].particleMass * (315.0f / (64.0f * Mathf.PI * Mathf.Pow(parameters[particles[i].parameterID].smoothingRadius, 9.0f))) * Mathf.Pow(parameters[particles[i].parameterID].smoothingRadiusSq - r2, 3.0f);
                }
            }
            */

            // AABB
            
            for (int j = 0; j < particles.Length; j++)
            {
                float xij = Mathf.Abs(particles[j].position.x - particles[i].position.x);
                float yij = Mathf.Abs(particles[j].position.x - particles[i].position.x);
                if (xij < parameters[particles[i].parameterID].smoothingRadiusSq && yij < parameters[particles[i].parameterID].smoothingRadiusSq)
                {
                    Vector3 rij = particles[j].position - particles[i].position;
                    float r2 = rij.sqrMagnitude;

                    if (r2 < parameters[particles[i].parameterID].smoothingRadiusSq)
                    {
                        particles[i].density += parameters[particles[i].parameterID].particleMass * (315.0f / (64.0f * Mathf.PI * Mathf.Pow(parameters[particles[i].parameterID].smoothingRadius, 9.0f))) * Mathf.Pow(parameters[particles[i].parameterID].smoothingRadiusSq - r2, 3.0f);
                    }
                }
            }
            

            particles[i].pressure = GAS_CONST * (particles[i].density - parameters[particles[i].parameterID].restDensity);
        }
    }



    private void ComputeForces()
    {
        for (int i = 0; i < particles.Length; i++)
        {
            Vector3 forcePressure = Vector3.zero;
            Vector3 forceViscosity = Vector3.zero;

            // Physics
            // Circle
            /*
            for (int j = 0; j < particles.Length; j++)
            {
                if (i == j) continue;

                Vector3 rij = particles[j].position - particles[i].position;
                float r2 = rij.sqrMagnitude;
                float r = Mathf.Sqrt(r2);

                if (r < parameters[particles[i].parameterID].smoothingRadius)
                {
                    forcePressure += -rij.normalized * parameters[particles[i].parameterID].particleMass * (particles[i].pressure + particles[j].pressure) / (2.0f * particles[j].density) * (-45.0f / (Mathf.PI * Mathf.Pow(parameters[particles[i].parameterID].smoothingRadius, 6.0f))) * Mathf.Pow(parameters[particles[i].parameterID].smoothingRadius - r, 2.0f);

                    forceViscosity += parameters[particles[i].parameterID].particleViscosity * parameters[particles[i].parameterID].particleMass * (particles[j].velocity - particles[i].velocity) / particles[j].density * (45.0f / (Mathf.PI * Mathf.Pow(parameters[particles[i].parameterID].smoothingRadius, 6.0f))) * (parameters[particles[i].parameterID].smoothingRadius - r);
                }
            }
            */

            // AABB
            
            for (int j = 0; j < particles.Length; j++)
            {
                if (i == j) continue;

                float xij = Mathf.Abs(particles[j].position.x - particles[i].position.x);
                float yij = Mathf.Abs(particles[j].position.x - particles[i].position.x);
                if (xij < parameters[particles[i].parameterID].smoothingRadiusSq && yij < parameters[particles[i].parameterID].smoothingRadiusSq)
                {
                    Vector3 rij = particles[j].position - particles[i].position;
                    float r2 = rij.sqrMagnitude;
                    float r = Mathf.Sqrt(r2);

                    if (r < parameters[particles[i].parameterID].smoothingRadius)
                    {
                        forcePressure += -rij.normalized * parameters[particles[i].parameterID].particleMass * (particles[i].pressure + particles[j].pressure) / (2.0f * particles[j].density) * (-45.0f / (Mathf.PI * Mathf.Pow(parameters[particles[i].parameterID].smoothingRadius, 6.0f))) * Mathf.Pow(parameters[particles[i].parameterID].smoothingRadius - r, 2.0f);

                        forceViscosity += parameters[particles[i].parameterID].particleViscosity * parameters[particles[i].parameterID].particleMass * (particles[j].velocity - particles[i].velocity) / particles[j].density * (45.0f / (Mathf.PI * Mathf.Pow(parameters[particles[i].parameterID].smoothingRadius, 6.0f))) * (parameters[particles[i].parameterID].smoothingRadius - r);
                    }
                }
            }
            

            Vector3 forceGravity = GRAVITY * particles[i].density * parameters[particles[i].parameterID].gravityMult;
            //Vector3 goalNorm = (particles[i].goalPosition - particles[i].position).normalized;
            Vector3 goalNorm;
            Vector3 rotation;
            if (i % spawnPositions.Length <= 45)
            {
                Vector3 goalPos = new Vector3(Random.Range(-4.5f, 4.5f), 0.0f, goalPos1.z);
                goalNorm = (goalPos - particles[i].position).normalized;
                rotation = new Vector3(0.0f, particles[i].forcePhysic.normalized.y, 0.0f);
            }
            else
            {
                Vector3 goalPos = new Vector3(Random.Range(-4.5f, 4.5f), 0.0f, goalPos2.z);
                goalNorm = (goalPos - particles[i].position).normalized;
                rotation = new Vector3(0.0f, particles[i].forcePhysic.normalized.y + 180.0f, 0.0f);
            }
            Vector3 forceGoal = goalNorm * goalPower;

            Vector3 Impulse = new Vector3(0.0f, 0.0f, -200000);

            // Apply
            if (addForce)
            {
                if (particles[i].position.z >= 10.0f && particles[i].position.z <= 12.0f)
                {
                    //particles[i].go.transform.GetChild(3).GetComponent<SkinnedMeshRenderer>().material.color = Color.red;
                    particles[i].forcePhysic = forcePressure + forceViscosity + forceGravity + forceGoal + Impulse;
                }
                else
                {
                    particles[i].forcePhysic = forcePressure + forceViscosity + forceGravity + forceGoal;
                }
            }
            else
            {
                particles[i].forcePhysic = forcePressure + forceViscosity + forceGravity + forceGoal;
                //particles[i].forcePhysic = forceGoal;
            }
            //particles[i].forcePhysic = forcePressure + forceViscosity + forceGravity;
            particles[i].go.transform.rotation = Quaternion.Euler(rotation);
        }
    }



    private void ApplyPosition()
    {
        for (int i = 0; i < particles.Length; i++)
        {
            particles[i].go.transform.position = particles[i].position;
        }
    }

    private void CheckVelocityForAnimation()
    {
        for (int i = 0; i < particles.Length; i++)
        {
            if(particles[i].velocity.magnitude > 8f)
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

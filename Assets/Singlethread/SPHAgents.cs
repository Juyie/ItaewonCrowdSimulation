using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.ParticleSystem;

public class SPHAgents : MonoBehaviour
{
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

    public Vector3 position;

    public Vector3 velocity;
    public Vector3 forcePhysic;
    public Vector3 forceHeading;

    public float density;
    public float pressure;

    public Vector3 goalPos;

    public void Init(Vector3 _position, Vector3 _goalPosition)
    {
        position = _position;
        goalPos = _goalPosition;

        velocity = Vector3.zero;
        forcePhysic = Vector3.zero;
        forceHeading = Vector3.zero;
        density = 0.0f;
        pressure = 0.0f;
        GetComponent<Animator>().enabled = true;
        GetComponent<Animator>().SetBool("isWalking", false);
    }

    [Header("Parameters")]
    [SerializeField] private int parameterID = 0;
    [SerializeField] private SPHParameters[] parameters = null;

    private List<GameObject> agents = new List<GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        //agents = new List<GameObject>();
    }

    // Update is called once per frame
    void Update()
    {
        ComputeDensityPressure();
        ComputeForces();
        Integrate();
        ComputeColliders();

        ApplyPosition();

        CheckVelocityForAnimation();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Agent") && !agents.Contains(other.gameObject))
        {
            agents.Add(other.gameObject);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Agent") && agents.Contains(other.gameObject))
        {
            agents.Remove(other.gameObject);
        }
    }

    /*
    void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Agent") && !agents.Contains(other.gameObject))
        {
            agents.Add(other.gameObject);
        }
    }
    */

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

        for (int j = 0; j < colliders.Length; j++)
        {
            // Check collision
            Vector3 penetrationNormal;
            Vector3 penetrationPosition;
            float penetrationLength;
            if (Intersect(colliders[j], position, parameters[parameterID].particleRadius, out penetrationNormal, out penetrationPosition, out penetrationLength))
            {
                velocity = DampVelocity(colliders[j], velocity, penetrationNormal, 1.0f - parameters[parameterID].particleDrag);
                position = penetrationPosition - penetrationNormal * Mathf.Abs(penetrationLength);
            }
        }
    }
    private void Integrate()
    {
        if (density != 0)
        {
            velocity += DT * forcePhysic / density;
            position += DT * velocity;
            //Debug.Log(forcePhysic);
        }
    }

    private void ComputeDensityPressure()
    {
        if (agents != null)
        {
            density = 0.0f;

            for (int i = 0; i < agents.Count; i++)
            {
                Vector3 ri = agents[i].transform.position - transform.position;
                float r2 = ri.sqrMagnitude;

                if (r2 < parameters[parameterID].smoothingRadiusSq)
                {
                    density += parameters[parameterID].particleMass * (315.0f / (64.0f * Mathf.PI * Mathf.Pow(parameters[parameterID].smoothingRadius, 9.0f))) * Mathf.Pow(parameters[parameterID].smoothingRadiusSq - r2, 3.0f);
                }
            }

            pressure = GAS_CONST * (density - parameters[parameterID].restDensity);
        }
    }

    private void ComputeForces()
    {
        if (agents != null)
        {
            Vector3 forcePressure = Vector3.zero;
            Vector3 forceViscosity = Vector3.zero;

            // Physics
            for (int i = 0; i < agents.Count; i++)
            {
                Vector3 ri = agents[i].transform.position - transform.position;
                //Debug.Log("ri: " + ri);
                float r2 = ri.sqrMagnitude;
                float r = Mathf.Sqrt(r2);

                if (r < parameters[parameterID].smoothingRadius && agents[i].GetComponent<SPHAgents>().density != 0)
                {
                    forcePressure += -ri.normalized * parameters[parameterID].particleMass * (pressure + agents[i].GetComponent<SPHAgents>().pressure) / (2.0f * agents[i].GetComponent<SPHAgents>().density) * (-45.0f / (Mathf.PI * Mathf.Pow(parameters[parameterID].smoothingRadius, 6.0f))) * Mathf.Pow(parameters[parameterID].smoothingRadius - r, 2.0f);
                    forceViscosity += parameters[parameterID].particleViscosity * parameters[parameterID].particleMass * (agents[i].GetComponent<SPHAgents>().velocity - velocity) / agents[i].GetComponent<SPHAgents>().density * (45.0f / (Mathf.PI * Mathf.Pow(parameters[parameterID].smoothingRadius, 6.0f))) * (parameters[parameterID].smoothingRadius - r);
                }
            }

            Vector3 forceGravity = GRAVITY * density * parameters[parameterID].gravityMult;

            // forceGoal
            Vector3 goalNorm;
            Vector3 rotation;
            goalPos = new Vector3(Random.Range(-4.5f, 4.5f), 0.0f, goalPos.z);
            goalNorm = (goalPos - transform.position).normalized;
            rotation = new Vector3(0.0f, forcePhysic.normalized.y, 0.0f);
            Vector3 forceGoal = goalNorm * goalPower;
            //Debug.Log("Pressure: " + forcePressure + ", Viscosity: " + forceViscosity);

            // Apply
            forcePhysic = forcePressure + forceViscosity + forceGravity + forceGoal;
            //Debug.Log("force physic: " + forcePhysic);
        }
        else
        {
            Vector3 goalNorm;
            Vector3 rotation;
            goalNorm = (goalPos - gameObject.transform.position).normalized;
            rotation = new Vector3(0.0f, forcePhysic.normalized.y, 0.0f);
            Vector3 forceGoal = goalNorm * goalPower;
            forcePhysic = forceGoal;
        }
    }

    private void ApplyPosition()
    {
        gameObject.transform.position = position;
    }

    private void CheckVelocityForAnimation()
    {
        if (velocity.magnitude > 5f)
        {
            GetComponent<Animator>().SetBool("isWalking", true);
        }
        else
        {
            GetComponent<Animator>().SetBool("isWalking", false);
        }
    }
}

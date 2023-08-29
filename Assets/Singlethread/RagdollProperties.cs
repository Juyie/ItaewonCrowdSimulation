using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RagdollProperties : MonoBehaviour
{
    public SPHManagerSingleThread SPHManager;

    private SPHManagerSingleThread.SPHParticle[] particles;
    private RagdollSpawner.RagdollAgent[] Ragdollagents;

    private const float GAS_CONST = 2000.0f;

    // Start is called before the first frame update
    void Start()
    {
        particles = SPHManager.particles;
        Ragdollagents = GameObject.Find("RagdollManager").GetComponent<RagdollSpawner>().RagdollAgents;
    }

    // Update is called once per frame
    void Update()
    {
        particles = SPHManager.particles;
        Ragdollagents = GameObject.Find("RagdollManager").GetComponent<RagdollSpawner>().RagdollAgents;
        ComputeDensityPressure();
    }

    private void ComputeDensityPressure()
    {
        for (int i = 0; i < Ragdollagents.Length; i++)
        {
            Ragdollagents[i].density = 0.0f;

            // AABB
            // SPH Agents
            for (int j = 0; j < particles.Length; j++)
            {
                float xij = Mathf.Abs(particles[j].position.x - Ragdollagents[i].go.transform.position.x);
                float zij = Mathf.Abs(particles[j].position.z - Ragdollagents[i].go.transform.position.z);
                if (xij < SPHManager.parameters[0].smoothingRadiusSq && zij < SPHManager.parameters[0].smoothingRadiusSq)
                {
                    Vector3 rij = particles[j].position - Ragdollagents[i].go.transform.position;
                    float r2 = rij.sqrMagnitude;

                    if (r2 < SPHManager.parameters[0].smoothingRadiusSq)
                    {
                        Ragdollagents[i].density += SPHManager.parameters[0].particleMass * (315.0f / (64.0f * Mathf.PI * Mathf.Pow(SPHManager.parameters[0].smoothingRadius, 9.0f))) * Mathf.Pow(SPHManager.parameters[0].smoothingRadiusSq - r2, 3.0f);
                    }
                }
            }

            // Ragdoll Agents
            for (int k = 0; k < Ragdollagents.Length; k++)
            {
                float xij = Mathf.Abs(Ragdollagents[k].go.transform.position.x - Ragdollagents[i].go.transform.position.x);
                float zij = Mathf.Abs(Ragdollagents[k].go.transform.position.z - Ragdollagents[i].go.transform.position.z);
                if (xij < SPHManager.parameters[0].smoothingRadiusSq && zij < SPHManager.parameters[0].smoothingRadiusSq)
                {
                    Vector3 rij = Ragdollagents[k].go.transform.position - Ragdollagents[i].go.transform.position;
                    float r2 = rij.sqrMagnitude;

                    if (r2 < SPHManager.parameters[0].smoothingRadiusSq)
                    {
                        Ragdollagents[i].density += SPHManager.parameters[0].particleMass * (315.0f / (64.0f * Mathf.PI * Mathf.Pow(SPHManager.parameters[0].smoothingRadius, 9.0f))) * Mathf.Pow(SPHManager.parameters[0].smoothingRadiusSq - r2, 3.0f);
                    }
                }
            }

            Ragdollagents[i].pressure = GAS_CONST * (Ragdollagents[i].density - SPHManager.parameters[0].restDensity);
        }
    }
}

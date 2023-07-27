using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.ParticleSystem;

public class RVOAgentProperty : MonoBehaviour
{
    public SPHManagerSingleThread SPHManager;

    private SPHManagerSingleThread.SPHParticle[] particles;
    private NavagentSpawner.RVOAgent[] RVOagents;

    private const float GAS_CONST = 2000.0f;

    // Start is called before the first frame update
    void Start()
    {
        particles = SPHManager.particles;
        RVOagents = GameObject.Find("RVOManager").GetComponent<NavagentSpawner>().RVOAgents;
    }

    // Update is called once per frame
    void Update()
    {
        particles = SPHManager.particles;
        RVOagents = GameObject.Find("RVOManager").GetComponent<NavagentSpawner>().RVOAgents;
        ComputeDensityPressure();
    }

    private void ComputeDensityPressure()
    {
        for (int i = 0; i < RVOagents.Length; i++)
        {
            RVOagents[i].density = 0.0f;

            // AABB
            // SPH Agents
            for (int j = 0; j < particles.Length; j++)
            {
                float xij = Mathf.Abs(particles[j].position.x - RVOagents[i].go.transform.position.x);
                float zij = Mathf.Abs(particles[j].position.z - RVOagents[i].go.transform.position.z);
                if (xij < SPHManager.parameters[0].smoothingRadiusSq && zij < SPHManager.parameters[0].smoothingRadiusSq)
                {
                    Vector3 rij = particles[j].position - RVOagents[i].go.transform.position;
                    float r2 = rij.sqrMagnitude;

                    if (r2 < SPHManager.parameters[0].smoothingRadiusSq)
                    {
                        RVOagents[i].density += SPHManager.parameters[0].particleMass * (315.0f / (64.0f * Mathf.PI * Mathf.Pow(SPHManager.parameters[0].smoothingRadius, 9.0f))) * Mathf.Pow(SPHManager.parameters[0].smoothingRadiusSq - r2, 3.0f);
                    }
                }
            }

            // RVO Agents
            for (int k = 0; k < RVOagents.Length; k++)
            {
                float xij = Mathf.Abs(RVOagents[k].go.transform.position.x - RVOagents[i].go.transform.position.x);
                float zij = Mathf.Abs(RVOagents[k].go.transform.position.z - RVOagents[i].go.transform.position.z);
                if (xij < SPHManager.parameters[0].smoothingRadiusSq && zij < SPHManager.parameters[0].smoothingRadiusSq)
                {
                    Vector3 rij = RVOagents[k].go.transform.position - RVOagents[i].go.transform.position;
                    float r2 = rij.sqrMagnitude;

                    if (r2 < SPHManager.parameters[0].smoothingRadiusSq)
                    {
                        RVOagents[i].density += SPHManager.parameters[0].particleMass * (315.0f / (64.0f * Mathf.PI * Mathf.Pow(SPHManager.parameters[0].smoothingRadius, 9.0f))) * Mathf.Pow(SPHManager.parameters[0].smoothingRadiusSq - r2, 3.0f);
                    }
                }
            }

            RVOagents[i].pressure = GAS_CONST * (RVOagents[i].density - SPHManager.parameters[0].restDensity);
        }
    }
}

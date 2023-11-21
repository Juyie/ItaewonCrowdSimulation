using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.ParticleSystem;

public class RVOAgentProperty : MonoBehaviour
{
    public SPHManagerSingleThread SPHManager;

    private SPHManagerSingleThread.SPHParticle[] particles;
    private NavagentSpawner.RVOAgent[] RVOagents;

    private const float GAS_CONST = 100.0f;

    // Start is called before the first frame update
    void Start()
    {
        particles = SPHManager.particles;
        RVOagents = NavagentSpawner.Instance.RVOAgents;
    }

    // Update is called once per frame
    void Update()
    {
        if (SPHManagerSingleThread.Instance.RVO_SPH)
        {
            particles = SPHManager.particles;
            RVOagents = NavagentSpawner.Instance.RVOAgents;
            ComputeDensityPressure();
        }
    }

    private void ComputeDensityPressure()
    {
        for (int i = 0; i < RVOagents.Length; i++)
        {
            if (RVOagents[i].go != null)
            {
                RVOagents[i].density = 0.0f;

                // AABB
                // SPH Agents
                for (int j = 0; j < particles.Length; j++)
                {
                    if (particles[j].go != null)
                    {
                        Vector2 rij = new Vector2(particles[j].position.x - RVOagents[i].go.transform.position.x, particles[j].position.z - RVOagents[i].go.transform.position.z);
                        float distanceSquared = rij.sqrMagnitude;
                        float diff = SPHManager.parameters[0].smoothingRadiusSq - distanceSquared;

                        if (diff > 0)
                        {
                            RVOagents[i].density += SPHManager.parameters[0].particleMass * (4.0f / (Mathf.PI * Mathf.Pow(SPHManager.parameters[0].smoothingRadius, 8.0f))) * Mathf.Pow(diff, 3.0f);
                        }
                    }
                }

                // RVO Agents
                for (int k = 0; k < RVOagents.Length; k++)
                {
                    if (RVOagents[k].go != null)
                    {
                        Vector2 rij = new Vector2(RVOagents[k].go.transform.position.x - RVOagents[i].go.transform.position.x, RVOagents[k].go.transform.position.z - RVOagents[i].go.transform.position.z);
                        float distanceSquared = rij.sqrMagnitude;
                        float diff = SPHManager.parameters[0].smoothingRadiusSq - distanceSquared;

                        if (diff > 0)
                        {
                            RVOagents[i].density += SPHManager.parameters[0].particleMass * (4.0f / (Mathf.PI * Mathf.Pow(SPHManager.parameters[0].smoothingRadius, 8.0f))) * Mathf.Pow(diff, 3.0f);
                        }
                    }
                }

                // add the agent itself
                RVOagents[i].density += SPHManager.parameters[0].particleMass * (4.0f / (Mathf.PI * Mathf.Pow(SPHManager.parameters[0].smoothingRadius, 8.0f))) * Mathf.Pow(SPHManager.parameters[0].smoothingRadiusSq, 3.0f);

                RVOagents[i].pressure = GAS_CONST * (RVOagents[i].density - SPHManager.parameters[0].restDensity);
            }
        }
    }
}

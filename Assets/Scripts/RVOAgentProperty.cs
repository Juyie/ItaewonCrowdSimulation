using DataStructures.ViliWonka.KDTree;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.ParticleSystem;

public class RVOAgentProperty : MonoBehaviour
{
    public SPHManagerSingleThread SPHManager;

    private SPHManagerSingleThread.SPHParticle[] particles;
    private GameObject[] RVOGameObject;
    private Vector3[] RVOPointCloud;
    private KDTree RVOKDTree;
    private int[] TypeOfSimulation;

    private KDQuery query;

    private const float GAS_CONST = 100.0f;

    // Start is called before the first frame update
    void Start()
    {
        RVOGameObject = NavagentSpawner.Instance.RVOGameObject;
        RVOPointCloud = NavagentSpawner.Instance.RVOPointCloud;
        RVOKDTree = NavagentSpawner.Instance.RVOKDTree;
        TypeOfSimulation = NavagentSpawner.Instance.TypeOfSimulation;
        query = new KDQuery();
    }

    // Update is called once per frame
    void Update()
    {
        /*
        if (SPHManagerSingleThread.Instance.RVO_SPH)
        {
            particles = SPHManager.particles;
            RVOGameObject = NavagentSpawner.Instance.RVOGameObject;
            RVOPointCloud = NavagentSpawner.Instance.RVOPointCloud;
            RVOKDTree = NavagentSpawner.Instance.RVOKDTree;
            TypeOfSimulation = NavagentSpawner.Instance.TypeOfSimulation;
            ComputeDensityPressure();
        }
        */
    }

    private void ComputeDensityPressure()
    {
        for (int i = 0; i < RVOGameObject.Length; i++)
        {
            if (TypeOfSimulation[i] == 0)
            {
                SPHProperties sp = RVOGameObject[i].GetComponent<SPHProperties>();
                sp.density = 0.0f;

                List<int> results = new List<int>();
                // RVO Agents
                query.Radius(RVOKDTree, sp.position, SPHManager.parameters[0].smoothingRadiusSq, results);
                for (int k = 0; k < results.Count; k++)
                {
                    Vector2 rij = new Vector2(RVOPointCloud[results[k]].x - sp.position.x, RVOPointCloud[results[k]].z - sp.position.z);
                    float distanceSquared = rij.sqrMagnitude;
                    float diff = SPHManager.parameters[0].smoothingRadiusSq - distanceSquared;

                    sp.density += SPHManager.parameters[0].particleMass * (4.0f / (Mathf.PI * Mathf.Pow(SPHManager.parameters[0].smoothingRadius, 8.0f))) * Mathf.Pow(diff, 3.0f);
                }

                // add the agent itself
                sp.density += SPHManager.parameters[0].particleMass * (4.0f / (Mathf.PI * Mathf.Pow(SPHManager.parameters[0].smoothingRadius, 8.0f))) * Mathf.Pow(SPHManager.parameters[0].smoothingRadiusSq, 3.0f);

                sp.pressure = GAS_CONST * (sp.density - SPHManager.parameters[0].restDensity);
                // Debug.Log("RVO KDTree/ Density: " + sp.density + ", Pressure: " + sp.pressure);
            }
        }
    }
}

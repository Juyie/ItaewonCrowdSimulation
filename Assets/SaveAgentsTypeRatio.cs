using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class AgentRatio
{
    public int[] RVOs;
    public int[] SPHs;
    public int[] SPHZs;
    public int[] Ragdolls;
    public int[] RagdollM;
    public int[] RagdollW;
}

public class Force
{
    public float[] forces;
}

public class SaveAgentsTypeRatio : MonoBehaviour
{
    [SerializeField]
    private bool save;

    [SerializeField]
    private DisplayAgentNumber agentNumber;

    private bool ready = true;
    private int count = 0;
    private AgentRatio agentRatio = null;
    private bool once = true;
    private Force force = null;
    private bool forceOnce = true;

    // Start is called before the first frame update
    void Start()
    {
        agentRatio = new AgentRatio();
        agentRatio.RVOs = new int[101];
        agentRatio.SPHs = new int[101];
        agentRatio.SPHZs = new int[101];
        agentRatio.Ragdolls = new int[101];
        agentRatio.RagdollM = new int[101];
        agentRatio.RagdollW = new int[101];

        force = new Force();
        force.forces = new float[6000];
    }

    // Update is called once per frame
    void Update()
    {
        if(save && ready)
        {
            StartCoroutine(JsonAppend());
        }

        if(save && count == 40 && once)
        {
            JsonSave();
            once = false;
        }

        if(agentNumber.agentNumber == 6000 && forceOnce)
        {
            JsonSaveForce();
            forceOnce = false;
        }
    }

    private void JsonSaveForce()
    {
        for (int i = 0; i < NavagentSpawner.Instance.RVOGameObject.Length; i++)
        {
            force.forces[i] = NavagentSpawner.Instance.RVOGameObject[i].transform.GetChild(0).GetChild(0).GetComponent<CalculateForce>().maxForce;
        }

        string path = "C:\\Users\\juyie\\Desktop\\SimulationData\\";

        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }

        string saveJson = JsonUtility.ToJson(force);

        File.WriteAllText(path + "maxForce" + DateTime.Now.ToString("yyyy-MM-dd HH-mm-ss") + ".json", saveJson);
    }

    IEnumerator JsonAppend()
    {
        ready = false;

        int rvoC = 0;
        int sphC = 0;
        int sphzC = 0;
        int ragC = 0;
        int ragmC = 0;
        int ragwC = 0;

        for (int i = 0; i < NavagentSpawner.Instance.RVOGameObject.Length; i++)
        {
            switch (NavagentSpawner.Instance.TypeOfSimulation[i])
            {
                case 0:
                    rvoC++;
                    break;
                case 1:
                    if (NavagentSpawner.Instance.RVOGameObject[i].GetComponent<SPHProperties>().goalForce == 0)
                    {
                        sphzC++;
                    }
                    else
                    {
                        sphC++;
                    }
                    break;
                case 2: 
                    ragC++;
                    if (NavagentSpawner.Instance.RVOGameObject[i].name.StartsWith("w"))
                    {
                        ragwC++;
                    }
                    else
                    {
                        ragmC++;
                    }
                    break;
            }
        }

        agentRatio.RVOs[count] = rvoC;
        agentRatio.SPHs[count] = sphC;
        agentRatio.SPHZs[count] = sphzC;
        agentRatio.Ragdolls[count] = ragC;
        agentRatio.RagdollM[count] = ragmC;
        agentRatio.RagdollW[count] = ragwC;

        yield return new WaitForSeconds(5.0f);
        count++;
        ready = true;
    }

    private void JsonSave()
    {
        string path = "C:\\Users\\juyie\\Desktop\\SimulationData\\";

        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }

        string saveJson = JsonUtility.ToJson(agentRatio);

        File.WriteAllText(path + "agentRatio" + DateTime.Now.ToString("yyyy-MM-dd HH-mm-ss") + ".json", saveJson);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.AI;

public class AgentData
{
    public Vector3[] positions;
    public Vector3[] rotations;
    public string[] targetPosNames;
}

public class SaveAgentsData : MonoBehaviour
{
    [SerializeField]
    private bool save;

    [SerializeField]
    GameObject agentPf;

    [SerializeField]
    private DisplayAgentNumber displayAgentNumber;

    private static SaveAgentsData instance;

    private void Awake()
    {
        instance = this;
        JsonLoad();
    }

    public static SaveAgentsData Instance
    {
        get { return instance; }
    }

    private void Update()
    {
        //Debug.Log(GameObject.Find("RVOAgents").transform.GetChild(0).position);
    }

    public void JsonSave()
    {
        if (save)
        {
            GameObject RVOAgents = GameObject.Find("RVOAgents");
            int agentsNum = RVOAgents.transform.childCount;

            AgentData data = new AgentData();

            data.positions = new Vector3[agentsNum];
            data.rotations = new Vector3[agentsNum];
            data.targetPosNames = new string[agentsNum];

            for (int i = 0; i < agentsNum; i++)
            {
                data.positions[i] = RVOAgents.transform.GetChild(i).gameObject.transform.position;
                data.rotations[i] = RVOAgents.transform.GetChild(i).gameObject.transform.rotation.eulerAngles;
                data.targetPosNames[i] = RVOAgents.transform.GetChild(i).GetComponent<PlayerMovement>().target.name;
            }

            string path = "C:\\Users\\juyie\\Desktop\\SimulationData\\";

            /*
            if(!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            */

            string saveJson = JsonUtility.ToJson(data);

            File.WriteAllText(path + "agentdata4000.json", saveJson);
            Debug.Log("data save");
        }
    }

    public void JsonLoad()
    {
        string path = "C:\\Users\\juyie\\Desktop\\SimulationData\\agentdata1800.json";

            string saveFile = File.ReadAllText(path);
            AgentData data = JsonUtility.FromJson<AgentData>(saveFile);

        for (int i = 0; i < data.positions.Length; i++)
        {
            GameObject newAgent = Instantiate(agentPf);
            newAgent.name += i;
            newAgent.transform.position = data.positions[i];
            newAgent.transform.rotation = Quaternion.Euler(data.rotations[i]);
            newAgent.GetComponent<PlayerMovement>().target = GameObject.Find(data.targetPosNames[i]).transform;
            newAgent.transform.parent = GameObject.Find("RVOAgents").transform;
            newAgent.GetComponent<NavMeshAgent>().enabled = true;
            newAgent.GetComponent<PlayerMovement>().enabled = true;
            SPHProperties sp = newAgent.GetComponent<SPHProperties>();
            sp.position = newAgent.transform.position;
            newAgent.SetActive(true);
            displayAgentNumber.agentNumber++;
        }
    }
}

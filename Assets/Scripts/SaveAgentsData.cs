using System.Collections;
using System.Collections.Generic;
using DataStructures.ViliWonka.KDTree;
using UnityEngine;
using System.IO;
using UnityEngine.AI;
using UnityEngine.Recorder;
using UnityEditor;
using Unity.Transforms;

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
    private bool load;

    [SerializeField]
    GameObject MAgentPf;

    [SerializeField]
    GameObject WMAgentPf;

    [SerializeField]
    private DisplayAgentNumber displayAgentNumber;

    private static SaveAgentsData instance;

    private void Start()
    {
        instance = this;
        if (load)
        {
            JsonLoad();
        }
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

            File.WriteAllText(path + "agentdata4500_v3.json", saveJson);
            //RecorderWindow recorderWindow = GetRecorderWindow();
            //recorderWindow.StopRecording();
            Debug.Log("data save");
        }
    }

    public void JsonLoad()
    {
        string path = "C:\\Users\\juyie\\Desktop\\SimulationData\\agentdata4500_v3.json";

        string saveFile = File.ReadAllText(path);
        AgentData data = JsonUtility.FromJson<AgentData>(saveFile);

        Debug.Log(data.positions.Length);
        int dataLength = 4500;
        //int dataLength = 2400;

        for (int i = dataLength * 4 / 5; i < dataLength; i++)
        //for(int i = 0; i < dataLength * 1 / 6; i++)
        {
            GameObject newAgent;
            int prefabNum = Random.Range(0, 2);
            if (prefabNum == 0)
            {
                newAgent = Instantiate(MAgentPf);
            }
            else
            {
                newAgent = Instantiate(WMAgentPf);
            }
            newAgent.name += i;
            newAgent.transform.position = data.positions[i];
            newAgent.transform.rotation = Quaternion.Euler(data.rotations[i]);
            newAgent.GetComponent<PlayerMovement>().target = GameObject.Find(data.targetPosNames[i]).transform;
            newAgent.transform.parent = GameObject.Find("RVOAgents").transform;
            newAgent.GetComponent<NavMeshAgent>().enabled = true;
            newAgent.GetComponent<PlayerMovement>().enabled = true;
            SPHProperties sp = newAgent.GetComponent<SPHProperties>();
            sp.position = newAgent.transform.position;
            if (newAgent.name.StartsWith("w"))
            {
                newAgent.transform.GetChild(4).GetComponent<SkinnedMeshRenderer>().material.color = new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f));
            }
            else
            {
                newAgent.transform.GetChild(3).GetComponent<SkinnedMeshRenderer>().material.color = new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f));
            }
            newAgent.SetActive(true);

            NavagentSpawner.Instance.RVOGameObject[i] = newAgent;
            NavagentSpawner.Instance.RVOPointCloud[i] = sp.position;
            NavagentSpawner.Instance.TypeOfSimulation[i] = 0;

            displayAgentNumber.agentNumber++;
        }
        
        // waited agent
        for (int i = 0; i < dataLength * 4 / 5; i++)
        //for(int i = 0; i < dataLength; i++)
        {
            GameObject newAgent;
            int prefabNum = Random.Range(0, 2);
            if (prefabNum == 0)
            {
                newAgent = Instantiate(MAgentPf);
            }
            else
            {
                newAgent = Instantiate(WMAgentPf);
            }
            newAgent.name += i;
            newAgent.transform.position = data.positions[i] + new Vector3(0.0f, 100.0f, 0.0f);
            newAgent.transform.rotation = Quaternion.Euler(data.rotations[i]);
            newAgent.GetComponent<PlayerMovement>().target = GameObject.Find(data.targetPosNames[i]).transform;
            newAgent.transform.parent = GameObject.Find("WaitAgents").transform;
            newAgent.GetComponent<NavMeshAgent>().enabled = false;
            newAgent.GetComponent<PlayerMovement>().enabled = false;
            SPHProperties sp = newAgent.GetComponent<SPHProperties>();
            sp.position = newAgent.transform.position;
            if (newAgent.name.StartsWith("w"))
            {
                newAgent.transform.GetChild(4).GetComponent<SkinnedMeshRenderer>().material.color = new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f));
            }
            else
            {
                newAgent.transform.GetChild(3).GetComponent<SkinnedMeshRenderer>().material.color = new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f));
            }
            newAgent.SetActive(true);

            NavagentSpawner.Instance.RVOGameObject[i] = newAgent;
            NavagentSpawner.Instance.RVOPointCloud[i] = sp.position;
            NavagentSpawner.Instance.TypeOfSimulation[i] = 4; // waited agent
        }

        for (int i = 0; i < dataLength * 1 / 3; i++)
        //for(int i = dataLength * 1 / 6; i < dataLength; i++)
        {
            GameObject newAgent;
            int prefabNum = Random.Range(0, 2);
            if (prefabNum == 0)
            {
                newAgent = Instantiate(MAgentPf);
            }
            else
            {
                newAgent = Instantiate(WMAgentPf);
            }
            newAgent.name += 4500 + i;
            newAgent.transform.position = data.positions[i] + new Vector3(0.0f, 100.0f, 0.0f);
            newAgent.transform.rotation = Quaternion.Euler(data.rotations[i]);
            newAgent.GetComponent<PlayerMovement>().target = GameObject.Find(data.targetPosNames[i]).transform;
            newAgent.transform.parent = GameObject.Find("WaitAgents").transform;
            newAgent.GetComponent<NavMeshAgent>().enabled = false;
            newAgent.GetComponent<PlayerMovement>().enabled = false;
            SPHProperties sp = newAgent.GetComponent<SPHProperties>();
            sp.position = newAgent.transform.position;
            if (newAgent.name.StartsWith("w"))
            {
                newAgent.transform.GetChild(4).GetComponent<SkinnedMeshRenderer>().material.color = new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f));
            }
            else
            {
                newAgent.transform.GetChild(3).GetComponent<SkinnedMeshRenderer>().material.color = new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f));
            }
            newAgent.SetActive(true);

            NavagentSpawner.Instance.RVOGameObject[dataLength + i] = newAgent;
            NavagentSpawner.Instance.RVOPointCloud[dataLength + i] = sp.position;
            NavagentSpawner.Instance.TypeOfSimulation[dataLength + i] = 4; // waited agent
        }
        /*
        //for (int i = 0; i < data.positions.Length / 2 - 3; i++)
        for (int i = 0; i < data.positions.Length; i++)
        {
            GameObject newAgent;
            if (i % 2 == 0)
            {
                newAgent = Instantiate(MAgentPf);
            }
            else
            {
                newAgent = Instantiate(WMAgentPf);
            }
            newAgent.name += i + data.positions.Length;
            newAgent.transform.position = data.positions[i];
            newAgent.transform.rotation = Quaternion.Euler(data.rotations[i]);
            newAgent.GetComponent<PlayerMovement>().target = GameObject.Find(data.targetPosNames[i]).transform;
            newAgent.transform.parent = GameObject.Find("RVOAgents").transform;
            newAgent.GetComponent<NavMeshAgent>().enabled = true;
            newAgent.GetComponent<PlayerMovement>().enabled = true;
            SPHProperties sp = newAgent.GetComponent<SPHProperties>();
            sp.position = newAgent.transform.position;
            newAgent.SetActive(true);

            NavagentSpawner.Instance.RVOGameObject[i + data.positions.Length] = newAgent;
            NavagentSpawner.Instance.RVOPointCloud[i + data.positions.Length] = sp.position;
            NavagentSpawner.Instance.TypeOfSimulation[i + data.positions.Length] = 0;

            displayAgentNumber.agentNumber++;
        }
        */
        //DebugRVOGameObject();
        NavagentSpawner.Instance.RVOKDTree = new KDTree(NavagentSpawner.Instance.RVOPointCloud, NavagentSpawner.Instance.maxPointsPerLeafNode);
    }

    /*
    private RecorderWindow GetRecorderWindow()
    {
        return (RecorderWindow)EditorWindow.GetWindow(typeof(RecorderWindow));
    }
    */

    private void DebugRVOGameObject()
    {
        for(int i = 0; i < NavagentSpawner.Instance.RVOGameObject.Length; i++)
        {
            Debug.Log(NavagentSpawner.Instance.RVOGameObject[i]);
        }
    }
}

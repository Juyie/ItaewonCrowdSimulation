using System.Collections;
using System.Collections.Generic;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class GridAgentList : MonoBehaviour
{
    [SerializeField]
    private bool printList;

    List<GameObject> agentList = new List<GameObject>();
    Dictionary<GameObject, float> agentSpeedDictionary = new Dictionary<GameObject, float>();

    private float exponentialNum = 3.0f;
    private SPHManagerSingleThread SPHManager;

    public void SetExponentialNum(float num)
    {
        exponentialNum = num;
    }

    // Start is called before the first frame update
    void Start()
    {
        SPHManager = GameObject.Find("SPHManager").GetComponent<SPHManagerSingleThread>();
    }

    // Update is called once per frame
    void Update()
    {
        if (printList)
        {
            PrintList();
        }
    }

    // List
    public int GetListLength()
    {
        return agentList.Count;
    }

    public void AddAgent(GameObject agent)
    {
        //Debug.Log("Add: " + name);
        agentList.Add(agent);
        //Debug.Log(agentList.Count);
        //PrintList();
    }

    public bool FindAgent(GameObject agent)
    {
        if (agentList.Contains(agent))
        {
            return true;
        }
        else
        {
            return false;
        }

    }

    public void RemoveAgent(GameObject agent)
    {
        //Debug.Log("Remove: " + name);
        agentList.Remove(agent);
        //Debug.Log(agentList.Count);
        //PrintList();
    }

    public void TurnOnSPH()
    {
        foreach(GameObject agent in agentList)
        {
            if (agent == null)
            {
                RemoveAgent(agent);
            }
            agent.GetComponent<NavMeshAgent>().enabled = false;
            agent.GetComponent<NavMeshObstacle>().enabled = true;
            agent.GetComponent<SPHProperties>().position = agent.transform.position;
            agent.transform.parent = GameObject.Find("SPHAgents").transform;
            NavagentSpawner.Instance.TypeOfSimulation[int.Parse(agent.name.Substring(23))] = 1;
            //SPHManagerSingleThread.Instance.particles[int.Parse(agent.name.Substring(23))].Init(agent.GetComponent<SPHProperties>().position, agent.GetComponent<SPHProperties>().goalPosition, agent.GetComponent<SPHProperties>().parameterID, agent);
            agent.transform.GetChild(0).GetComponent<SkinnedMeshRenderer>().material.color = Color.yellow;
            //agent.transform.position -= new Vector3(0, SPHManager.parameters[0].particleRadius / 2, 0);
        }
    }

    public void TurnOnSPHZombie()
    {
        foreach(GameObject agent in agentList)
        {
            if (agent == null)
            {
                RemoveAgent(agent);
            }
            agent.GetComponent<SPHProperties>().goalForce = 0;
            agent.transform.GetChild(0).GetComponent<SkinnedMeshRenderer>().material.color = Color.green;
        }
    }

    public void CheckAndTurnOnSPH()
    {
        for (int i = 0; i < NavagentSpawner.Instance.RVOGameObject.Length; i++)
        {
            GameObject agent = NavagentSpawner.Instance.RVOGameObject[i];
            if (agent.GetComponent<SPHProperties>().density > 4)
            {
                agent.GetComponent <NavMeshAgent>().enabled = false;
                agent.GetComponent<NavMeshObstacle>().enabled |= true;
                agent.GetComponent<SPHProperties>().position = agent.transform.position;
                agent.transform.parent = GameObject.Find("SPHAgents").transform;
                NavagentSpawner.Instance.TypeOfSimulation[i] = 1;
                agent.transform.GetChild(0).GetComponent<SkinnedMeshRenderer>().material.color = Color.yellow;
            }
        }
    }

    public void TurnOffSPH()
    {
        foreach (GameObject agent in agentList)
        {
            if (agent == null)
            {
                RemoveAgent(agent);
            }
            agent.GetComponent<NavMeshAgent>().enabled = true;
            agent.GetComponent<NavMeshObstacle>().enabled = false;
            agent.GetComponent<SPHProperties>().position = agent.transform.position;
            agent.transform.parent = GameObject.Find("RVOAgents").transform;
            NavagentSpawner.Instance.TypeOfSimulation[int.Parse(agent.name.Substring(23))] = 0;
            //InitSPH(int.Parse(agent.name.Substring(23)), agent);
            agent.transform.GetChild(0).GetComponent<SkinnedMeshRenderer>().material.color = Color.white;
        }
    }

    public void TurnOnSPHZombieDensity()
    {
        foreach (GameObject agent in agentList)
        {
            if (agent == null)
            {
                RemoveAgent(agent);
            }
            agent.GetComponent<SPHProperties>().SPHZombieDensity = true;
        }
    }

    public void TurnOffSPHZombieDensity()
    {
        foreach (GameObject agent in agentList)
        {
            if (agent == null)
            {
                RemoveAgent(agent);
            }
            agent.GetComponent<SPHProperties>().SPHZombieDensity = false;
        }
    }

    private void InitSPH(int i, GameObject agent)
    {
        SPHManagerSingleThread.Instance.particles[i].position = agent.transform.position;
        SPHManagerSingleThread.Instance.particles[i].goalPosition = agent.GetComponent<SPHProperties>().goalPosition;
        SPHManagerSingleThread.Instance.particles[i].velocity = Vector3.zero;
        SPHManagerSingleThread.Instance.particles[i].forcePhysic = Vector3.zero;
        SPHManagerSingleThread.Instance.particles[i].forceHeading = Vector3.zero;
        SPHManagerSingleThread.Instance.particles[i].density = 0.0f;
        SPHManagerSingleThread.Instance.particles[i].pressure = 0.0f;
        SPHManagerSingleThread.Instance.particles[i].go = null;
    }

    public void TurnOnRagdolls()
    {
        foreach (GameObject agent in agentList)
        {
            if (agent == null)
            {
                RemoveAgent(agent);
            }
            agent.GetComponent<OnOffRagdoll>().TurnOnRagdoll();
            agent.transform.parent = GameObject.Find("RagdollAgents").transform;
            NavagentSpawner.Instance.TypeOfSimulation[int.Parse(agent.name.Substring(23))] = 2;
            //agent.transform.GetChild(3).GetComponent<SkinnedMeshRenderer>().material.color = Color.red;
        }
    }

    public void TurnOnRagdollDensity()
    {
        foreach (GameObject agent in agentList)
        {
            if (agent == null)
            {
                RemoveAgent(agent);
            }
            agent.GetComponent<SPHProperties>().ragdollDensity = true;
        }
    }

    public void TurnOffRagdollDensity()
    {
        foreach (GameObject agent in agentList)
        {
            if (agent == null)
            {
                RemoveAgent(agent);
            }
            agent.GetComponent<SPHProperties>().ragdollDensity = false;
        }
    }

    public void TurnOffRagdolls()
    {
        foreach (GameObject agent in agentList)
        {
            if (agent == null)
            {
                RemoveAgent(agent);
            }
            agent.GetComponent<OnOffRagdoll>().TurnOffRagdoll();
        }
    }
    private void PrintList()
    {
        string debugString = agentList.Count.ToString() + ": [";

        foreach (GameObject agent in agentList)
        {
            debugString += agent.name + ", ";
        }
        debugString += "]";
        Debug.Log(debugString);
    }

    // Dictionary
    public int GetDictionaryLength()
    {
        return agentSpeedDictionary.Count;
    }

    public float GetSpeedPerAgent()
    {
        float weight = 0;
        foreach (KeyValuePair<GameObject, float> item in agentSpeedDictionary)
        {
            //weight += (1.5f - item.Value) / 1.5f;
            weight += Mathf.Pow((Mathf.Exp((1.5f - item.Value) / 1.5f) - 1) / (Mathf.Exp(1) - 1), exponentialNum);
        }
        return weight;
    }

    public void AddAgentDic(GameObject name, float speed)
    {
        agentSpeedDictionary.Add(name, speed);
    }

    public bool FindAgentDic(GameObject name)
    {
        if (agentSpeedDictionary.ContainsKey(name))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public void FixAgentDic(GameObject name, float speed)
    {
        agentSpeedDictionary[name] = speed;
    }

    public void RemoveAgentDic(GameObject name)
    {
        agentSpeedDictionary.Remove(name);
    }

    public void TurnOnRagdollsDic()
    {
        foreach (KeyValuePair<GameObject, float> item in agentSpeedDictionary)
        {
            item.Key.GetComponent<OnOffRagdoll>().TurnOnRagdoll();
        }
    }
}

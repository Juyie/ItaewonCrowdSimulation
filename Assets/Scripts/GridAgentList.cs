using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GridAgentList : MonoBehaviour
{
    [SerializeField]
    private bool printList;

    List<GameObject> agentList = new List<GameObject>();
    Dictionary<GameObject, float> agentSpeedDictionary = new Dictionary<GameObject, float>();

    private float exponentialNum = 3.0f;

    public void SetExponentialNum(float num)
    {
        exponentialNum = num;
    }

    // Start is called before the first frame update
    void Start()
    {

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

    public void TurnOnRagdolls()
    {
        foreach (GameObject agent in agentList)
        {
            if (agent == null)
            {
                RemoveAgent(agent);
            }
            agent.GetComponent<OnOffRagdoll>().TurnOnRagdoll();
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

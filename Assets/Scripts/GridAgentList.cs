using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GridAgentList : MonoBehaviour
{
    [SerializeField]
    private bool printList;

    List<GameObject> agentList = new List<GameObject>();

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

    public void TurnOnRagdolls()
    {
        foreach (GameObject agent in agentList)
        {
            if(agent == null)
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
}

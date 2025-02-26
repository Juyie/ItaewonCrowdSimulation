using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AgentList : MonoBehaviour
{
    [SerializeField]
    private OnOffRagdoll onOffRagdoll;

    [SerializeField]
    private bool useUI;

    [SerializeField]
    private Text UIText;

    List<string> agentList = new List<string>();

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (useUI)
        {
            UIText.text = agentList.Count.ToString();
        }
    }

    public int GetListLength()
    {
        return agentList.Count;
    }

    public void AddAgent(string name)
    {
        agentList.Add(name);
    }

    public bool FindAgent(string name)
    {
        if (agentList.Contains(name))
        {
            return true;
        }
        else
        {
            return false;
        }

    }

    public void RemoveAgent(string name)
    {
        //Debug.Log("Remove: " + name);
        agentList.Remove(name);
        //Debug.Log(agentList.Count);
        //PrintList();
    }

    private void PrintList()
    {
        

        string debugString = agentList.Count.ToString() + ": [";

        foreach(string agent in agentList)
        {
            debugString += agent + ", ";
        }
        debugString += "]";
        Debug.Log(debugString);
    }
}

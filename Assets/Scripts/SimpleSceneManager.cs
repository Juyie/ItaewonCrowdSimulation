using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.AI;

public class SimpleSceneManager : MonoBehaviour
{
    [SerializeField]
    private GameObject charPref;

    [SerializeField]
    private DisplayAgentNumber displayAgentNum;

    [SerializeField]
    private bool activeRVO = false;

    [SerializeField]
    private bool activeSPH = false;

    [SerializeField]
    private Transform pos1;

    [SerializeField]
    private Transform pos2;

    int count = 0;
    int row = 20;
    float timer = 5.0f;
    bool ready = true;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        /*
        if (Input.GetKeyDown(KeyCode.A))
        {
            if (activeRVO)
            {
                addRVOAgent();
            }
            if (activeSPH)
            {
                addSPHAgents();
            }
        }
        */
        if (ready)
        {
            Invoke("addRVOAgent", 3f);
            ready = false;
        }

        /*
        if (ready && count < 10000)
        {
            ready = false;
            StartCoroutine(addSPHAgentsBySeconds(timer));
        }
        */
    }

    private void addRVOAgent()
    {
        // pos1
        count = displayAgentNum.agentNumber; 
        displayAgentNum.agentNumber++;
        GameObject newAgent = Instantiate(charPref);
        newAgent.name += count.ToString();
        Vector3 randPos = new Vector3(0.0f, 0.0f, Random.Range(-3f, 3f));
        newAgent.transform.position = pos1.position;
        newAgent.transform.parent = GameObject.Find("RVOAgents").transform;
        newAgent.GetComponent<NavMeshAgent>().enabled = true;
        newAgent.GetComponent<PlayerMovement>().enabled = true;
        newAgent.GetComponent<PlayerMovement>().target = pos2;
        SPHProperties sp = newAgent.GetComponent<SPHProperties>();
        sp.position = newAgent.transform.position;
        newAgent.SetActive(true);

        Color randColor = Random.ColorHSV(0, 1, 1, 1, 1, 1);
        newAgent.transform.GetChild(0).GetComponent<SkinnedMeshRenderer>().material.color = randColor;
        newAgent.transform.GetChild(3).GetComponent<SkinnedMeshRenderer>().material.color = Color.red;

        /*
        NavagentSpawner.Instance.RVOAgents[count].density = 1.0f;
        NavagentSpawner.Instance.RVOAgents[count].pressure = 0.0f;
        NavagentSpawner.Instance.RVOAgents[count].go = newAgent;
        */

        while (!newAgent.GetComponent<NavMeshAgent>().isOnNavMesh)
        {
            newAgent.GetComponent<NavMeshAgent>().enabled = false;
            newAgent.GetComponent<NavMeshAgent>().enabled = true;
        }

        // pos2
        count = displayAgentNum.agentNumber;
        displayAgentNum.agentNumber++;
        newAgent = Instantiate(charPref);
        newAgent.name += count.ToString();
        randPos = new Vector3(0.0f, 0.0f, Random.Range(-3f, 3f));
        newAgent.transform.position = pos2.position;
        newAgent.transform.parent = GameObject.Find("RVOAgents").transform;
        newAgent.GetComponent<NavMeshAgent>().enabled = true;
        newAgent.GetComponent<PlayerMovement>().enabled = true;
        newAgent.GetComponent<PlayerMovement>().target = pos1;
        sp = newAgent.GetComponent<SPHProperties>();
        sp.position = newAgent.transform.position;
        newAgent.SetActive(true);

        randColor = Random.ColorHSV(0, 1, 1, 1, 1, 1);
        newAgent.transform.GetChild(0).GetComponent<SkinnedMeshRenderer>().material.color = randColor;
        newAgent.transform.GetChild(3).GetComponent<SkinnedMeshRenderer>().material.color = Color.blue;

        /*
        NavagentSpawner.Instance.RVOAgents[count].density = 1.0f;
        NavagentSpawner.Instance.RVOAgents[count].pressure = 0.0f;
        NavagentSpawner.Instance.RVOAgents[count].go = newAgent;
        */

        while (!newAgent.GetComponent<NavMeshAgent>().isOnNavMesh)
        {
            newAgent.GetComponent<NavMeshAgent>().enabled = false;
            newAgent.GetComponent<NavMeshAgent>().enabled = true;
        }

        //ready = true;
    }

    private void addSPHAgents()
    {
        for (int i = 0; i < 100; i++)
        {
            GameObject newAgent = Instantiate(charPref);
            newAgent.name += count;
            newAgent.transform.position = new Vector3(-6.5f + (i % row) * 0.5f, 12.0f, -15.0f + (i / row) % row * 0.5f);
            newAgent.GetComponent<PlayerMovement>().target = GameObject.Find("SimpleTarget1").transform;
            newAgent.transform.parent = GameObject.Find("SPHAgents").transform;
            SPHProperties sp = newAgent.GetComponent<SPHProperties>();
            sp.position = newAgent.transform.position;
            SPHManagerSingleThread.Instance.particles[int.Parse(newAgent.name.Substring(23))].Init(newAgent.GetComponent<SPHProperties>().position, newAgent.GetComponent<SPHProperties>().goalPosition, newAgent.GetComponent<SPHProperties>().parameterID, newAgent);
            newAgent.SetActive(true);
            count++;
        }
        displayAgentNum.agentNumber += 100;
    }

    IEnumerator addSPHAgentsBySeconds(float second)
    {
        for (int i = 0; i < 100; i++)
        {
            GameObject newAgent = Instantiate(charPref);
            newAgent.name += count;
            newAgent.transform.position = new Vector3(-6.5f + (i % row) * 0.5f, 8.0f, -15.0f + (i / row) % row * 0.5f);
            newAgent.GetComponent<PlayerMovement>().target = GameObject.Find("SimpleTarget1").transform;
            newAgent.transform.parent = GameObject.Find("SPHAgents").transform;
            newAgent.GetComponent<NavMeshAgent>().enabled = false;
            newAgent.GetComponent<NavMeshObstacle>().enabled = true;
            SPHProperties sp = newAgent.GetComponent<SPHProperties>();
            sp.position = newAgent.transform.position;
            SPHManagerSingleThread.Instance.particles[int.Parse(newAgent.name.Substring(23))].Init(newAgent.GetComponent<SPHProperties>().position, newAgent.GetComponent<SPHProperties>().goalPosition, newAgent.GetComponent<SPHProperties>().parameterID, newAgent);
            newAgent.SetActive(true);
            count++;
        }

        for (int i = 0; i < 100; i++)
        {
            GameObject newAgent = Instantiate(charPref);
            newAgent.name += count;
            newAgent.transform.position = new Vector3(-6.5f + (i % row) * 0.5f, 8.0f, 15.0f - (i / row) % row * 0.5f);
            newAgent.GetComponent<PlayerMovement>().target = GameObject.Find("SimpleTarget1").transform;
            newAgent.transform.parent = GameObject.Find("SPHAgents").transform;
            newAgent.GetComponent<NavMeshAgent>().enabled = false;
            newAgent.GetComponent<NavMeshObstacle>().enabled = true;
            SPHProperties sp = newAgent.GetComponent<SPHProperties>();
            sp.position = newAgent.transform.position;
            SPHManagerSingleThread.Instance.particles[int.Parse(newAgent.name.Substring(23))].Init(newAgent.GetComponent<SPHProperties>().position, newAgent.GetComponent<SPHProperties>().goalPosition, newAgent.GetComponent<SPHProperties>().parameterID, newAgent);
            newAgent.SetActive(true);
            count++;
        }

        for (int i = 0; i < 100; i++)
        {
            GameObject newAgent = Instantiate(charPref);
            newAgent.name += count;
            newAgent.transform.position = new Vector3(30.0f - (i % row) * 0.5f, 13.0f, 0.0f - (i / row) % row * 0.5f);
            newAgent.GetComponent<PlayerMovement>().target = GameObject.Find("SimpleTarget1").transform;
            newAgent.transform.parent = GameObject.Find("SPHAgents").transform;
            newAgent.GetComponent<NavMeshAgent>().enabled = false;
            newAgent.GetComponent<NavMeshObstacle>().enabled = true;
            SPHProperties sp = newAgent.GetComponent<SPHProperties>();
            sp.position = newAgent.transform.position;
            SPHManagerSingleThread.Instance.particles[int.Parse(newAgent.name.Substring(23))].Init(newAgent.GetComponent<SPHProperties>().position, newAgent.GetComponent<SPHProperties>().goalPosition, newAgent.GetComponent<SPHProperties>().parameterID, newAgent);
            newAgent.SetActive(true);
            count++;
        }

        for (int i = 0; i < 100; i++)
        {
            GameObject newAgent = Instantiate(charPref);
            newAgent.name += count;
            newAgent.transform.position = new Vector3(-30.0f + (i % row) * 0.5f, 4.0f, 0.0f - (i / row) % row * 0.5f);
            newAgent.GetComponent<PlayerMovement>().target = GameObject.Find("SimpleTarget1").transform;
            newAgent.transform.parent = GameObject.Find("SPHAgents").transform;
            newAgent.GetComponent<NavMeshAgent>().enabled = false;
            newAgent.GetComponent<NavMeshObstacle>().enabled = true;
            SPHProperties sp = newAgent.GetComponent<SPHProperties>();
            sp.position = newAgent.transform.position;
            SPHManagerSingleThread.Instance.particles[int.Parse(newAgent.name.Substring(23))].Init(newAgent.GetComponent<SPHProperties>().position, newAgent.GetComponent<SPHProperties>().goalPosition, newAgent.GetComponent<SPHProperties>().parameterID, newAgent);
            newAgent.SetActive(true);
            count++;
        }

        displayAgentNum.agentNumber += 400;

        yield return new WaitForSeconds(second);
        ready = true;
    }
}

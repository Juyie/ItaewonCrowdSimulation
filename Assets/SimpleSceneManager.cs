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
        if (Input.GetKeyDown(KeyCode.A))
        {
            addSPHAgents();
        }

        if (ready)
        {
            ready = false;
            StartCoroutine(addSPHAgentsBySeconds(timer));
        }
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
            newAgent.transform.position = new Vector3(-6.5f + (i % row) * 0.5f, 12.0f, -15.0f + (i / row) % row * 0.5f);
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
            newAgent.transform.position = new Vector3(-6.5f + (i % row) * 0.5f, 12.0f, 15.0f - (i / row) % row * 0.5f);
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
            newAgent.transform.position = new Vector3(30.0f - (i % row) * 0.5f, 12.0f, 0.0f - (i / row) % row * 0.5f);
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
            newAgent.transform.position = new Vector3(-30.0f + (i % row) * 0.5f, 12.0f, 0.0f - (i / row) % row * 0.5f);
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

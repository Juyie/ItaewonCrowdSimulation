using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SpawnAgents : MonoBehaviour
{
    [SerializeField]
    private Transform spawnTf;

    [SerializeField]
    private GameObject agent;

    [SerializeField]
    private float[] intervalTimes;

    [SerializeField]
    private DisplayAgentNumber displayAgentNumber;

    private bool isReady = true;
    private Vector3 randPos;
    private bool isOn = true;
    private int count = 0;

    // Start is called before the first frame update
    void Awake()
    {
        isOn = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (isReady && isOn)
        {
            StartCoroutine(Spawn());
            isReady = false;
        }

        if (Input.GetKeyDown(KeyCode.Return))
        {
            isOn = !isOn;
        }
    }

    IEnumerator Spawn()
    {
        float intervalTime = Random.Range(intervalTimes[0], intervalTimes[1]);
        yield return new WaitForSeconds(intervalTime - 1);
        GameObject newAgent = Instantiate(agent);
        randPos = new Vector3(Random.Range(-3f, 3f), 0, Random.Range(-16f, 16f));
        newAgent.name += count.ToString();
        newAgent.transform.position = spawnTf.position + randPos;
        newAgent.transform.rotation = agent.transform.rotation;
        newAgent.transform.parent = spawnTf;
        newAgent.GetComponent<NavMeshAgent>().enabled = true;
        newAgent.GetComponent<PlayerMovement>().enabled = true;
        newAgent.SetActive(true);
        displayAgentNumber.agentNumber++;
        isReady = true;
        count++;
    }

    IEnumerator SetReady()
    {
        yield return new WaitForSeconds(1);
        isReady = true;
    }
}

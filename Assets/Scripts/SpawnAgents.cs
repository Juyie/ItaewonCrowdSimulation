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

    private bool isReady = true;

    // Start is called before the first frame update
    void Awake()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (isReady)
        {
            StartCoroutine(Spawn());
            isReady = false;
        }
    }

    IEnumerator Spawn()
    {
        float intervalTime = Random.Range(intervalTimes[0], intervalTimes[1]);
        yield return new WaitForSeconds(intervalTime - 1);
        GameObject newAgent = Instantiate(agent);
        newAgent.transform.position = spawnTf.position;
        newAgent.transform.parent = spawnTf;
        newAgent.GetComponent<NavMeshAgent>().enabled = true;
        newAgent.GetComponent<PlayerMovement>().enabled = true;
        isReady = true;
    }

    IEnumerator SetReady()
    {
        yield return new WaitForSeconds(1);
        isReady = true;
    }
}

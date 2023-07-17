using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NavagentController : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        gameObject.GetComponent<NavMeshAgent>().enabled = true;
    }

    // Update is called once per frame
    void Update()
    {
        if(gameObject.GetComponent<NavMeshAgent>().speed > 0)
        {
            gameObject.GetComponent<Animator>().SetBool("isWalking", true);
        }
        else
        {
            gameObject.GetComponent<Animator>().SetBool("isWalking", false);
        }

        if (gameObject.GetComponent<NavMeshAgent>().isOnNavMesh)
        {
            gameObject.GetComponent<NavMeshAgent>().destination = new Vector3(Random.Range(-4.5f, 4.5f), 0.0f, 19.0f);
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NavagentController : MonoBehaviour
{
    public Vector3 goalPos;

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
            gameObject.GetComponent<NavMeshAgent>().destination = goalPos;
        }

        if(gameObject.GetComponent<NavMeshObstacle>().enabled)
        {
            gameObject.GetComponent<NavMeshAgent>().enabled = false;
        }
    }
}

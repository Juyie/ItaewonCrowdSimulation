using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField]
    public Transform target;

    private Animator animator;
    private NavMeshAgent navMeshAgent;
    private bool isRagdollOn;
    private DisplayAgentNumber displayAgentNumber;

    private bool addTag = false;

    // Start is called before the first frame update
    void Awake()
    {
        animator= GetComponent<Animator>();
        navMeshAgent = GetComponent<NavMeshAgent>();
        //navMeshAgent.SetDestination(target.position);

        displayAgentNumber = FindObjectOfType<DisplayAgentNumber>();
    }

    // Update is called once per frame
    void Update()
    {
        if(!addTag)
        {
            Invoke("AddTag", 3f);
            addTag = true;
        }
        if (navMeshAgent.enabled == true)
        {
            if (navMeshAgent.isOnNavMesh)
            {
                if (navMeshAgent.velocity.sqrMagnitude <= 0.1)
                {
                    animator.SetBool("isWalking", false);
                }
                else
                {
                    animator.SetBool("isWalking", true);
                }

                isRagdollOn = gameObject.GetComponent<OnOffRagdoll>().GetIsRagdollOn();
                if (isRagdollOn)
                {
                    navMeshAgent.isStopped = true;
                }
                else if (!isRagdollOn && navMeshAgent.destination != null)
                {
                    navMeshAgent.SetDestination(target.position);
                    gameObject.GetComponent<SPHProperties>().goalPosition = target.position;
                }

                if (!navMeshAgent.pathPending)
                {
                    if (navMeshAgent.remainingDistance <= 0.3)
                    {
                        //displayAgentNumber.agentNumber--;
                        //Destroy(gameObject);
                    }
                }
            }
            else
            {
                Destroy(gameObject);
            }
        }
    }

    private void AddTag()
    {
        gameObject.tag = "agent";
    }
}

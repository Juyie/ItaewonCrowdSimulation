using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField]
    private Transform target;

    private Animator animator;
    private NavMeshAgent navMeshAgent;
    private bool isRagdollOn;

    // Start is called before the first frame update
    void Awake()
    {
        animator= GetComponent<Animator>();
        navMeshAgent = GetComponent<NavMeshAgent>();
        //navMeshAgent.SetDestination(target.position);
    }

    // Update is called once per frame
    void Update()
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
            else
            {
                navMeshAgent.SetDestination(target.position);
            }

            if (!navMeshAgent.pathPending)
            {
                if (navMeshAgent.remainingDistance < 0.1)
                {
                    Destroy(gameObject);
                }
            }
        }
        else
        {
            Destroy(gameObject);
        }
    }
}

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
    private GameObject targetsParent;
    private Transform startTrans;

    // Start is called before the first frame update
    void Awake()
    {
        animator= GetComponent<Animator>();
        navMeshAgent = GetComponent<NavMeshAgent>();
        //navMeshAgent.SetDestination(target.position);

        displayAgentNumber = FindObjectOfType<DisplayAgentNumber>();

        targetsParent = GameObject.Find("Targets");
    }

    private void Start()
    {
        int randInt = Random.Range(0, 3);
        if (target.name == "Target1")
        {
            startTrans = targetsParent.transform.GetChild(randInt + 1);
        }
        else if (target.name == "Target2")
        {
            switch (randInt)
            {
                case 0:
                    startTrans = targetsParent.transform.GetChild(0);
                    break;
                case 1:
                    startTrans = targetsParent.transform.GetChild(2);
                    break;
                case 2:
                    startTrans = targetsParent.transform.GetChild(3);
                    break;
            }
        }
        else if (target.name == "Target3")
        {
            switch (randInt)
            {
                case 0:
                    startTrans = targetsParent.transform.GetChild(0);
                    break;
                case 1:
                    startTrans = targetsParent.transform.GetChild(1);
                    break;
                case 2:
                    startTrans = targetsParent.transform.GetChild(3);
                    break;
            }
        }
        else if (target.name == "Target4")
        {
            startTrans = targetsParent.transform.GetChild(randInt);
        }
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
                        gameObject.GetComponent<SPHProperties>().position = startTrans.position;
                        gameObject.transform.position = startTrans.position;
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
        else // SPH agent
        {
            if(Vector3.Distance(target.position, gameObject.transform.position) <= 0.3f)
            {
                gameObject.GetComponent<SPHProperties>().position = startTrans.position;
                gameObject.transform.position = startTrans.position;
            }
        }
    }

    private void AddTag()
    {
        gameObject.tag = "agent";
    }
}

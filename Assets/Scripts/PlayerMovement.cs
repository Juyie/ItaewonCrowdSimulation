using System.Collections;
using System.Collections.Generic;
using Unity.Entities.UniversalDelegates;
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

    public Vector3 velocity;
    private Vector3 posBefore;
    private Vector3 posAfter;
    private Vector3 midPosition = new Vector3(24.2f, 0.0f, 5.0f);
    private Vector3 topPosition = new Vector3(44.3f, 2.7f, 5.0f);
    private Vector3 bottomPosition = new Vector3(-1.1f, -2.2f, 5.0f);

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
        if (target == null)
        {
            this.enabled = false;
        }
        else
        {
            int randInt = Random.Range(0, 2);
            if (target.name == "Target1" || target.name == "Target2")
            {
                switch (randInt)
                {
                    case 0:
                        startTrans = targetsParent.transform.GetChild(2);
                        break;
                    case 1:
                        startTrans = targetsParent.transform.GetChild(3);
                        break;
                }
                gameObject.GetComponent<SPHProperties>().goalPosition = bottomPosition;
            }
            else if (target.name == "Target3" || target.name == "Target4")
            {
                switch (randInt)
                {
                    case 0:
                        startTrans = targetsParent.transform.GetChild(0);
                        break;
                    case 1:
                        startTrans = targetsParent.transform.GetChild(1);
                        break;
                }
                gameObject.GetComponent<SPHProperties>().goalPosition = topPosition;
            }

            posBefore = transform.position;
            posAfter = transform.position;
        }
    }

    // Update is called once per frame
    void Update()
    {
        CalculateVelocity();

        if (!addTag)
        {
            Invoke("AddTag", 3f);
            addTag = true;
        }
        if (navMeshAgent.enabled == true)
        {
            if (navMeshAgent.isOnNavMesh)
            {
                if (velocity.sqrMagnitude <= 0.1)
                {
                    animator.SetBool("isWalking", false);
                }
                else
                {
                    animator.SetBool("isWalking", true);
                    animator.speed = GetComponent<NavMeshAgent>().velocity.magnitude / 0.6f;
                }

                isRagdollOn = gameObject.GetComponent<OnOffRagdoll>().GetIsRagdollOn();
                if (isRagdollOn)
                {
                    navMeshAgent.isStopped = true;
                }
                else if (!isRagdollOn && navMeshAgent.destination != null)
                {
                    navMeshAgent.SetDestination(target.position);
                    //gameObject.GetComponent<SPHProperties>().goalPosition = target.position;
                }

                if (!navMeshAgent.pathPending)
                {
                    if (navMeshAgent.remainingDistance <= 0.5f)
                    {
                        gameObject.transform.position = startTrans.position;
                        gameObject.GetComponent<SPHProperties>().position = startTrans.position;
                        //displayAgentNumber.agentNumber--;
                        //Destroy(gameObject);
                    }
                }
            }
            else
            {
                //Destroy(gameObject);
                if(startTrans == null)
                {
                    this.enabled = false;
                }
                gameObject.transform.position = startTrans.position;
                gameObject.GetComponent<SPHProperties>().position = startTrans.position;
            }
        }
        else // SPH agent
        {
            if(Vector3.Distance(target.position, gameObject.transform.position) <= 0.5f)
            {
                gameObject.transform.position = startTrans.position;
                gameObject.GetComponent<SPHProperties>().position = startTrans.position;
            }
        }

        // navigation
        if (target.name == "Target1" || target.name == "Target2")
        {
            if (Vector3.Distance(bottomPosition, gameObject.transform.position) <= 5f && gameObject.GetComponent<SPHProperties>().goalPosition == bottomPosition)
            {
                gameObject.GetComponent<SPHProperties>().goalPosition = midPosition;
            }
            else if (Vector3.Distance(midPosition, gameObject.transform.position) <= 5f && gameObject.GetComponent<SPHProperties>().goalPosition == midPosition)
            {
                gameObject.GetComponent<SPHProperties>().goalPosition = topPosition;
            }
            else if (Vector3.Distance(topPosition, gameObject.transform.position) <= 5f && gameObject.GetComponent<SPHProperties>().goalPosition == topPosition)
            {
                gameObject.GetComponent<SPHProperties>().goalPosition = target.position;
            }
        }
        else if (target.name == "Target3" || target.name == "Target4")
        {
            if (Vector3.Distance(topPosition, gameObject.transform.position) <= 5f && gameObject.GetComponent<SPHProperties>().goalPosition == topPosition)
            {
                gameObject.GetComponent<SPHProperties>().goalPosition = midPosition;
            }
            else if (Vector3.Distance(midPosition, gameObject.transform.position) <= 5f && gameObject.GetComponent<SPHProperties>().goalPosition == midPosition)
            {
                gameObject.GetComponent<SPHProperties>().goalPosition = bottomPosition;
            }
            else if (Vector3.Distance(bottomPosition, gameObject.transform.position) <= 5f && gameObject.GetComponent<SPHProperties>().goalPosition == bottomPosition)
            {
                gameObject.GetComponent<SPHProperties>().goalPosition = target.position;
            }
        }

        // Rendering
        if ((gameObject.transform.position.x > 30 && (gameObject.transform.position.z < -9.21 || gameObject.transform.position.z > 19.1))
            || (gameObject.transform.position.x < 10 && (gameObject.transform.position.z < -7.29 || gameObject.transform.position.z > 14))
            || gameObject.transform.position.x < -4.3f)
        {
            for (int i = 0; i < 5; i++)
            {
                gameObject.transform.GetChild(i).GetComponent<SkinnedMeshRenderer>().enabled = false;
            }
            if (gameObject.name.StartsWith("w")) 
            {
                gameObject.transform.GetChild(5).GetComponent<SkinnedMeshRenderer>().enabled = false;
            }
        }
        else
        {
            for (int i = 0; i < 5; i++)
            {
                gameObject.transform.GetChild(i).GetComponent<SkinnedMeshRenderer>().enabled = true;
            }
            if (gameObject.name.StartsWith("w"))
            {
                gameObject.transform.GetChild(5).GetComponent<SkinnedMeshRenderer>().enabled = true;
            }
        }
    }

    private void AddTag()
    {
        gameObject.tag = "agent";
    }

    private void CalculateVelocity()
    {
        posBefore = posAfter;
        posAfter = transform.position;

        velocity = (posAfter - posBefore) / Time.deltaTime;
    }
}

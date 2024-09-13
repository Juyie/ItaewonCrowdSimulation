using JKress.AITrainer;
using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.MLAgentsExamples;
using UnityEngine;
using UnityEngine.AI;

public class OnOffRagdoll : MonoBehaviour
{
    [SerializeField]
    private bool isForTest;

    [SerializeField]
    private Rigidbody[] rigidbodies;

    [SerializeField]
    private Collider[] bodyColliders;

    [SerializeField]
    private GameObject[] colorBodies;

    [SerializeField]
    private NavMeshObstacle[] navObstacles;

    [SerializeField]
    private CapsuleCollider hardCollider;

    [SerializeField]
    private ObjectContact[] contactDetectors;

    private NavMeshAgent navAgent;

    private Animator animator;
    private bool isRagdollOn = false;
    private PlayerMovement playerMovement;
    private bool isStop = false;

    public bool GetIsRagdollOn()
    {
        return isRagdollOn;
    }

    // Start is called before the first frame update
    void Awake()
    {
        animator = GetComponent<Animator>();

        if(!isForTest)
            TurnOffRigidBody();

        navAgent = GetComponent<NavMeshAgent>();

        playerMovement = GetComponent<PlayerMovement>();

        //TurnOnRigidBody();
    }

    // Update is called once per frame
    void Update()
    {
        
        if(isRagdollOn && Input.GetKeyDown(KeyCode.Space))
        {
            TurnOffRagdoll();
        }
        else if(!isRagdollOn && Input.GetKeyDown(KeyCode.Space))
        {
            TurnOnRagdoll();
        }
        
        /*
        if (isStop)
        {
            navAgent.isStopped = true;
        }
        else
        {
            navAgent.isStopped = false;
        }
        */
    }

    private void TurnOnRigidBody()
    {
        animator.enabled = false;

        for (int i = 0; i < rigidbodies.Length; i++)
        {
            rigidbodies[i].useGravity = true;
            rigidbodies[i].isKinematic = false;
        }

        for(int j = 0; j < bodyColliders.Length; j++)
        {
            bodyColliders[j].enabled = true;
        }
       
        GetComponent<Rigidbody>().mass = 0;
        GetComponent<Rigidbody>().useGravity = true;
        GetComponent<Rigidbody>().freezeRotation = false;


        hardCollider.enabled = false;
    }

    private void TurnOffRigidBody()
    {
        animator.enabled = true;

        for (int i = 0; i < rigidbodies.Length - 1; i++)
        {
            rigidbodies[i].useGravity = false;
            rigidbodies[i].isKinematic = true;
        }

        for (int j = 0; j < bodyColliders.Length; j++)
        {
            bodyColliders[j].enabled = false;
        }

        //hardCollider.enabled = true;
    }

    private void TurnOnChangeColor()
    {
        for(int i = 0; i < colorBodies.Length; i++)
        {
            Color bodyColor = colorBodies[i].GetComponent<SkinnedMeshRenderer>().material.color;
            colorBodies[i].GetComponent<SkinnedMeshRenderer>().material.color = new Color(bodyColor.r, bodyColor.g, bodyColor.b, 1.0f);
            colorBodies[i].GetComponent<ChangeColor>().enabled = true;
        }
    }
    private void TurnOffChangeColor()
    {
        for (int i = 0; i < colorBodies.Length; i++)
        {
            colorBodies[i].GetComponent<ChangeColor>().enabled = false;
            Color bodyColor = colorBodies[i].GetComponent<SkinnedMeshRenderer>().material.color;
            colorBodies[i].GetComponent<SkinnedMeshRenderer>().material.color = new Color(bodyColor.r, bodyColor.g, bodyColor.b, 0.3f);
        }
    }

    public void TurnOnNavObstacles()
    {
        navAgent.enabled = false;
        
        for (int i = 0; i < navObstacles.Length; i++)
        {
            navObstacles[i].enabled = true;
        }
        //StopNavMeshAgent();
    }

    private void TurnOffNavObstacles()
    {
        navAgent.enabled = true;
        
        for (int i = 0; i < navObstacles.Length; i++)
        {
            navObstacles[i].enabled = false;
        }
        //RestartNavMeshAgent();
    }

    private void TurnOnWalkerML()
    {
        GetComponent<WalkerAgent>().enabled = true;
    }

    private void TurnOffWalkerML()
    {
        GetComponent<WalkerAgent>().enabled = false;
    }

    private void TurnOnContactDetectors()
    {
        for (int i = 0; i < contactDetectors.Length; i++)
        {
            contactDetectors[i].enabled = true;
        }
    }

    private void TurnOffContactDetectors()
    {
        for (int i = 0; i < contactDetectors.Length; i++)
        {
            contactDetectors[i].enabled = false;
        }
    }

    public void TurnOnRagdoll()
    {
        TurnOnRigidBody();
        TurnOnChangeColor();
        TurnOnNavObstacles();
        //TurnOnWalkerML();
        //TurnOnContactDetectors();

        isRagdollOn = true;
    }

    public void TurnOffRagdoll()
    {
        TurnOffRigidBody();
        TurnOffChangeColor();
        TurnOffNavObstacles();
        //TurnOffWalkerML();
        //TurnOffContactDetectors();

        isRagdollOn = false;
    }

    private void StopNavMeshAgent()
    {
        isStop = true;
        playerMovement.enabled = false;
        navAgent.speed = 0;
        navAgent.angularSpeed = 0;
        navAgent.acceleration = 0;
    }

    private void RestartNavMeshAgent()
    {
        isStop = false;
        playerMovement.enabled = true;
        navAgent.speed = 1.5f;
        navAgent.angularSpeed = 120f;
        navAgent.acceleration = 3f;
    }
}

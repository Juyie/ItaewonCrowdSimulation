using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;
using UnityEngine.AI;

public class OnOffRagdoll : MonoBehaviour
{
    [SerializeField]
    private Rigidbody[] rigidbodies;

    [SerializeField]
    private GameObject[] colorBodies;

    [SerializeField]
    private NavMeshObstacle[] navObstacles;

    private NavMeshAgent navAgent;

    private Animator animator;
    private bool isRagdollOn = false;

    public bool GetIsRagdollOn()
    {
        return isRagdollOn;
    }

    // Start is called before the first frame update
    void Awake()
    {
        animator = GetComponent<Animator>();

        TurnOffRigidBody();

        navAgent = GetComponent<NavMeshAgent>();
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
    }

    private void TurnOnRigidBody()
    {
        animator.enabled = false;

        for (int i = 0; i < rigidbodies.Length; i++)
        {
            rigidbodies[i].useGravity = true;
            rigidbodies[i].isKinematic = false;
        }
    }

    private void TurnOffRigidBody()
    {
        animator.enabled = true;

        for (int i = 0; i < rigidbodies.Length; i++)
        {
            rigidbodies[i].useGravity = false;
            rigidbodies[i].isKinematic = true;
        }
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

    private void TurnOnNavObstacles()
    {
        navAgent.enabled = false;
        /*
        for (int i = 0; i < navObstacles.Length; i++)
        {
            navObstacles[i].enabled = true;
        }
        */
    }

    private void TurnOffNavObstacles()
    {
        navAgent.enabled = true;
        /*
        for (int i = 0; i < navObstacles.Length; i++)
        {
            navObstacles[i].enabled = false;
        }
        */
    }

    public void TurnOnRagdoll()
    {
        TurnOnRigidBody();
        TurnOnChangeColor();
        TurnOnNavObstacles();

        isRagdollOn = true;
    }

    public void TurnOffRagdoll()
    {
        TurnOffRigidBody();
        TurnOffChangeColor();
        TurnOffNavObstacles();

        isRagdollOn = false;
    }
}

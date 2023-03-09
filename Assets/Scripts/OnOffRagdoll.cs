using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class OnOffRagdoll : MonoBehaviour
{
    [SerializeField]
    private Rigidbody[] rigidbodies;

    [SerializeField]
    private GameObject[] colorBodies;

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

        for (int i = 0; i < rigidbodies.Length; i++)
        {
            TurnOffRigidBody(rigidbodies[i]);
        }
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

    private void TurnOnRigidBody(Rigidbody rigidbody)
    {
        animator.enabled = false;
        rigidbody.useGravity = true;
        rigidbody.isKinematic = false;
    }

    private void TurnOffRigidBody(Rigidbody rigidbody)
    {
        animator.enabled = true;
        rigidbody.useGravity = false;
        rigidbody.isKinematic = true;
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

    public void TurnOnRagdoll()
    {
        for (int i = 0; i < rigidbodies.Length; i++)
        {
            TurnOnRigidBody(rigidbodies[i]);
        }

        TurnOnChangeColor();

        isRagdollOn = true;
    }

    public void TurnOffRagdoll()
    {
        for (int i = 0; i < rigidbodies.Length; i++)
        {
            TurnOffRigidBody(rigidbodies[i]);
        }

        TurnOffChangeColor();

        isRagdollOn = false;
    }
}

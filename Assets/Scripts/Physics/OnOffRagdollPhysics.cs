using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class OnOffRagdollPhysics : MonoBehaviour
{
    [SerializeField]
    private Rigidbody[] rigidbodies;

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

        for(int i = 0; i < rigidbodies.Length; i++)
        {
            TurnOffRagdoll(rigidbodies[i]);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(isRagdollOn && Input.GetKeyDown(KeyCode.Space))
        {
            for (int i = 0; i < rigidbodies.Length; i++)
            {
                TurnOffRagdoll(rigidbodies[i]);
            }

            isRagdollOn = false;
        }
        else if(!isRagdollOn && Input.GetKeyDown(KeyCode.Space))
        {
            for (int i = 0; i < rigidbodies.Length; i++)
            {
                TurnOnRagdoll(rigidbodies[i]);
            }

            isRagdollOn = true;
        }
    }

    private void TurnOnRagdoll(Rigidbody rigidbody)
    {
        animator.enabled = false;
        rigidbody.useGravity = true;
        rigidbody.isKinematic = false;
    }

    private void TurnOffRagdoll(Rigidbody rigidbody)
    {
        animator.enabled = true;
        rigidbody.useGravity = false;
        rigidbody.isKinematic = true;
    }
}

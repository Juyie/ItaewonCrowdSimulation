using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeColor : MonoBehaviour
{
    [SerializeField]
    private GameObject[] bodies;

    [SerializeField]
    private Gradient gradient;

    [Range(0f, 1f)]
    private float t;

    private float force = 0f;
    private float maxForce = 0;
    private float totalWeight = 0;

    private void Awake()
    {
        for(int i = 0; i < bodies.Length; i++)
        {
            totalWeight += bodies[i].GetComponent<Rigidbody>().mass;
        }
        Debug.Log(totalWeight);
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        for(int i = 0; i < bodies.Length; i++)
        {
            force += bodies[i].GetComponent<CalculateContactForce>().forcePower;
        }
        //force /= bodies.Length;
        force /= totalWeight;

        t = Mathf.Min(force / 50, 1f);

        GetComponent<SkinnedMeshRenderer>().material.color = gradient.Evaluate(t);
        if (force > maxForce)
        {
            maxForce = force;
            Debug.Log(maxForce);
        }

        if (Input.GetMouseButton(0))
        {
            if(GetComponent<SkinnedMeshRenderer>().material.color == Color.blue || GetComponent<SkinnedMeshRenderer>().material.color == Color.white) 
            {
                GetComponent<SkinnedMeshRenderer>().enabled = false;
            }
        }
        else
        {
            GetComponent<SkinnedMeshRenderer>().enabled = true;
        }

        force = 0;
    }
}

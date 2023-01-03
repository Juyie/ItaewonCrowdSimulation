using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeColor : MonoBehaviour
{
    [SerializeField]
    private GameObject[] bodies;

    [SerializeField]
    private Material[] colors;

    [SerializeField]
    private float[] boundaryNums;

    private float force = 0f;
    private float maxForce = 0;
    private float totalWeight = 0;

    private void Awake()
    {
        for(int i = 0; i < bodies.Length; i++)
        {
            totalWeight += bodies[i].GetComponent<Rigidbody>().mass;
        }
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
        force /= bodies.Length;

        GetComponent<SkinnedMeshRenderer>().material.color = Color.HSVToRGB(1f, force / 1000, 1f);
        if (force > maxForce)
        {
            maxForce = force;
            Debug.Log(maxForce);
        }
        force = 0;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CalculateForce : MonoBehaviour
{
    [SerializeField]
    private GameObject[] bodyParts;

    private bool once = true;
    public float maxForce = 0f;
    private float force = 0f;
    private DisplayAgentNumber agentNumber;

    // Start is called before the first frame update
    void Start()
    {
        agentNumber = GameObject.Find("Canvas").GetComponent<DisplayAgentNumber>();
    }

    // Update is called once per frame
    void Update()
    {
        force = 0;
        for (int i = 0; i < bodyParts.Length; i++)
        {
            force += bodyParts[i].GetComponent<CalculateContactForce>().forcePower;
        }
        maxForce = Mathf.Max(maxForce, force);

        /*
        if(agentNumber.agentNumber == 5994 && once)
        {
            Debug.Log("Max force: " + maxForce);
            once = false;
        }
        */
    }
}

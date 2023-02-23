using System.Collections;
using System.Collections.Generic;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.UIElements;

public class CalculateContactForcePhysics : MonoBehaviour
{
    public float forcePower;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (transform.root.name != collision.transform.root.name)
        {
            forcePower = collision.impulse.magnitude / Time.fixedDeltaTime;
        }
        //Debug.Log("calculated force: " +forcePower);
    }

    private void OnCollisionStay(Collision collision)
    {
        if (transform.root.name != collision.transform.root.name)
        {
            forcePower = collision.impulse.magnitude / Time.fixedDeltaTime;
        }
        //Debug.Log("calculated force: "  + forcePower);
    }

    private void OnCollisionExit(Collision collision)
    {
        forcePower = 0;
    }
}

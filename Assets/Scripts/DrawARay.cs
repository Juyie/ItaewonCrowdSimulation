using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawARay : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionStay(Collision collision)
    {
        foreach(ContactPoint contact in collision.contacts)
        {
            Debug.Log("draw");
            Debug.DrawRay(contact.point, contact.normal, Color.red);
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        
    }
}

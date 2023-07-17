using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class StopNavigate : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("agent"))
        {
            if (other.GetComponent<OnOffRagdoll>().GetIsRagdollOn())
            {
                GetComponent<OnOffRagdoll>().TurnOnNavObstacles();
            }
        }
    }
}

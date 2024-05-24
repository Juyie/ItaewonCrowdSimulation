using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VisualizeTrigger : MonoBehaviour
{
    private Color originalColor = Color.white;
    private Color redColor = Color.red;

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
            for (int i = 0; i < 5; i++)
            {
                other.transform.GetChild(i).GetComponent<SkinnedMeshRenderer>().material.color = redColor;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("agent"))
        {
            for (int i = 0; i < 5; i++)
            {
                other.transform.GetChild(i).GetComponent<SkinnedMeshRenderer>().material.color = originalColor;
            }
        }
    }
}

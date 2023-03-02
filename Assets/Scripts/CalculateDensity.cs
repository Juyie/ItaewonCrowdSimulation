using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CalculateDensity : MonoBehaviour
{
    [SerializeField]
    private Text densityUIText;

    private int agentNum = 1;

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
        Debug.Log("Trigg Enter");
        agentNum++;
        DisplayUI();
    }

    private void OnTriggerStay(Collider other)
    {
        Debug.Log("Trigg Stay");
    }

    private void OnTriggerExit(Collider other)
    {
        Debug.Log("Trigg Exit");
        agentNum--;
        DisplayUI();
    }
    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("Col Enter");
    }

    private void OnCollisionStay(Collision collision)
    {
        Debug.Log("Col Stay");
    }

    private void DisplayUI()
    {
        densityUIText.text = agentNum.ToString(); 
    }
}

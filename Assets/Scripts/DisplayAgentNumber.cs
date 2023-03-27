using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DisplayAgentNumber : MonoBehaviour
{
    [SerializeField]
    private GameObject[] groups;

    [SerializeField]
    private Text uiText;

    public int agentNumber = 0;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        uiText.text = agentNumber.ToString();
    }
}

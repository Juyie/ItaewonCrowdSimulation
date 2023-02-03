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

    private int agentNumber = 0;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        foreach(GameObject group in groups)
        {
            agentNumber += group.transform.childCount;
        }
        uiText.text = agentNumber.ToString();
        agentNumber = 0;
    }
}

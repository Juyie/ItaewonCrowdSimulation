using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActiveRagdollManager : MonoBehaviour
{
    [SerializeField]
    private GameObject[] ragdolls;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            for(int i = 0; i < ragdolls.Length; i++)
            {
                ragdolls[i].GetComponent<OnOffRagdoll>().TurnOnRagdoll();
            }
        }
    }
}

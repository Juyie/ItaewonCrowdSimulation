using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckNeighborGrid : MonoBehaviour
{
    [SerializeField]
    private CalculateDensityGrid[] neighborGrids;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (CheckNeighbor())
        {
            GetComponent<CalculateDensityGrid>().turnOn = true;
        }
    }

    private bool CheckNeighbor()
    {
        bool check = true;
        for(int i = 0; i < neighborGrids.Length; i++)
        {
            if (!neighborGrids[i].satisfy)
            {
                check = false;
            }
        }
        return check;
    }
}

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
        else
        {
            GetComponent<CalculateDensityGrid>().turnOn = false;
        }

        if (CheckNeighborSPH())
        {
            GetComponent<CalculateDensityGrid>().allSPH = true;
        }
        else
        {
            GetComponent<CalculateDensityGrid>().allSPH = false;
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

    private bool CheckNeighborSPH()
    {
        bool check = true;
        for (int i = 0; i < neighborGrids.Length; i++)
        {
            if (!neighborGrids[i].mySPH)
            {
                check = false;
            }
        }
        return check;
    }
}

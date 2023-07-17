using System.Collections;
using System.Collections.Generic;
using Unity.Physics.Systems;
using UnityEngine;

public class GridController : MonoBehaviour
{
    public bool Step = true;
    public bool Exponential = false;
    public float stepSize = 0.01f;
    public float exponentialSize = 3.0f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Step)
        {
            Exponential = false;
        }
        else
        {
            Exponential = true;
        }
    }
}

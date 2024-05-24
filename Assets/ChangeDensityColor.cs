using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeDensityColor : MonoBehaviour
{
    [SerializeField]
    private GameObject[] colorParts;

    [SerializeField]
    private Gradient gradient;

    [Range(0f, 1f)]
    private float t;

    public int density = 0;
    private float maxDensity = 16;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        t = Mathf.Min(density / maxDensity, 1f);

        for (int j = 0; j < colorParts.Length; j++)
        {
            colorParts[j].GetComponent<SkinnedMeshRenderer>().material.color = gradient.Evaluate(t);
        }
    }
}

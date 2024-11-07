using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeVelocityColor : MonoBehaviour
{
    [SerializeField]
    private GameObject[] colorParts;

    [SerializeField]
    private Gradient gradient;

    [Range(0f, 1f)]
    private float t;

    public float velocity = 0;
    private float maxVelocity = 0.6f;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        velocity = transform.parent.GetComponent<SPHProperties>().velocity.magnitude;
        t = Mathf.Min(velocity / maxVelocity, 1f);

        for (int j = 0; j < colorParts.Length; j++)
        {
            colorParts[j].GetComponent<SkinnedMeshRenderer>().material.color = gradient.Evaluate(t);
        }
    }
}

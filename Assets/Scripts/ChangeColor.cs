using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeColor : MonoBehaviour
{
    [SerializeField]
    private GameObject[] bodyParts;

    [SerializeField]
    private GameObject[] colorParts;

    [SerializeField]
    private Gradient gradient;

    [Range(0f, 1f)]
    private float t;

    [SerializeField]
    private CalculateForce calculateForce;

    private float force = 0f;
    private float maxForce = 0;
    private float totalWeight = 0;
    private float dangerForce = 5000f;

    private void OnEnable()
    {
        calculateForce.enabled = true;
    }

    private void Awake()
    {
        for(int i = 0; i < bodyParts.Length; i++)
        {
            totalWeight += bodyParts[i].GetComponent<Rigidbody>().mass;
        }
        //Debug.Log(totalWeight);
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        force = 0;
        for (int i = 0; i < bodyParts.Length; i++)
        {
            force += bodyParts[i].GetComponent<CalculateContactForce>().forcePower;
        }
        //force /= bodies.Length;
        //force /= totalWeight;

        t = Mathf.Min(force / 5000, 1f);

        for (int j = 0; j < colorParts.Length; j++)
        {
            colorParts[j].GetComponent<SkinnedMeshRenderer>().material.color = gradient.Evaluate(t);
        }
        if (force > maxForce)
        {
            maxForce = force;
            //Debug.Log(maxForce);
        }

        /*
        if (Input.GetMouseButton(0))
        {
            if(GetComponent<SkinnedMeshRenderer>().material.color == Color.blue || GetComponent<SkinnedMeshRenderer>().material.color == Color.white) 
            {
                GetComponent<SkinnedMeshRenderer>().enabled = false;
            }
        }
        else
        {
            GetComponent<SkinnedMeshRenderer>().enabled = true;
        }
        */

        if (force > 5000f)
        {
            bodyParts[0].GetComponent<Rigidbody>().freezeRotation = false;
            bodyParts[0].GetComponent<Rigidbody>().isKinematic = false;
            for (int i = 1; i < bodyParts.Length; i++)
            {
                JointDrive drive = new JointDrive();
                drive.positionSpring = 0;
                drive.positionDamper = 0;

                bodyParts[i].GetComponent<ConfigurableJoint>().angularXDrive = drive;

                bodyParts[i].GetComponent<ConfigurableJoint>().angularYZDrive = drive;
            }
        }
    }
}

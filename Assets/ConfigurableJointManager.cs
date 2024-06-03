using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConfigurableJointManager : MonoBehaviour
{
    public bool springDamper = true;
    public float positionSpring;
    public float positionDamper;

    public bool targetPosition = true;
    public bool targetRotation = true;

    private ConfigurableJoint joint;

    // Start is called before the first frame update
    void Start()
    {
        joint = GetComponent<ConfigurableJoint>();

        if (targetPosition)
        {
            joint.targetPosition = transform.localPosition;
        }

        if (targetRotation)
        {
            joint.targetRotation = transform.localRotation;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (springDamper)
        {
            /*
            JointDrive x = joint.xDrive;
            JointDrive y = joint.yDrive;
            JointDrive z = joint.zDrive;
            JointDrive angX = joint.angularXDrive;
            JointDrive angYZ = joint.angularYZDrive;

            x.positionSpring = positionSpring;
            y.positionSpring = positionSpring;
            z.positionSpring = positionSpring;
            angX.positionSpring = positionSpring;
            angYZ.positionSpring = positionSpring;

            x.positionDamper = positionDamper;
            y.positionDamper = positionDamper;
            z.positionDamper = positionDamper;
            angX.positionDamper = positionDamper;
            angYZ.positionDamper = positionDamper;
            */

            JointDrive slerpDrive = joint.slerpDrive;
            slerpDrive.positionSpring = positionSpring;
            slerpDrive.positionDamper = positionDamper;
        }
    }
}

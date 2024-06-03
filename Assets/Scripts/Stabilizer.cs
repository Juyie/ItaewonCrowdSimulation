using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stabilizer : MonoBehaviour
{
    public float uprightTorque = 1000f;

    public AnimationCurve uprightTorqueFunction;

    Rigidbody rb;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        var uprightAngle = Vector3.Angle(transform.up, Vector3.up) / 180;
        var balancePercent = uprightTorqueFunction.Evaluate(uprightAngle);
        var uprightTorqueVal = balancePercent * uprightTorque;

        var rot = Quaternion.FromToRotation(transform.up, Vector3.up);
        rb.AddTorque(new Vector3(rot.x, rot.y, rot.z) * uprightTorqueVal);
    }
}

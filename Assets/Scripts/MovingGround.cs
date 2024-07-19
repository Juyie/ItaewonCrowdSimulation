using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingGround : MonoBehaviour
{
    private Quaternion nextRotation;
    private float speed = 1f;

    // Start is called before the first frame update
    void Start()
    {
        nextRotation = ChooseRandomRotation();
    }

    // Update is called once per frame
    void Update()
    {
        if (Quaternion.Angle(transform.rotation, nextRotation) < 1f)
        {
            nextRotation = ChooseRandomRotation();
        }
        transform.rotation = Quaternion.Lerp(transform.rotation, nextRotation, speed * Time.deltaTime);
    }

    Quaternion ChooseRandomRotation()
    {
        float x = Random.Range(-15f, 15f);
        float z = Random.Range(-15f, 15f);

        Quaternion rotation = Quaternion.Euler(x, 0f, z);

        return rotation;
    }
}

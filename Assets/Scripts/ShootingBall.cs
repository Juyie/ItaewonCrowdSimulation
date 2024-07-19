using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootingBall : MonoBehaviour
{
    public GameObject agent;

    private Vector3 agentPosition;
    private float force = 5.0f;
    private Rigidbody rb;

    // Start is called before the first frame update
    void Start()
    {
        agentPosition = agent.transform.position;
        rb = GetComponent<Rigidbody>();
        SetRandomPos();
    }

    // Update is called once per frame
    void Update()
    {
        if(transform.localPosition.y < 0.1f)
        {
            SetRandomPos();
        }
        rb.AddForce(Vector3.Normalize(agentPosition - transform.position) * force);
    }

    void SetRandomPos()
    {
        float x = Random.Range(-1.5f, 1.5f);
        float y = Random.Range(1f, 2f);
        float z = Random.Range(-6f, -9f);

        transform.localPosition = new Vector3(x, y, z);
    }
}

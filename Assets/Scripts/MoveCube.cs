using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveCube : MonoBehaviour
{
    [SerializeField]
    private bool moveRight;
    public float forcePower = 10f;

    private Vector3 position;
    private float power = 1f;
    private Rigidbody rb;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        /*
        position = transform.position;
        if (moveRight)
        {
            position.x += Input.GetAxis("Horizontal") * Time.deltaTime * power;
        }
        else
        {
            position.x -= Input.GetAxis("Horizontal") * Time.deltaTime * power;
        }
        transform.position = position;
        */
        if(Input.GetAxis("Horizontal") != 0)
        {
            rb.AddForce(forcePower * new Vector3(-Input.GetAxis("Horizontal"), 0, 0), ForceMode.Acceleration);
        }
        if (Input.GetAxis("Vertical") != 0)
        {
            rb.AddForce(forcePower * new Vector3(0, 0, -Input.GetAxis("Vertical")));
        }
    }
}

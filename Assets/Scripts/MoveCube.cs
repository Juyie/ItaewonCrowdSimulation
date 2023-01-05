using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveCube : MonoBehaviour
{
    [SerializeField]
    private bool moveRight;

    private Vector3 position;
    private float power = 1f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
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
    }
}

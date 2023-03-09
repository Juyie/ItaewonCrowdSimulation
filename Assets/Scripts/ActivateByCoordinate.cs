using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActivateByCoordinate : MonoBehaviour
{
    [SerializeField]
    private Vector2 xCoordinates;

    [SerializeField] 
    private Vector2 zCoordinates;

    [SerializeField]
    private CapsuleCollider bodyCollider;

    [SerializeField]
    private GameObject[] bodies;

    [SerializeField]
    private GameObject densityPlane;

    private bool isActive = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if((transform.position.x >= xCoordinates[0] && transform.position.x <= xCoordinates[1]) 
            && (transform.position.z >= zCoordinates[0] && transform.position.z <= zCoordinates[1]))
        {
            Active();
            Invoke("ActiveAfterSeconds", 1f);
        }
        else
        {
            Deactive();
            Invoke("DeactiveAfterSeconds", 1f);
        }
    }

    private void Active()
    {
        for(int i = 0;  i < bodies.Length; i++)
        {
            bodies[i].SetActive(true);
        }

        densityPlane.SetActive(true);
    }

    private void Deactive()
    {
        for (int i = 0; i < bodies.Length; i++)
        {
            bodies[i].SetActive(false);
        }

        densityPlane.SetActive(false);
    }

    private void ActiveAfterSeconds()
    {
        bodyCollider.enabled = true;
    }

    private void DeactiveAfterSeconds()
    {
        bodyCollider.enabled = false;
    }
}

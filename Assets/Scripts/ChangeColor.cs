using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeColor : MonoBehaviour
{
    //private GameObject arrowPrefab;
    private GameObject arrow;

    public Vector3 scale;
    public Vector3 rotation;
    public Vector3 position;
    public Vector3 originRotation;
    public Vector3 originPosition;
    public bool collisionExit = false;

    // Start is called before the first frame update
    void Start()
    {
        //arrowPrefab = GameObject.Find("Arrow");
        arrow = gameObject.transform.GetChild(0).gameObject;
        originRotation = arrow.transform.localRotation.eulerAngles;
        originPosition = arrow.transform.localPosition;
    }

    // Update is called once per frame
    void Update()
    {
    }

    void OnCollisionEnter(Collision collision)
    {
        //Debug.Log(gameObject.tag + " collision, impulse: " + collision.impulse);
        //arrow = Instantiate(arrowPrefab);
        //arrow.transform.parent = gameObject.transform;

        /*
        scale = new Vector3(1f, 1f, Vector3.Magnitude(collision.impulse) / Time.fixedDeltaTime);
        arrow.transform.localScale = scale;

        rotation = originRotation + collision.impulse;
        arrow.transform.localRotation = Quaternion.Euler(rotation);

        if (originPosition.z == 0)
        {
            position = new Vector3(originPosition.x + 0.04f * (scale.z - 1), 0, 0);
        }
        else if(originPosition.x == 0)
        {
            position = new Vector3(0, 0, originPosition.z + 0.04f * (scale.z - 1));
        }
        arrow.transform.localPosition = position;
        */
    }

    private void OnCollisionStay(Collision collision)
    {
        //Debug.Log(gameObject.tag + " collision, impulse: " + collision.impulse);
        scale = new Vector3(1f, 1f, Vector3.Magnitude(collision.impulse / Time.fixedDeltaTime));
        rotation = originRotation + (collision.impulse / Time.fixedDeltaTime);
        /*
        if (originPosition.z == 0)
        {
            position = new Vector3(originPosition.x, originPosition.y, originPosition.z);
        }
        else if (originPosition.x == 0)
        {
            position = new Vector3(originPosition.x, originPosition.y, originPosition.z);
        }
        */
    }

    private void OnCollisionExit(Collision collision)
    {
        collisionExit = true;
    }
}

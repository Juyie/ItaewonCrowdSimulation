using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateArrow : MonoBehaviour
{
    private GameObject arrowPrefab;
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
        arrowPrefab = GameObject.Find("Arrow");

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

        for(int i = 0; i < collision.contactCount; i++)
        {
            //Debug.Log("contact point " + i + ": " + collision.contacts[i].point);

            //arrow = Instantiate(arrowPrefab);
            //arrow.transform.parent = gameObject.transform;

            scale = new Vector3(1f, 1f, Vector3.Magnitude(collision.impulse / Time.fixedDeltaTime));
            arrow.transform.localScale = scale;

            rotation = originRotation + collision.impulse;
            arrow.transform.localRotation = Quaternion.Euler(rotation);

            /*
            if (originPosition.z == 0)
            {
                position = new Vector3(originPosition.x + 0.04f * (scale.z - 1), 0, 0);
            }
            else if (originPosition.x == 0)
            {
                position = new Vector3(0, 0, originPosition.z + 0.04f * (scale.z - 1));
            }
            */

            /*
            if (arrow.transform.parent.tag == "Left Arm" || arrow.transform.parent.tag == "Left Elbow" || arrow.transform.parent.tag == "Right Arm" || arrow.transform.parent.tag == "Right Elbow")
            {
                position = new Vector3(0, 0, originPosition.z);// + arrow.transform.InverseTransformPoint(collision.contacts[i].point);
            }
            else
            {
                position = new Vector3(originPosition.x, 0, 0);// + arrow.transform.InverseTransformPoint(collision.contacts[i].point);
            }
            arrow.transform.localPosition = position;
            */
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        Debug.Log("Collision Stay");
        for (int i = 0; i < collision.contactCount; i++)
        {
            scale = new Vector3(1f, 1f, Vector3.Magnitude(collision.impulse / Time.fixedDeltaTime));

            rotation = originRotation + (collision.impulse / Time.fixedDeltaTime);

            if (arrow.transform.parent.tag == "Left Arm" || arrow.transform.parent.tag == "Left Elbow" || arrow.transform.parent.tag == "Right Arm" || arrow.transform.parent.tag == "Right Elbow")
            {
                position = new Vector3(0, 0, originPosition.z + 0.04f);// + arrow.transform.InverseTransformPoint(collision.contacts[i].point);

            }
            else
            {
                position = new Vector3(originPosition.x, 0, 0);// + arrow.transform.InverseTransformPoint(collision.contacts[i].point);
            }
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        Debug.Log("Collision Exit");
        collisionExit = true;
    }
}

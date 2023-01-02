using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpdateArrow : MonoBehaviour
{
    private Transform parent;
    private Vector3 scale;
    private Vector3 rotation;
    private Vector3 position;
    private bool collisionExit = false;

    // Start is called before the first frame update
    void Start()
    {
        parent = gameObject.transform.parent;
    }

    // Update is called once per frame
    void Update()
    {
        if (gameObject.transform.parent != null)
        {
            scale = parent.GetComponent<CreateArrow>().scale;
            rotation = parent.GetComponent<CreateArrow>().rotation;
            position = parent.GetComponent<CreateArrow>().position;

            gameObject.transform.localScale = scale;
            gameObject.transform.localRotation = Quaternion.Euler(rotation);
            gameObject.transform.localPosition = position;
        }
    }
}

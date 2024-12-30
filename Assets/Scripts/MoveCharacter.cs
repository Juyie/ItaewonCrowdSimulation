using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveCharacter : MonoBehaviour
{
    public float playerSpeed = 2.0f;
    public float rotateXSpeed = 250.0f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 move = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        gameObject.transform.Translate(move * Time.deltaTime * playerSpeed);

        float y = Input.GetAxis("Mouse X") * rotateXSpeed * Time.deltaTime;
        gameObject.transform.eulerAngles = new Vector3(gameObject.transform.eulerAngles.x, gameObject.transform.eulerAngles.y + y, gameObject.transform.eulerAngles.z);
    }
}

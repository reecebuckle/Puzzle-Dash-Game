using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private CharacterController controller;
    public float speed = 12f;
    // Start is called before the first frame update
    private void Awake()
    {
        controller = gameObject.GetComponent<CharacterController>();
    }


    // Update is called once per frame
    void Update()
    {
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        Vector3 move = transform.right * x + transform.forward * z;

        controller.Move(move * speed * Time.deltaTime);

        // if (Input.GetKeyDown(KeyCode.Space))
        // {
        //     controller.Move(transform.position);
        // }

    }
}

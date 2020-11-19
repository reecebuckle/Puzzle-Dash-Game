using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public CharacterController controller;

    float speed;

    public float walkSpeed = 10f;

    public float sprintSpeed = 20f;

    public float gravity = -10f;
    public float jumpHeight = 5f;

    Vector3 velocity;
    bool isGrounded;
    public float verticalVeloticy;

    void Update()
    {


        isGrounded = controller.isGrounded;
        verticalVeloticy = controller.velocity.y;
        if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
        {
            speed = sprintSpeed;
        }
        else
        {
            speed = walkSpeed;
        }

        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }



        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        Vector3 move = transform.right * x + transform.forward * z;

        controller.Move(move * speed * Time.deltaTime);

        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        velocity.y += (gravity * Time.deltaTime);

        controller.Move(velocity * Time.deltaTime);

    }
}

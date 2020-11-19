using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovementRigidbody : MonoBehaviour
{
    private Rigidbody rb;
    // Start is called before the first frame update
    [SerializeField]
    private float walkSpeed = 10f;
    [SerializeField]
    private float sprintSpeed = 20f;

    public float speed;

    private float movementCap { get { return speed * 2; } }
    public float velocity;
    bool stopMoving;
    public float stoppingSpeed = 1f;
    float x;
    float z;
    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    private void Update()
    {
        x = Input.GetAxis("Horizontal");
        z = Input.GetAxis("Vertical");

        velocity = rb.velocity.magnitude;
        if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
        {
            speed = sprintSpeed;
        }
        else
        {
            speed = walkSpeed;
        }
        if (x == 0 && z == 0)
        {
            stopMoving = true;
        }
        else
        {
            stopMoving = false;
        }

        if (stopMoving)
            rb.velocity = Vector3.MoveTowards(rb.velocity, new Vector3(Vector3.zero.x, rb.velocity.y, Vector3.zero.z), stoppingSpeed);
    }
    void FixedUpdate()
    {
        if (rb.velocity.magnitude >= movementCap)
            return;



        Vector3 moveDir = ((x * transform.right) + (z * transform.forward)) * speed;

        rb.AddForce(moveDir);


    }
    void slowDown()
    {

    }
}

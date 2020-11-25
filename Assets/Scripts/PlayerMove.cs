
using System;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{

    //Assingables
    public Transform playerCam;
    public Transform orientation;

    public Transform raycastOrigin;
    //Other
    private Rigidbody rb;

    //Rotation and look
    private float xRotation;
    private float sensitivity = 50f;
    private float sensMultiplier = 1f;

    //Movement
    public float moveSpeed = 4500;

    public float walkSpeed = 4500f;
    public float sprintSpeed = 6000f;
    public float maxSpeed = 20;
    public bool grounded;
    public LayerMask whatIsGround;

    public float counterMovement = 0.175f;
    private float threshold = 0.01f;
    public float maxSlopeAngle = 35f;

    public float slopeAngle;

    // Wallrun
    [HideInInspector]
    public bool wallColliding = false;
    public bool wallRunning = false;
    [HideInInspector]
    public bool wallRunLeft = false;
    [HideInInspector]
    public bool WallRunRight = false;
    //Crouch & Slide
    private Vector3 crouchScale = new Vector3(1, 0.5f, 1);
    private Vector3 playerScale;
    public float slideForce = 400;
    float slideCounterMovement;

    public float flatSlideCounterMovement = 0.2f;
    //Jumping
    private bool readyToJump = true;
    private float jumpCooldown = 0.25f;
    public float jumpForce = 550f;

    //Input
    float x, y;
    bool jumping, sprinting, crouching;

    //Sliding
    private Vector3 normalVector = Vector3.up;
    private Vector3 wallNormalVector;
    public SoundPlayer soundPlayer;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();

    }

    void Start()
    {
        playerScale = transform.localScale;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }


    private void FixedUpdate()
    {
        if (!grounded && y != 0)
        {
            WallRun();
        }
        else if (wallRunning)
        {
            resetWallRun();
        }
        Movement();

    }

    private void Update()
    {
        determineSlope();
        MyInput();
        Look();

    }
    private void determineSlope()
    {
        RaycastHit hit;

        if (Physics.Raycast(raycastOrigin.position, raycastOrigin.TransformDirection(Vector3.down), out hit))
        {
            slopeAngle = Vector3.Angle(hit.normal, raycastOrigin.forward) - 90;
        }
    }
    private void MyInput()
    {
        x = Input.GetAxisRaw("Horizontal");
        y = Input.GetAxisRaw("Vertical");
        jumping = Input.GetButton("Jump");
        crouching = Input.GetKey(KeyCode.LeftControl);
        sprinting = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);

        // moveSpeed = sprinting ? sprintSpeed : walkSpeed;
        // maxSpeed = sprinting ? 40 : 20;
        moveSpeed = walkSpeed;
        maxSpeed = 20f;
        slideCounterMovement = (slopeAngle < -10) ? -1 : flatSlideCounterMovement;

        if (grounded && (x != 0 || y != 0))
        {
            if (!soundPlayer.audio.isPlaying)
                soundPlayer.PlaySound(soundPlayer.running);
        }

        //Crouching
        if (Input.GetKeyDown(KeyCode.LeftControl))
            StartCrouch();
        if (Input.GetKeyUp(KeyCode.LeftControl))
            StopCrouch();
    }
    private void WallRun()
    {
        float wallRunSpeed = 1f;
        float wallRunSpeedCap = 25f;
        Vector3 leftBack = new Vector3(-1, 0, -1);
        Vector3[] directions = {
            orientation.transform.TransformDirection(new Vector3(-1, 0, -1).normalized),//back left
            orientation.transform.TransformDirection(Vector3.left),//left
            orientation.transform.TransformDirection(new Vector3(-1, 0, 1).normalized),//front left
            orientation.transform.TransformDirection(new Vector3(1, 0, 1).normalized),//front right
            orientation.transform.TransformDirection(Vector3.right),//right
            orientation.transform.TransformDirection(new Vector3(1, 0, -1).normalized)//back right
        };
        bool foundWallInRange = false;
        for (int i = 0; i < directions.Length; i++)
        {
            Vector3 raycastDirection = directions[i];

            Debug.DrawRay(orientation.position, raycastDirection, Color.blue);
            // orientation of wallrun?
            RaycastHit hit;
            if (Physics.Raycast(orientation.position, raycastDirection, out hit))
            {
                float angle = Vector3.Angle(hit.normal, Vector3.up);

                if (hit.distance <= 0.8f && (angle <= 100 && angle >= 80))
                {
                    if (!jumping)
                    {
                        if (i <= 2)
                        {
                            wallRunLeft = true;
                        }
                        else
                        {
                            WallRunRight = true;
                        }
                        wallRunning = true;
                        foundWallInRange = true;
                        Vector3 velocity = rb.velocity;
                        Vector3 normal = hit.normal;
                        normal.y = 0;
                        velocity.y = 0;
                        rb.AddForce(normal * jumpForce / 4 * -1);
                        normal.Normalize();
                        rb.velocity = velocity;
                        velocity = Vector3.ProjectOnPlane(velocity, normal);
                        Vector3 direction = velocity.normalized;
                        Debug.DrawRay(hit.point, direction, Color.green);
                        maxSpeed = sprintSpeed;
                        // rb.velocity = velocity;
                        rb.useGravity = false;
                        // rb.isKinematic = true;
                        // grounded = true;
                        // Debug.Log(rb.velocity.magnitude);
                        // transform.position = Vector3.MoveTowards(transform.position, direction * wallRunSpeed * Time.deltaTime, 0.5f);

                        if (rb.velocity.magnitude <= wallRunSpeedCap)
                        {
                            rb.AddForce(direction * wallRunSpeed * Time.deltaTime);
                        }
                        else
                        {
                            rb.velocity = rb.velocity.normalized * wallRunSpeedCap;
                        }
                        // if we detected a raycast in the wall run range, stop checking for the other one
                    }
                    else
                    {
                        // jump off
                        rb.AddForce(hit.normal * (jumpForce / 3));
                        resetWallRun();
                        grounded = true;
                        Jump();
                    }
                    break;
                }
            }
            else
            {
                // resetWallRun();
            }
        }
        if (!foundWallInRange)
        {
            // if none of the raycasts have found a wall in range
            resetWallRun();
        }

    }
    private void resetWallRun()
    {
        maxSpeed = sprintSpeed;
        rb.isKinematic = false;
        wallRunning = false;
        grounded = false;
        rb.useGravity = true;
        wallRunLeft = false;
        WallRunRight = false;
    }
    private void StartCrouch()
    {

        transform.localScale = crouchScale;
        transform.position = new Vector3(transform.position.x, transform.position.y - 0.5f, transform.position.z);
        if (rb.velocity.magnitude > 0.5f)
        {
            if (grounded)
            {
                rb.AddForce(orientation.transform.forward * slideForce);
            }
        }
    }

    private void StopCrouch()
    {
        transform.localScale = playerScale;
        transform.position = new Vector3(transform.position.x, transform.position.y + 0.5f, transform.position.z);
    }

    private void Movement()
    {
        //Extra gravity
        if (!wallRunning)
            rb.AddForce(Vector3.down * Time.deltaTime * 10);

        //Find actual velocity relative to where player is looking
        Vector2 mag = FindVelRelativeToLook();
        float xMag = mag.x, yMag = mag.y;

        //Counteract sliding and sloppy movement
        CounterMovement(x, y, mag);

        //If holding jump && ready to jump, then jump
        if (readyToJump && jumping) Jump();

        //Set max speed
        float maxSpeed = this.maxSpeed;

        //If sliding down a ramp, add force down so player stays grounded and also builds speed
        if (crouching && grounded && readyToJump)
        {
            rb.AddForce(Vector3.down * Time.deltaTime * 3000);
            return;
        }

        //If speed is larger than maxspeed, cancel out the input so you don't go over max speed
        if (x > 0 && xMag > maxSpeed) x = 0;
        if (x < 0 && xMag < -maxSpeed) x = 0;
        if (y > 0 && yMag > maxSpeed) y = 0;
        if (y < 0 && yMag < -maxSpeed) y = 0;

        //Some multipliers
        float multiplier = 1f, multiplierV = 1f;

        // Movement in air
        if (!grounded)
        {
            multiplier = 0.5f;
            multiplierV = 0.5f;
        }

        // Movement while sliding
        if (grounded && crouching) multiplierV = 0f;

        //Apply forces to move player
        rb.AddForce(orientation.transform.forward * y * moveSpeed * Time.deltaTime * multiplier * multiplierV);
        rb.AddForce(orientation.transform.right * x * moveSpeed * Time.deltaTime * multiplier);

    }

    private void Jump()
    {
        if (grounded && readyToJump)
        {
            readyToJump = false;

            //Add jump forces
            rb.AddForce(Vector2.up * jumpForce * 1.5f);
            rb.AddForce(normalVector * jumpForce * 0.5f);

            //If jumping while falling, reset y velocity.
            Vector3 vel = rb.velocity;
            if (rb.velocity.y < 0.5f)
                rb.velocity = new Vector3(vel.x, 0, vel.z);
            else if (rb.velocity.y > 0)
                rb.velocity = new Vector3(vel.x, vel.y / 2, vel.z);

            soundPlayer.PlaySound(soundPlayer.jump);
            Invoke(nameof(ResetJump), jumpCooldown);
        }
    }

    private void ResetJump()
    {
        readyToJump = true;
    }

    private float desiredX;
    private void Look()
    {
        float mouseX = Input.GetAxis("Mouse X") * sensitivity * Time.fixedDeltaTime * sensMultiplier;
        float mouseY = Input.GetAxis("Mouse Y") * sensitivity * Time.fixedDeltaTime * sensMultiplier;

        //Find current look rotation
        Vector3 rot = playerCam.transform.localRotation.eulerAngles;
        desiredX = rot.y + mouseX;

        //Rotate, and also make sure we dont over- or under-rotate.
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        //Perform the rotations
        playerCam.transform.localRotation = Quaternion.Euler(xRotation, desiredX, 0);
        orientation.transform.localRotation = Quaternion.Euler(0, desiredX, 0);
    }

    private void CounterMovement(float x, float y, Vector2 mag)
    {
        if (!grounded || jumping) return;

        //Slow down sliding
        if (crouching)
        {
            rb.AddForce(moveSpeed * Time.deltaTime * -rb.velocity.normalized * slideCounterMovement);
            return;
        }

        //Counter movement
        if (Math.Abs(mag.x) > threshold && Math.Abs(x) < 0.05f || (mag.x < -threshold && x > 0) || (mag.x > threshold && x < 0))
        {
            rb.AddForce(moveSpeed * orientation.transform.right * Time.deltaTime * -mag.x * counterMovement);
        }
        if (Math.Abs(mag.y) > threshold && Math.Abs(y) < 0.05f || (mag.y < -threshold && y > 0) || (mag.y > threshold && y < 0))
        {
            rb.AddForce(moveSpeed * orientation.transform.forward * Time.deltaTime * -mag.y * counterMovement);
        }

        //Limit diagonal running. This will also cause a full stop if sliding fast and un-crouching, so not optimal.
        if (Mathf.Sqrt((Mathf.Pow(rb.velocity.x, 2) + Mathf.Pow(rb.velocity.z, 2))) > maxSpeed)
        {
            float fallspeed = rb.velocity.y;
            Vector3 n = rb.velocity.normalized * maxSpeed;
            rb.velocity = new Vector3(n.x, fallspeed, n.z);
        }
    }

    /// <summary>
    /// Find the velocity relative to where the player is looking
    /// Useful for vectors calculations regarding movement and limiting movement
    /// </summary>
    /// <returns></returns>
    public Vector2 FindVelRelativeToLook()
    {
        float lookAngle = orientation.transform.eulerAngles.y;
        float moveAngle = Mathf.Atan2(rb.velocity.x, rb.velocity.z) * Mathf.Rad2Deg;

        float u = Mathf.DeltaAngle(lookAngle, moveAngle);
        float v = 90 - u;

        float magnitue = rb.velocity.magnitude;
        float yMag = magnitue * Mathf.Cos(u * Mathf.Deg2Rad);
        float xMag = magnitue * Mathf.Cos(v * Mathf.Deg2Rad);

        return new Vector2(xMag, yMag);
    }

    private bool IsFloor(Vector3 v)
    {
        float angle = Vector3.Angle(Vector3.up, v);
        return angle < maxSlopeAngle;
    }

    private bool cancellingGrounded;

    /// <summary>
    /// Handle ground detection
    /// </summary>
    private void OnCollisionStay(Collision other)
    {
        //Make sure we are only checking for walkable layers
        int layer = other.gameObject.layer;
        if (whatIsGround != (whatIsGround | (1 << layer))) return;

        //Iterate through every collision in a physics update
        for (int i = 0; i < other.contactCount; i++)
        {
            Vector3 normal = other.contacts[i].normal;
            //FLOOR
            if (IsFloor(normal))
            {
                grounded = true;
                cancellingGrounded = false;
                normalVector = normal;
                CancelInvoke(nameof(StopGrounded));
            }
        }

        //Invoke ground/wall cancel, since we can't check normals with CollisionExit
        float delay = 3f;
        if (!cancellingGrounded)
        {
            cancellingGrounded = true;
            Invoke(nameof(StopGrounded), Time.deltaTime * delay);
        }
    }

    private void StopGrounded()
    {
        grounded = false;
    }

}
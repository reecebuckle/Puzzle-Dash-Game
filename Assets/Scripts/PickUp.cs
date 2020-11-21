﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUp : MonoBehaviour
{
    //Set these in Unity (except look object is just for reference)
    [Header("Player Holding Properities")]
    [SerializeField] private Transform guide = null;
    [SerializeField] private Transform defaultGuide = null;

    //[SerializeField] private Transform orientation = null;
    public int interactableLayerIndex;
    public GameObject lookObject;

    //Can toggle these in Unity
    [Header("Object Movement Speed / Range")]
    public GameObject heldObject;
    [SerializeField] private float minSpeed = 0;
    [SerializeField] private float maxSpeed = 7000f;
    [SerializeField] private float minDistance = 2f;
    [SerializeField] private float maxDistance = 15f;
    [SerializeField] private float moveAmount = 15f;

    //Camera
    private Camera myCamera;

    //Reference to MovableObject
    private MovableObject movableObject;

    //Speed paremters which apply to objects movement speed
    private float currentSpeed = 0f;
    private float currentDist = 0f;
    private float objRotationSpeed = 100f;
    private Rigidbody heldObjectRB;

    //Variables in regards to viewing / looking at the obstacle
    Quaternion lookRot;
    private float sphereCastRadius = 0.5f;
    private Vector3 raycastPos;

    //Currently not being used
    private float rotationSpeed = 100f;

    //private Vector3 originalPos;
    // private Transform defaultParentPos;

    //Assign camera
    private void Start()
    {
        myCamera = Camera.main;
    }

    void Update()
    {
        //Look for holdable objects
        lookForObjects();

        //Drop held object if it surpasses max distance from player (usually 15f)
        if (heldObject != null)
        {
            //float objDistanceToParent = Vector3.Distance (heldObject.transform.position, guide.transform.position);
            float playerDistanceToObject = Vector3.Distance(transform.position, heldObject.transform.position);
            if (playerDistanceToObject > (maxDistance + 2f))
            {
                DropObject();
            }
        }

        //Upon pressing E
        if (Input.GetKeyDown(KeyCode.E))
        {
            //and we aren't holding an object and actively looking at an object
            if (heldObject == null && lookObject != null)
            {
                PickUpObject();
            }
            //if we press E whilst holding object, we drop it
            else if (heldObject != null)
            {
                DropObject();
            }
        }

        //Move held object position towards / away player
        if (Input.GetKey(KeyCode.F) && heldObject != null)
        {
            transformObjectPosition();
        }
    }

    //Velocity movement toward pickup parent and rotation
    private void FixedUpdate()
    {
        if (heldObject != null)
        {
            currentDist = Vector3.Distance(guide.position, heldObjectRB.position);
            currentSpeed = Mathf.SmoothStep(minSpeed, maxSpeed, currentDist / maxDistance);
            currentSpeed *= Time.fixedDeltaTime;
            Vector3 direction = guide.position - heldObjectRB.position;
            heldObjectRB.velocity = direction.normalized * currentSpeed;
            //Rotation
            lookRot = Quaternion.LookRotation(myCamera.transform.position - heldObjectRB.position);
            lookRot = Quaternion.Slerp(myCamera.transform.rotation, lookRot, rotationSpeed * Time.fixedDeltaTime);
            heldObjectRB.MoveRotation(lookRot);
        }

    }

    //Handles positive of object relative to player
    public void transformObjectPosition()
    {
        //Scroll forwards
        if (Input.GetAxis("Mouse ScrollWheel") > 0f)
        {
            float playerDistanceToParent = Vector3.Distance(transform.position, guide.transform.position);

            //Check object hasn't surpassed max distance
            if (playerDistanceToParent < maxDistance)
            {
                //move position of pickup parent by move amount (away from player)
                guide.transform.Translate(Vector3.forward * Time.fixedDeltaTime * moveAmount);
            }
        }
        //Scroll Backwards
        else if (Input.GetAxis("Mouse ScrollWheel") < 0f)
        {
            float playerDistanceToParent = Vector3.Distance(transform.position, guide.transform.position);
            //Check object hasn't surpassed min distance
            if (playerDistanceToParent > minDistance)
            {
                //move position of pickup parent by move amount (away from player)
                guide.transform.Translate(-Vector3.forward * Time.fixedDeltaTime * moveAmount);
            }
        }
    }



    //Check if we are currently looking at a holdable object (if broken then drop)
    public void lookForObjects()
    {
        raycastPos = myCamera.ScreenToWorldPoint(new Vector3(Screen.width / 2, Screen.height / 2, 0));
        RaycastHit hit;
        if (Physics.SphereCast(raycastPos, sphereCastRadius, myCamera.transform.forward, out hit, maxDistance, 1 << interactableLayerIndex))
        {
            lookObject = hit.collider.transform.root.gameObject;
        }
        else
        {
            lookObject = null;
        }
    }

    //Pick up Object
    public void PickUpObject()
    {
        //assign the looked object 
        movableObject = lookObject.GetComponentInChildren<MovableObject>();
        heldObject = lookObject;
        //grab rigidbody
        heldObjectRB = heldObject.GetComponent<Rigidbody>();
        //Unfreeze position so can be moved around
        heldObjectRB.constraints = RigidbodyConstraints.None;
        //re-freeze rotation when held
        heldObjectRB.constraints = RigidbodyConstraints.FreezeRotation;
        movableObject.pickupParent = this;
        //Pick up object smoothly with a short wait
        StartCoroutine(movableObject.PickUp());
    }

    //Drop object
    public void DropObject()
    {
        //reset constraints
        heldObjectRB.constraints = RigidbodyConstraints.None;
        heldObject = null;
        movableObject.pickedUp = false;
        //Freeze position and rotation of obstacle by a few seconds (dropFreezeTime) 
        currentDist = 0;
        //reset guide position upon dropping
        guide.transform.position = defaultGuide.transform.position;
    }

    //These two methods are called by the MovableObject class when it is released from being held
    //and detects a collision with the floor - after dropping a few seconds it then freezes
    public void FreezeObject()
    {
        Debug.Log("Freezing position");
        StartCoroutine(WaitToFreeze());

    }
    public IEnumerator WaitToFreeze()
    {
        Debug.Log("Waiting to Freeze");
        //this checks until vertical velocity is approximately 0 (object has reached a stable point on ground)
        //and then proceeds to freeze its position so that you can climb on it
        yield return new WaitUntil(() => Mathf.Approximately(heldObjectRB.velocity.y, 0) == true);
        heldObjectRB.constraints = RigidbodyConstraints.FreezeAll;
        Debug.Log("Frozen object");

    }


    public void RotateObject()
    {
        //Stop camera movement to allow player to rotate held object
        // if (Input.GetKeyDown(KeyCode.R) && isHolding == true) {
        // myCamera.GetComponent<MouseLook>().enabled = false;
        //transform.root.GetComponent<MouseLook>().enabled = false;

        // }

        //enable rotation of held object
        // if (Input.GetKey(KeyCode.R) && isHolding == true) {
        // var rotationX = Input.GetAxis("Mouse X") * objRotationSpeed;
        //var rotationY = Input.GetAxis("Mouse Y") * objRotationSpeed;
        //movableObject.heldObject.transform.RotateAroundLocal(myCamera.up, -Mathf.Deg2Rad * rotationX);
        //movableObject.heldObject.transform.RotateAroundLocal(myCamera.right, -Mathf.Deg2Rad * rotationY);

        // }

        //  if (Input.GetKeyUp(KeyCode.R) && isHolding == true) {
        // myCamera.GetComponent<MouseLook>().enabled = true;
        // transform.root.GetComponent<MouseLook>().enabled = true;

        // }
    }

}
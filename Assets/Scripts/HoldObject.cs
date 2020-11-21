using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoldObject : MonoBehaviour
{
    //Set these in Unity (except look object is just for reference)
    [Header("Player Holding Properities")]
    [SerializeField] private Transform guide = null;

    //[SerializeField] private Transform orientation = null;
    public int interactableLayerIndex;
    public GameObject lookObject;

    //Can toggle these in Unity
    [Header("Object Holding Movement Speed")]
    public GameObject heldObject;
    [SerializeField] private float minSpeed = 0;
    [SerializeField] private float maxSpeed = 7000f;
    [SerializeField] private float maxDistance = 5f;

    //Camera
    private Camera myCamera;


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
    private float rotationSpeed = 7500f;


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
                PickUpObj();
            }
            //if we press E whilst holding object, we drop it
            else if (heldObject != null)
            {
                DropObject();
            }
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
    public void PickUpObj()
    {
        //assign the looked object 
        //movableObject = lookObject.GetComponentInChildren<MovableObject>();
        heldObject = lookObject;
        //grab rigidbody
        heldObjectRB = heldObject.GetComponent<Rigidbody>();
        //Unfreeze position so can be moved around
        heldObjectRB.constraints = RigidbodyConstraints.None;
        //re-freeze rotation when held
        heldObjectRB.constraints = RigidbodyConstraints.FreezeRotation;
        //movableObject.pickupParent = this;
        //Pick up object smoothly with a short wait
        //StartCoroutine(movableObject.PickUp());
    }

    //Drop object
    public void DropObject()
    {
        //reset constraints
        heldObjectRB.constraints = RigidbodyConstraints.None;
        heldObject = null;
        currentDist = 0;
    }
}

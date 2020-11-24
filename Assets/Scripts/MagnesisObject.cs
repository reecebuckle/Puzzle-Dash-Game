using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagnesisObject : MonoBehaviour
{
    //Set these in Unity (except look object is just for reference)
    [Header("Magnesis-ish Properities")]
    [SerializeField] private Transform guide = null;
    [SerializeField] private Transform defaultGuide = null;
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
    [SerializeField] private float rotationSpeed = 7500f;

    //Camera and Moveable Object
    private Camera myCamera;
    private MovableObject movableObject;

    //Speed paremters which apply to objects movement speed
    private float currentSpeed = 0f;
    private float currentDist = 0f;
    private Rigidbody heldObjectRB;

    //Variables in regards to viewing / looking at the obstacle
    private float sphereCastRadius = 0.5f;
    private Vector3 raycastPos;

    //Assign camera
    private void Start()
    {
        myCamera = Camera.main;
    }

    void Update()
    {
        //Look for holdable objects
        LookForObjects();

        //Drop held object if it surpasses max distance from player (usually 15f)
        if (heldObject != null)
        {
            //Drop held object if sprinting or crouching
            if (Input.GetKeyDown(KeyCode.LeftShift) || Input.GetKeyDown(KeyCode.LeftControl))
                DropObject();

            //small bug here that causes null pointer if doesnt check quick enough
            CheckDropDistance();
        }


        //Upon pressing E
        if (Input.GetKeyDown(KeyCode.E))
        {
            //and we aren't holding an object and actively looking at an object
            if (heldObject == null && lookObject != null)
                PickUpObject();
            //if we press E whilst holding object, we drop it
            else if (heldObject != null)
                DropObject();
        }

        //Move object position (towards/away player) and its rotation
        if (Input.GetKey(KeyCode.F) && heldObject != null)
            TransformObjectPositionRotation();

    }

    //Velocity movement toward pickup parent and rotation
    private void FixedUpdate()
    {
        if (heldObject != null)
        {
            //Smooths object movement
            currentDist = Vector3.Distance(guide.position, heldObjectRB.position);
            currentSpeed = Mathf.SmoothStep(minSpeed, maxSpeed, currentDist / maxDistance);
            currentSpeed *= Time.fixedDeltaTime;
            Vector3 direction = guide.position - heldObjectRB.position;
            heldObjectRB.velocity = direction.normalized * currentSpeed;
        }
    }

    //Checks maximum drop distance
    public void CheckDropDistance()
    {
        //Checks distance between player and max hold distance and drops if required
        float playerDistanceToObject = Vector3.Distance(transform.position, heldObject.transform.position);
        if (playerDistanceToObject > (maxDistance + 2f))
            DropObject();
    }

    //Handles positive of object relative to player
    //This is called from Update() not fixed update since it doesn't actually change any physics of the obstacle, 
    //just it's transform + it works a lot smoother when matched with player's frame rate
    public void TransformObjectPositionRotation()
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
        //rotate X axis clockwise
        else if (Input.GetMouseButton(0))
        {
            float rotAmount = rotationSpeed * Mathf.Deg2Rad * Time.deltaTime;
            heldObject.transform.Rotate(0, rotAmount, 0, Space.Self);
        }
        //rotate y axis clockwise
        else if (Input.GetMouseButton(1))
        {
            float rotAmount = rotationSpeed * Mathf.Deg2Rad * Time.deltaTime;
            heldObjectRB.transform.Rotate(rotAmount, 0, 0, Space.Self);
        }
    }


    //Check if we are currently looking at a holdable object (if broken then drop)
    public void LookForObjects()
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
}
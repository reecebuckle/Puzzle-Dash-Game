using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoldObject : MonoBehaviour
{
    //Set these in Unity (except look object is just for reference)
    [Header("Player Holding Properities")]
    [SerializeField] private Transform holdPosition = null;
    public int interactableLayerIndex;
    public GameObject lookObject;

    //Can toggle these in Unity
    [Header("Object Holding Movement Speed")]
    public GameObject heldObject;
    [SerializeField] private float minSpeed = 0f;
    [SerializeField] private float maxSpeed = 10000f;
    [SerializeField] private float maxDistance = 5f;
    [SerializeField] private float throwForce = 1000f;

    //Camera
    private Camera myCamera;

    //Speed paremters which apply to objects movement speed
    private float currentSpeed = 0f;
    private float currentDist = 0f;
    private Rigidbody heldObjectRB;

    //Variables in regards to viewing / looking at the obstacle
    Quaternion lookRot;
    private float sphereCastRadius = 0.5f;
    private Vector3 raycastPos;

    //Currently not being used
    private float rotationSpeed = 7500f;

    private bool tryThrow = false;


    //Assign camera
    private void Start()
    {
        myCamera = Camera.main;
    }

    //Read input control data in Update (called once per frame)
    void Update()
    {
        //Look for object via Raycasts
        LookForObjects();

        //Drop held object if it surpasses max distance from player (usually 15f)
        if (heldObject != null)
            CheckDropDistance();

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

        //Mouse wheel click leads to throw object
        if (Input.GetMouseButton(2) && heldObject != null)
            ThrowObject();

    }

    //Handle physics/forces and collisisons in FixedUpdate! (Called 0, once or multiple times per frame)
    //Velocity movement toward pickup parent and rotation
    private void FixedUpdate()
    {
        //Calculate position and rotation of held object 
        if (heldObject != null)
        {
            //really jiggery
            //heldObject.transform.position = holdPosition.transform.position;
            currentDist = Vector3.Distance(holdPosition.position, heldObjectRB.position);
            currentSpeed = Mathf.SmoothStep(minSpeed, maxSpeed, currentDist / maxDistance);
            currentSpeed *= Time.fixedDeltaTime;
            Vector3 direction = holdPosition.position - heldObjectRB.position;
            heldObjectRB.velocity = direction.normalized * currentSpeed;

            //Rotation
            lookRot = Quaternion.LookRotation(myCamera.transform.position - heldObjectRB.position);
            lookRot = Quaternion.Slerp(myCamera.transform.rotation, lookRot, rotationSpeed * Time.fixedDeltaTime);
            heldObjectRB.MoveRotation(lookRot);

            //If read a throw command and holding object, add force to that object
            if (tryThrow)
                ThrowObject();

        }
    }

    public void CheckDropDistance()
    {
        //Checks distance between player and max hold distance and drops if required
        float playerDistanceToObject = Vector3.Distance(transform.position, heldObject.transform.position);
        if (playerDistanceToObject > (maxDistance + 2f))
            DropObject();
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
        heldObject = lookObject;
        //grab rigidbody
        heldObjectRB = heldObject.GetComponent<Rigidbody>();
        //Unfreeze position so can be moved around
        heldObjectRB.constraints = RigidbodyConstraints.None;
        //re-freeze rotation when held
        heldObjectRB.constraints = RigidbodyConstraints.FreezeRotation;
        //Temporarily disable collisions when picked up
        heldObjectRB.detectCollisions = false;

    }

    //Drop object
    public void DropObject()
    {
        heldObject.transform.parent = null;
        heldObjectRB.constraints = RigidbodyConstraints.None;
        currentDist = 0;

        //Reanable physics
        heldObjectRB.detectCollisions = true;
        heldObject = null;

    }

    //Throw the object 
    public void ThrowObject()
    {
        tryThrow = false;
        heldObjectRB.constraints = RigidbodyConstraints.None;
    
        //renable physics
        heldObjectRB.detectCollisions = true;
        heldObjectRB.AddForce(holdPosition.transform.forward * throwForce);
        //WaitUntilThrowing();
        heldObject = null;
    }

    //currently not used
    public IEnumerator WaitUntilThrowing()
    {
        yield return new WaitForSecondsRealtime(0.5f);
        heldObjectRB.AddForce(holdPosition.transform.forward * throwForce);

    }
}
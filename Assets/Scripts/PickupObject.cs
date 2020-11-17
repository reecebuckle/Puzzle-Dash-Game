using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class PickupObject : MonoBehaviour {
    Vector3 objectPos;
    float distance;
    public bool isHolding = false;
    public float throwForce = 100f;
    public bool isHoldable = true;
    public float pickUpDistance = 1f;
    public GameObject item;
    public GameObject parent;
    

    // Update is called once per frame
    void Update () {

        //Drop object if its moved too far away from player (e.g stuck behind a wall)
        distance = Vector3.Distance (item.transform.position, parent.transform.position);
        if (distance >= 3f) {
            isHolding = false;
        }
        
        if (isHolding == true) {
            //set velocities to zero
            //these two methods allow object to still collide whilst being held so will drop if it hit
            item.GetComponent<Rigidbody> ().velocity = Vector3.zero;
            item.GetComponent<Rigidbody> ().angularVelocity = Vector3.zero;
            item.transform.SetParent (parent.transform);

            //Throwing the object
            if (Input.GetMouseButtonDown (1)) {
                item.GetComponent<Rigidbody> ().AddForce (parent.transform.forward * throwForce);
                isHolding = false;
            }
        } else {
            //saves current location for object if you let go 
            objectPos = item.transform.position;
            //no longer attatched to our parent player
            item.transform.SetParent (null);
            //reanble gravity forces
            item.GetComponent<Rigidbody> ().useGravity = true;
            //update new position of object which is no longer joined the player
            item.transform.position = objectPos;
        }
    }

    //whilst holding left mouse down and object is in range, hold it 
    //can change this to E if desired
    void OnMouseDown () {
        if (distance <= pickUpDistance) {
            isHolding = true;
            item.GetComponent<Rigidbody> ().useGravity = false;
            item.GetComponent<Rigidbody> ().detectCollisions = true;
        }
    }

    //let go
    void OnMouseUp () {
        isHolding = false;
    }
}
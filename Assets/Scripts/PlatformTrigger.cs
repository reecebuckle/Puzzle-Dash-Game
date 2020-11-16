using System.Collections;
using System.Collections.Generic;
using UnityEngine;

   //Temp implementation to move the door without animating the door
public class PlatformTrigger : MonoBehaviour {
    [SerializeField]
    GameObject door;

    //not open by default
    bool isOpen = false;

    void OnTriggerEnter (Collider collider) {

        if (!isOpen) {
            isOpen = true;
            //move position of door up by 4
            door.transform.position += new Vector3 (0, 4, 0);
        }
    }
}
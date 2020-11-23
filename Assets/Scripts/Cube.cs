using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cube : MonoBehaviour
{
    //typically the cube
    private GameObject gameObject = null;
    // Start is called before the first frame update
    void Start()
    {
    }

    public void OnCollisionEnter(Collision collision)
     {
        if (collision.gameObject.tag == "theobjectToIgnore")
        {
            // Physics.IgnoreCollision(.collider, collider);
        }

    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cube : MonoBehaviour
{
    //typically the cube
    [Header("Holdable Object Properities")]

    public int worldLayerIndex;
    public int playerLayerIndex;
    // Start is called before the first frame update
    void Start()
    {
    }

    public void OnCollisionEnter(Collision collision)
     {
        if (collision.gameObject.layer == playerLayerIndex || collision.gameObject.layer == worldLayerIndex) 
        {
            //Physics.IgnoreCollision(.collider, collider);
        }

    }

}

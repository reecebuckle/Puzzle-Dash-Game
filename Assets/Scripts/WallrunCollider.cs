using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallrunCollider : MonoBehaviour
{
    public string direction;
    private void OnTriggerEnter(Collider other)
    {
        if (other.name != "Player")
            Debug.Log("Collision detected: " + direction);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Temp implementation to move the door without animating the door
public class PlatformTrigger : MonoBehaviour
{
    [Header("Platform Properities")]
    [SerializeField] GameObject door = null;
    //how long it takes for door to move, higher is longer
    [SerializeField] private float doorMovementTime = 50f;
    //Can toggle these in Unity
    private bool isOpen = false;

    private void OnCollisionEnter(Collision collision)
    {
        //Once fully open ignore checking
        if (!isOpen)
        {
            if (collision.collider.name == "RedBox")
            {
                isOpen = true;
                StartCoroutine(OpenDoor());
            }

        }
    }

    public IEnumerator OpenDoor()
    {
        float totalMovementTime = 50f;
        float currentMovementTime = 0f;
        Vector3 destination = door.transform.position - new Vector3(0, -21, 0);
        while (Vector3.Distance(transform.localPosition, destination) > 0)
        {
            door.transform.position = Vector3.Lerp(door.transform.position, (door.transform.position + new Vector3(0, -21, 0)), (currentMovementTime / totalMovementTime));
            currentMovementTime += Time.deltaTime;
            yield return null;
        }

    }
}
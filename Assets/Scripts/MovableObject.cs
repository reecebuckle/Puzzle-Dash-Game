using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovableObject : MonoBehaviour
{
    [Header("Movable Obstacle Properities")]
    public float pickupWaitTime = 0.2f;

    public float breakForce = 35f;
    [HideInInspector] public bool pickedUp = false;
    [HideInInspector] public MagnesisObject pickupParent;
    public AudioSource audioSource;

    private void Start()
    {
        // just incase PlayOnAwake is ticked
        audioSource.Stop();
    }


    private void OnCollisionEnter(Collision collision)
    {
        if (pickedUp)
        {
            if (collision.relativeVelocity.magnitude > breakForce)
            {
                pickupParent.DropObject();
            }

        }
        //if object was dropped by a (non null) parent and no longer picked up
        else if (!pickedUp && pickupParent != null)
        {
            //if object collides with the ground specfically
            if (collision.collider.tag == "Ground")
            {
                pickupParent.FreezeObject();
                //reset to null so it doesn't attempt this code multiple times once object is dropped and player collides with it
                pickupParent = null;
            }

        }
    }

    //this is used to prevent the connection from breaking when you just picked up the object as it sometimes fires a collision with the ground or whatever it is touching
    public IEnumerator PickUp()
    {
        PlaySoundTrack();
        yield return new WaitForSecondsRealtime(pickupWaitTime);
        pickedUp = true;


    }

    public void PlaySoundTrack()
    {
        if (!audioSource.isPlaying)
            audioSource.Play();
    }

    public void StopSoundTrack()
    {
        audioSource.Stop();

    }
}

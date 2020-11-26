using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Temp implementation to move the door without animating the door
public class PlatformTrigger : MonoBehaviour
{
    [Header("Platform Properities")]
    [SerializeField] GameObject door = null;
    //how long it takes for door to move, higher is longer
    [SerializeField] private float doorMovementTime = 300f;
    [SerializeField] private Renderer platformSurface = null;
    public AudioSource audioSource;
    //Can toggle these in Unity
    private bool isOpen = false;


    private void Start()
    {
        // just incase PlayOnAwake is ticked
        audioSource.Stop();
    }

    private void OnCollisionEnter(Collision collision)
    {
        //Once fully open ignore checking
        if (!isOpen)
        {
            if (collision.collider.name == "RedBox" || collision.collider.name == "SmallBox")
            {
                isOpen = true;
                platformSurface.material.color = Color.green;
                StartCoroutine(OpenDoor());
                PlaySoundTrack();

            }

        }
    }

    //DOOR OPENING SOUND (UNDER FREE USE) AVAILABLE HERE:
    //https://freesound.org/people/alexo400/sounds/543404/
    public void PlaySoundTrack()
    {
        if (!audioSource.isPlaying)
            audioSource.Play();
    }

    public IEnumerator OpenDoor()
    {
        float currentMovementTime = 0f;
        Vector3 destination = door.transform.position - new Vector3(0, -10, 0);
        while (Vector3.Distance(transform.localPosition, destination) > 0)
        {
            door.transform.position = Vector3.Lerp(door.transform.position, destination, (currentMovementTime / doorMovementTime));
            currentMovementTime += Time.deltaTime;
            yield return null;
        }

    }
}
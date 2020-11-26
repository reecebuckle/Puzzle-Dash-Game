using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RespawnCube : MonoBehaviour
{

    [Header("Movable Obstacle Properities")]
    [SerializeField] private Transform Spawnpoint;
    [SerializeField] private GameObject box;
    public AudioSource audioSource;

    private void Start()
    {
        // just incase PlayOnAwake is ticked
        audioSource.Stop();
        //make a copy of old rigidbody

    }

    public void OnMouseDown()
    {
        Debug.Log("Detected mouse click");
        PlaySoundTrack();

        //Rather than destroy game object, move it back to spawn point!!
        box.transform.position = Spawnpoint.transform.position;

    }

    //BUTTON Sound track that is played is a free to use song available at:
    //https://freesound.org/people/JarredGibb/sounds/219478/
    public void PlaySoundTrack()
    {
        if (!audioSource.isPlaying)
            audioSource.Play();
    }

}

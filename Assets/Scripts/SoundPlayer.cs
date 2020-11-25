using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundPlayer : MonoBehaviour
{
    public AudioSource audio;
    // taken from https://freesound.org/people/laurenmg95/sounds/386691/
    public AudioClip jump, running;

    // Update is called once per frame
    void Update()
    {

        // if (Input.GetKeyDown(KeyCode.Space))
        // {
        //     PlaySound(jump);
        // }
    }
    public void PlaySound(AudioClip soundClip)
    {
        audio.PlayOneShot(soundClip);
    }

}

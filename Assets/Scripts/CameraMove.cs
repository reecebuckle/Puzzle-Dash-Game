using UnityEngine;

public class CameraMove : MonoBehaviour
{

    public Transform player;

    void FixedUpdate()
    {
        transform.position = player.transform.position;
    }
}
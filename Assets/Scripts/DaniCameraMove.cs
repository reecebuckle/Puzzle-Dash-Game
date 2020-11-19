using UnityEngine;

public class DaniCameraMove : MonoBehaviour
{

    public Transform player;

    void FixedUpdate()
    {
        transform.position = player.transform.position;
    }
}
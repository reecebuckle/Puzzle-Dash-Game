using UnityEngine;

public class DaniCameraMove : MonoBehaviour
{

    public Transform player;

    void Update()
    {
        transform.position = player.transform.position;
    }
}
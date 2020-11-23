using UnityEngine;

public class CameraMove : MonoBehaviour
{

    public Transform player;
    public PlayerMove playerMoveScript;
    public Camera cam;
    private float _cameraRotationSpeed = 3.5f;

    private void Update()
    {
        transform.position = player.transform.position;
        if (playerMoveScript.wallRunning)
        {
            if (playerMoveScript.wallRunLeft)
            {
                Tilt(-15f);
            }
            else if (playerMoveScript.WallRunRight)
            {
                Tilt(15f);
            }
        }
        else
        {
            Tilt(0f);
        }
    }
    void FixedUpdate()
    {

    }
    void Tilt(float tiltAngle)
    {
        float angle = cam.transform.eulerAngles.z;
        float targetAngle = tiltAngle;
        angle = Mathf.LerpAngle(angle, targetAngle, _cameraRotationSpeed * Time.deltaTime);
        Vector3 tilt = new Vector3(cam.transform.eulerAngles.x, cam.transform.eulerAngles.y, angle);
        cam.transform.eulerAngles = tilt;
    }
}
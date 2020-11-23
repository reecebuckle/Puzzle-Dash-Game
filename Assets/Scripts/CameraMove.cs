using UnityEngine;

public class CameraMove : MonoBehaviour
{

    public Transform player;
    public PlayerMove playerMoveScript;
    public Camera camera;
    private float _cameraRotationSpeed = 3.5f;

    private void Update()
    {
        transform.position = player.transform.position;
        if (playerMoveScript.wallRunning)
        {
            if (playerMoveScript.wallRunLeft)
            {
                tiltRight();
            }
            else if (playerMoveScript.WallRunRight)
            {
                tiltLeft();
            }
        }
        else
        {
            removeTilt();
        }
    }
    void FixedUpdate()
    {

    }
    void tiltLeft()
    {
        float angle = camera.transform.eulerAngles.z;
        float targetAngle = 20f;
        angle = Mathf.LerpAngle(angle, targetAngle, _cameraRotationSpeed * Time.deltaTime);
        Vector3 tilt = new Vector3(camera.transform.eulerAngles.x, camera.transform.eulerAngles.y, angle);
        camera.transform.eulerAngles = tilt;
    }
    void tiltRight()
    {
        float angle = camera.transform.eulerAngles.z;
        float targetAngle = -20f;
        angle = Mathf.LerpAngle(angle, targetAngle, _cameraRotationSpeed * Time.deltaTime);
        Vector3 tilt = new Vector3(camera.transform.eulerAngles.x, camera.transform.eulerAngles.y, angle);
        camera.transform.eulerAngles = tilt;
    }
    void removeTilt()
    {
        float angle = camera.transform.eulerAngles.z;
        float targetAngle = 0f;
        angle = Mathf.LerpAngle(angle, targetAngle, _cameraRotationSpeed * Time.deltaTime);
        Vector3 tilt = new Vector3(camera.transform.eulerAngles.x, camera.transform.eulerAngles.y, angle);
        camera.transform.eulerAngles = tilt;
    }
}
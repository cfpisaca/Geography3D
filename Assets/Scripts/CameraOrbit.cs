using UnityEngine;

public class CameraOrbit : MonoBehaviour
{
    public Transform target; // Target to orbit around (the sphere)
    public float distance = 120.0f; // Distance from the target
    public float xSpeed = 5.0f;
    public float ySpeed = 5.0f;

    private float x = 0.0f;
    private float y = 0.0f;

    void Start()
    {
        Vector3 angles = transform.eulerAngles;
        x = angles.y;
        y = angles.x;

        // Ensure the camera is correctly positioned at start:
        UpdatePosition();
    }

    void LateUpdate()
    {
        if (target)
        {
            // Replace mouse input with VR controller joystick input
            // Left joystick horizontal for x-axis rotation
            x += OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick).x * xSpeed * Time.deltaTime;
            // Right joystick vertical for y-axis rotation
            y -= OVRInput.Get(OVRInput.Axis2D.SecondaryThumbstick).y * ySpeed * Time.deltaTime;

            // Clamp the vertical angle to avoid flipping at the poles
            y = ClampAngle(y, -89f, 89f);

            UpdatePosition();
        }
    }

    void UpdatePosition()
    {
        Quaternion rotation = Quaternion.Euler(y, x, 0);
        Vector3 negDistance = new Vector3(0.0f, 0.0f, -distance);
        Vector3 position = rotation * negDistance + target.position;

        transform.rotation = rotation;
        transform.position = position;
    }

    public static float ClampAngle(float angle, float min, float max)
    {
        if (angle < -360F)
            angle += 360F;
        if (angle > 360F)
            angle -= 360F;
        return Mathf.Clamp(angle, min, max);
    }
}

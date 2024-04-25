using UnityEngine;

public class CameraOrbit : MonoBehaviour
{
    public Transform target; 
    public float distance = 120.0f; 
    public float xSpeed = 15.0f;
    public float ySpeed = 15.0f;

    private float x = 0.0f;
    private float y = 0.0f;

    void Start()
    {
        Vector3 angles = transform.eulerAngles;
        x = angles.y;
        y = angles.x;

        UpdatePosition();
    }

    void LateUpdate()
    {
        if (target)
        {
            x += OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick).x * xSpeed * Time.deltaTime;
            y -= OVRInput.Get(OVRInput.Axis2D.SecondaryThumbstick).y * ySpeed * Time.deltaTime;

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

using UnityEngine;

public class RayHelper : MonoBehaviour
{
    public Camera mainCamera;
    public Transform rayOrigin;
    public float maxRayDistance = 100.0f;
    public LayerMask layerMask; // Ensure your country objects are on a specific layer and set this mask accordingly.
    public SphereMapper sphereMapper; // Reference to the SphereMapper component

    void Update()
    {
        Ray ray = new Ray(rayOrigin.position, rayOrigin.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, maxRayDistance, layerMask))
        {
            if (sphereMapper != null)
            {
                // Use the instance of SphereMapper to call HandleHover
                sphereMapper.HandleHover(hit.transform.gameObject);
            }
            else
            {
                Debug.LogError("SphereMapper reference not set in RayHelper.");
            }
        }
    }
}

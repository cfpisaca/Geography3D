using System.Collections.Generic;
using UnityEngine;

public class SphereMapper : MonoBehaviour
{
    public LoadGeoData loadGeoData;
    public float globeRadius = 10f;
    public Material lineMaterial;
    public float borderThickness = 0.3f; 
    public float innerBorderScale = 0.95f; 

    private void Start()
    {
        if (loadGeoData != null && loadGeoData.LoadGeodata != null)
        {
            RenderGeoJson(loadGeoData.LoadGeodata);
        }
        else
        {
            Debug.LogError("JSON data not loaded. Make sure the loadGeoData component is attached and has a JSON file assigned.");
        }
    }

    private void RenderGeoJson(GeoJson geoJson)
    {
        foreach (var feature in geoJson.features)
        {
            GameObject countryObj = new GameObject(feature.properties.ADMIN);
            countryObj.transform.SetParent(transform, false);

            MeshCollider meshCollider = countryObj.AddComponent<MeshCollider>();
            meshCollider.sharedMesh = GenerateCountryMesh(countryObj, feature.geometry);
            DrawCountryBorders(countryObj, feature.geometry, globeRadius, borderThickness, lineMaterial);
            DrawCountryBorders(countryObj, feature.geometry, globeRadius * innerBorderScale, borderThickness * 1.5f, lineMaterial, true);

            CountryInteract countryInteract = countryObj.AddComponent<CountryInteract>();
        }
    }

    private void DrawCountryBorders(GameObject countryObj, Geometry geometry, float radius, float thickness, Material material, bool inner = false)
    {
        if (geometry.type == "MultiPolygon" || geometry.type == "Polygon")
        {
            foreach (var polygon in geometry.coordinates)
            {
                GameObject polygonObj = new GameObject(inner ? "InnerBorder" : "OuterBorder");
                polygonObj.transform.SetParent(countryObj.transform, false);

                LineRenderer lineRenderer = polygonObj.AddComponent<LineRenderer>();
                lineRenderer.material = material;
                lineRenderer.startWidth = thickness;
                lineRenderer.endWidth = thickness;
                lineRenderer.useWorldSpace = false;

                List<Vector3> linePoints = new List<Vector3>();

                foreach (var linearRing in polygon)
                {
                    for (int i = 0; i < linearRing.Length - 1; i++)
                    {
                        linePoints.Add(LatLonToSphere(linearRing[i][1], linearRing[i][0], radius));
                    }

                    if (linearRing.Length > 0)
                    {
                        linePoints.Add(LatLonToSphere(linearRing[0][1], linearRing[0][0], radius));
                    }
                }

                lineRenderer.positionCount = linePoints.Count;
                lineRenderer.SetPositions(linePoints.ToArray());

                lineRenderer.material.color = Color.white;
            }
        }
        else
        {
            Debug.LogWarning("Unsupported geometry type for borders.");
        }
    }

    private Mesh GenerateCountryMesh(GameObject countryObj, Geometry geometry)
    {
        MeshFilter meshFilter = countryObj.AddComponent<MeshFilter>();

        List<Vector3> vertices = new List<Vector3>();
        List<int> triangles = new List<int>();

        if (geometry.type == "MultiPolygon" || geometry.type == "Polygon")
        {
            int startVertexIndex = 0;

            foreach (var polygon in geometry.coordinates)
            {
                for (int i = 0; i < polygon.Length; i++)
                {
                    var linearRing = polygon[i];
                    for (int j = 0; j < linearRing.Length; j++)
                    {
                        vertices.Add(LatLonToSphere(linearRing[j][1], linearRing[j][0], globeRadius));
                    }

                    if (i == 0) 
                    {
                        for (int j = 2; j < linearRing.Length; j++)
                        {
                            triangles.Add(startVertexIndex);
                            triangles.Add(startVertexIndex + j - 1);
                            triangles.Add(startVertexIndex + j);
                        }
                    }
                }

                startVertexIndex = vertices.Count; 
            }
        }

        Mesh mesh = new Mesh();
        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.RecalculateNormals(); 
        meshFilter.mesh = mesh;
        return mesh;
    }

    private Vector3 LatLonToSphere(float lat, float lon, float radius)
    {
        lat *= Mathf.Deg2Rad;
        lon *= Mathf.Deg2Rad;
        float x = radius * Mathf.Cos(lat) * Mathf.Cos(lon);
        float y = radius * Mathf.Sin(lat);
        float z = radius * Mathf.Cos(lat) * Mathf.Sin(lon);
        return new Vector3(x, y, z);
    }
}

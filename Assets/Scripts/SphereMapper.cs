using System.Collections.Generic;
using UnityEngine;

public class SphereMapper: MonoBehaviour
{
    public LoadGeoData loadGeoData;
    public float globeRadius = 10f;
    public Material lineMaterial;

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
            DrawCountryBorders(countryObj, feature.geometry);
        }
    }

    private void DrawCountryBorders(GameObject countryObj, Geometry geometry)
    {
        if (geometry.type == "MultiPolygon")
        {
            foreach (var polygon in geometry.coordinates)
            {
                GameObject polygonObj = new GameObject("Polygon");
                polygonObj.transform.SetParent(countryObj.transform, false);
                LineRenderer lineRenderer = polygonObj.AddComponent<LineRenderer>();
                lineRenderer.material = lineMaterial;
                lineRenderer.startWidth = 0.1f;
                lineRenderer.endWidth = 0.1f;
                lineRenderer.useWorldSpace = false;

                List<Vector3> linePoints = new List<Vector3>();

                foreach (var linearRing in polygon)
                {
                    for (int i = 0; i < linearRing.Length - 1; i++)
                    {
                        linePoints.Add(LatLonToSphere(linearRing[i][1], linearRing[i][0], globeRadius));
                    }
                    
                    if (linearRing.Length > 0)
                    {
                        linePoints.Add(LatLonToSphere(linearRing[0][1], linearRing[0][0], globeRadius));
                    }
                }

                lineRenderer.positionCount = linePoints.Count;
                lineRenderer.SetPositions(linePoints.ToArray());
            }
        }
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

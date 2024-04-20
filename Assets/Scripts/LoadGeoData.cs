using UnityEngine;
using Newtonsoft.Json;
using System;

[Serializable]
public class GeoJson
{
    public string type;
    public Feature[] features;
}

[Serializable]
public class Feature
{
    public string type;
    public Properties properties;
    public Geometry geometry;
}

[Serializable]
public class Properties
{
    public string ADMIN;
    public string ISO_A3;
    public string ISO_A2;
}

[Serializable]
public class Geometry
{
    public string type;
    public float[][][][] coordinates; 
}

public class LoadGeoData : MonoBehaviour
{
    public TextAsset geoJsonFile;
    public GeoJson LoadGeodata { get; private set; }

    void Awake()
    {
        if (geoJsonFile != null)
        {
            LoadGeodata = JsonConvert.DeserializeObject<GeoJson>(geoJsonFile.text);
            Debug.Log("GeoJSON data loaded successfully using Newtonsoft.Json.");
        }
        else
        {
            Debug.LogError("GeoJson file is not assigned.");
        }
    }
}

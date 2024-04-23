using System.Collections.Generic;
using UnityEngine;

public class SphereMapper : MonoBehaviour
{
    public LoadGeoData loadGeoData;
    public float globeRadius = 10f;
    public Material lineMaterial;
    public Material countryMaterial;

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
            FillPolygon(countryObj.transform, feature.geometry, RandomColor()); // Fill the country with a random color
            CreateMeshCollider(countryObj);
        }
    }

    private void FillPolygon(Transform parent, Geometry geometry, Color color)
    {
        if (geometry == null)
        {
            Debug.LogError("Invalid geometry for FillPolygon.");
            return;
        }

        if (geometry.type == "Polygon")
        {
            FillSinglePolygon(parent, geometry.coordinates[0], color);
        }
        else if (geometry.type == "MultiPolygon")
        {
            foreach (var polygon in geometry.coordinates)
            {
                FillSinglePolygon(parent, polygon, color);
            }
        }
        else
        {
            Debug.LogWarning("Unsupported geometry type for filling: " + geometry.type);
        }
    }

    private void FillSinglePolygon(Transform parent, float[][][] polygon, Color color)
    {
        foreach (var linearRing in polygon)
        {
            List<Vector3> vertices = new List<Vector3>();

            foreach (var vertex in linearRing)
            {
                Vector3 vertexPos = LatLonToSphere((float)vertex[1], (float)vertex[0], globeRadius);
                vertices.Add(vertexPos);
            }

            GameObject polygonObj = new GameObject("FilledPolygon");
            polygonObj.transform.SetParent(parent, false);

            MeshRenderer meshRenderer = polygonObj.AddComponent<MeshRenderer>();
            meshRenderer.material = new Material(countryMaterial);
            meshRenderer.material.color = color;

            MeshFilter meshFilter = polygonObj.AddComponent<MeshFilter>();
            meshFilter.mesh = CreateFilledMesh(vertices);
        }
    }

    private Mesh CreateFilledMesh(List<Vector3> vertices)
    {
        List<Vector3> filledVertices = new List<Vector3>();
        List<int> triangles = new List<int>();

        filledVertices.AddRange(vertices);
        Vector3 center = Vector3.zero;
        foreach (Vector3 vertex in vertices)
        {
            center += vertex;
        }
        center /= vertices.Count;
        filledVertices.Add(center);

        for (int i = 0; i < vertices.Count; i++)
        {
            triangles.Add(filledVertices.Count - 1);
            triangles.Add(i);
            triangles.Add((i + 1) % vertices.Count);
        }

        Mesh mesh = new Mesh();
        mesh.vertices = filledVertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.RecalculateNormals();

        return mesh;
    }

    private void CreateMeshCollider(GameObject countryObj)
    {
        MeshFilter[] meshFilters = countryObj.GetComponentsInChildren<MeshFilter>();
        List<CombineInstance> combineInstances = new List<CombineInstance>();

        foreach (MeshFilter meshFilter in meshFilters)
        {
            CombineInstance combineInstance = new CombineInstance();
            combineInstance.mesh = meshFilter.sharedMesh;
            combineInstance.transform = meshFilter.transform.localToWorldMatrix;
            combineInstances.Add(combineInstance);
        }

        Mesh combinedMesh = new Mesh();
        combinedMesh.CombineMeshes(combineInstances.ToArray(), true);

        MeshCollider meshCollider = countryObj.AddComponent<MeshCollider>();
        meshCollider.sharedMesh = combinedMesh;
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

    private Color RandomColor()
    {
        return new Color(Random.value, Random.value, Random.value);
    }
}


public class Triangulator
{
    private List<Vector2> points = new List<Vector2>();

    public Triangulator(Vector2[] points)
    {
        this.points = new List<Vector2>(points);
    }

    public int[] Triangulate()
    {
        List<int> indices = new List<int>();

        int n = points.Count;
        if (n < 3)
            return indices.ToArray();

        int[] V = new int[n];
        if (Area() > 0)
        {
            for (int v = 0; v < n; v++)
                V[v] = v;
        }
        else
        {
            for (int v = 0; v < n; v++)
                V[v] = (n - 1) - v;
        }

        int nv = n;
        int count = 2 * nv;
        for (int m = 0, v = nv - 1; nv > 2;)
        {
            if ((count--) <= 0)
                return indices.ToArray();

            int u = v;
            if (nv <= u)
                u = 0;
            v = u + 1;
            if (nv <= v)
                v = 0;
            int w = v + 1;
            if (nv <= w)
                w = 0;

            if (Snip(u, v, w, nv, V))
            {
                int a, b, c, s, t;
                a = V[u];
                b = V[v];
                c = V[w];
                indices.Add(a);
                indices.Add(b);
                indices.Add(c);
                m++;
                for (s = v, t = v + 1; t < nv; s++, t++)
                    V[s] = V[t];
                nv--;
                count = 2 * nv;
            }
        }

        indices.Reverse();
        return indices.ToArray();
    }

    private float Area()
    {
        int n = points.Count;
        float A = 0.0f;
        for (int p = n - 1, q = 0; q < n; p = q++)
        {
            Vector2 pval = points[p];
            Vector2 qval = points[q];
            A += pval.x * qval.y - qval.x * pval.y;
        }

        return (A * 0.5f);
    }

    private bool Snip(int u, int v, int w, int n, int[] V)
    {
        int p;
        Vector2 A = points[V[u]];
        Vector2 B = points[V[v]];
        Vector2 C = points[V[w]];
        if (Mathf.Epsilon > (((B.x - A.x) * (C.y - A.y)) - ((B.y - A.y) * (C.x - A.x))))
            return false;
        for (p = 0; p < n; p++)
        {
            if ((p == u) || (p == v) || (p == w))
                continue;
            Vector2 P = points[V[p]];
            if (InsideTriangle(A, B, C, P))
                return false;
        }
        return true;
    }

    private bool InsideTriangle(Vector2 A, Vector2 B, Vector2 C, Vector2 P)
    {
        float ax, ay, bx, by, cx, cy, apx, apy, bpx, bpy, cpx, cpy;
        float cCROSSap, bCROSScp, aCROSSbp;

        ax = C.x - B.x; ay = C.y - B.y;
        bx = A.x - C.x; by = A.y - C.y;
        cx = B.x - A.x; cy = B.y - A.y;
        apx = P.x - A.x; apy = P.y - A.y;
        bpx = P.x - B.x; bpy = P.y - B.y;
        cpx = P.x - C.x; cpy = P.y - C.y;

        aCROSSbp = ax * bpy - ay * bpx;
        cCROSSap = cx * apy - cy * apx;
        bCROSScp = bx * cpy - by * cpx;

        return ((aCROSSbp >= 0.0f) && (bCROSScp >= 0.0f) && (cCROSSap >= 0.0f));
    }
}


using UnityEngine;
using System.Collections.Generic;

public class SphereMeshMapper : MonoBehaviour
{
    public LoadGeoData loadGeoData;
    public float globeRadius = 10f;
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

            MeshFilter meshFilter = countryObj.AddComponent<MeshFilter>();
            MeshRenderer meshRenderer = countryObj.AddComponent<MeshRenderer>();
            meshRenderer.material = countryMaterial;

            meshRenderer.material.color = GetColorFromName(feature.properties.ADMIN);

            Mesh countryMesh = CreateCountryMesh(feature.geometry);
            meshFilter.mesh = countryMesh;
        }
    }

    private Color GetColorFromName(string name)
    {
        Random.InitState(name.GetHashCode());
        return new Color(Random.value, Random.value, Random.value);
    }

    private Mesh CreateCountryMesh(Geometry geometry)
    {
        Mesh mesh = new Mesh();
        List<Vector3> vertices = new List<Vector3>();
        List<int> triangles = new List<int>();

        if (geometry.type == "MultiPolygon" || geometry.type == "Polygon")
        {
            foreach (var polygon in geometry.coordinates)
            {
                foreach (var linearRing in polygon)
                {
                    List<Vector3> polygonVertices = new List<Vector3>();

                    foreach (var coord in linearRing)
                    {
                        Vector3 point = LatLonToSphere(coord[1], coord[0], globeRadius);
                        if (!vertices.Contains(point))
                        {
                            vertices.Add(point);
                        }
                        polygonVertices.Add(point);
                    }

                    if (polygonVertices.Count >= 3)
                    {
                        Triangulator triangulator = new Triangulator(polygonVertices);
                        int[] localTriangles = triangulator.Triangulate();
                        foreach (int localIndex in localTriangles)
                        {
                            triangles.Add(vertices.IndexOf(polygonVertices[localIndex]));
                        }
                    }
                }
            }

            // Calculate normals based on vertex positions relative to the center of the sphere
            Vector3[] normals = new Vector3[vertices.Count];
            Vector3 center = Vector3.zero;
            for (int i = 0; i < vertices.Count; i++)
            {
                normals[i] = (vertices[i] - center).normalized;
            }

            mesh.vertices = vertices.ToArray();
            mesh.normals = normals;
            mesh.triangles = triangles.ToArray();
        }
        else
        {
            Debug.LogWarning("Unsupported geometry type for mesh.");
            return null;
        }

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

public class Triangulator
{
    private List<Vector3> m_points;

    public Triangulator(List<Vector3> points)
    {
        m_points = points;
    }

    public int[] Triangulate()
    {
        List<int> indices = new List<int>();

        int n = m_points.Count;
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
        int n = m_points.Count;
        float A = 0.0f;
        for (int p = n - 1, q = 0; q < n; p = q++)
        {
            Vector3 pval = m_points[p];
            Vector3 qval = m_points[q];
            A += pval.x * qval.z - qval.x * pval.z;
        }
        return A * 0.5f;
    }

    private bool Snip(int u, int v, int w, int n, int[] V)
    {
        int p;
        Vector3 A = m_points[V[u]];
        Vector3 B = m_points[V[v]];
        Vector3 C = m_points[V[w]];
        if (Mathf.Epsilon > (((B.x - A.x) * (C.z - A.z)) - ((B.z - A.z) * (C.x - A.x))))
            return false;
        for (p = 0; p < n; p++)
        {
            if ((p == u) || (p == v) || (p == w))
                continue;
            Vector3 P = m_points[V[p]];
            if (InsideTriangle(A, B, C, P))
                return false;
        }
        return true;
    }

    private bool InsideTriangle(Vector3 A, Vector3 B, Vector3 C, Vector3 P)
    {
        float ax, az, bx, bz, cx, cz, apx, apz, bpx, bpz, cpx, cpz;
        ax = C.x - B.x; az = C.z - B.z;
        bx = A.x - C.x; bz = A.z - C.z;
        cx = B.x - A.x; cz = B.z - A.z;
        apx = P.x - A.x; apz = P.z - A.z;
        bpx = P.x - B.x; bpz = P.z - B.z;
        cpx = P.x - C.x; cpz = P.z - C.z;

        float aCROSSbp = ax * bpz - az * bpx;
        float cCROSSap = cx * apz - cz * apx;
        float bCROSScp = bx * cpz - bz * cpx;

        return ((aCROSSbp >= 0.0f) && (bCROSScp >= 0.0f) && (cCROSSap >= 0.0f));
    }
}

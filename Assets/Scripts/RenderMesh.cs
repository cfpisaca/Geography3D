using UnityEngine;

public class RenderMesh : MonoBehaviour
{
    private Mesh mesh;

    void Start() {
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;
        CreateMesh();
    }

    void CreateMesh() {
        MeshData[] faces = GenerateFaces(25); // RESOLUTION
        CombineMeshData(faces);
    }

    MeshData CreateFace(Vector3 normal, int resolution) {
        Vector3 axisA = new Vector3(normal.y, normal.z, normal.x);
        Vector3 axisB = Vector3.Cross(normal, axisA);
        Vector3[] vertices = new Vector3[resolution * resolution];
        int[] triangles = new int[(resolution - 1) * (resolution - 1) * 6];
        int triIndex = 0;

        for (int y = 0; y < resolution; y++) {
            for (int x = 0; x < resolution; x++) {
                int vertexIndex = x + y * resolution;
                Vector2 t = new Vector2(x, y) / (resolution - 1);
                Vector3 point = normal + axisA * (2 * t.x - 1) + axisB * (2 * t.y - 1);
                vertices[vertexIndex] = PointOnCubeToPointOnSphere(point);

                if (x != resolution - 1 && y != resolution - 1) {
                    triangles[triIndex] = vertexIndex;
                    triangles[triIndex + 1] = vertexIndex + resolution + 1;
                    triangles[triIndex + 2] = vertexIndex + resolution;

                    triangles[triIndex + 3] = vertexIndex;
                    triangles[triIndex + 4] = vertexIndex + 1;
                    triangles[triIndex + 5] = vertexIndex + resolution + 1;
                    triIndex += 6;
                }
            }
        }

        return new MeshData(vertices, triangles);
    }

    MeshData[] GenerateFaces(int resolution) {
        MeshData[] allMeshData = new MeshData[6];
        Vector3[] faceNormals = {
            Vector3.up,
            Vector3.down,
            Vector3.left,
            Vector3.right,
            Vector3.forward,
            Vector3.back
        };

        for (int i = 0; i < faceNormals.Length; i++) {
            allMeshData[i] = CreateFace(faceNormals[i], resolution);
        }

        return allMeshData;
    }

    public static Vector3 PointOnCubeToPointOnSphere(Vector3 point) {
        float x2 = point.x * point.x;
        float y2 = point.y * point.y;
        float z2 = point.z * point.z;
        float x = point.x * Mathf.Sqrt(1 - y2 / 2 - z2 / 2 + y2 * z2 / 3);
        float y = point.y * Mathf.Sqrt(1 - x2 / 2 - z2 / 2 + x2 * z2 / 3);
        float z = point.z * Mathf.Sqrt(1 - x2 / 2 - y2 / 2 + x2 * y2 / 3);

        return new Vector3(x, y, z);
    }

    void CombineMeshData(MeshData[] meshData) {
        CombineInstance[] combine = new CombineInstance[meshData.Length];
        for (int i = 0; i < meshData.Length; i++) {
            Mesh mesh = new Mesh();
            mesh.vertices = meshData[i].vertices;
            mesh.triangles = meshData[i].triangles;
            mesh.RecalculateNormals(); 

            combine[i].mesh = mesh;
            combine[i].transform = Matrix4x4.identity;
        }

        this.mesh.CombineMeshes(combine);
        this.mesh.RecalculateBounds();
    }
}

public struct MeshData {
    public Vector3[] vertices;
    public int[] triangles;

    public MeshData(Vector3[] vertices, int[] triangles)
    {
        this.vertices = vertices;
        this.triangles = triangles;
    }
}

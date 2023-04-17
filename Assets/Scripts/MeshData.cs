using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshData {
    public List<Vector3> Vertices;
    public List<int> Triangles;
    public List<Vector2> UVs;

    public List<Vector3> ColVertices;
    public List<int> ColTriangles;

    public MeshData() {
        Vertices = new List<Vector3>();
        Triangles = new List<int>();
        UVs = new List<Vector2>();
        ColVertices = new List<Vector3>();
        ColTriangles = new List<int>();
    }

    public void AddQuadWithUVs(Vector3[] quadPoints, Vector2[] uvs, bool reverse) {
        if (uvs.Length != 4) {
            Debug.LogError("Incorrect number of uvs for quad. Received: " + uvs + "uvs");
        }
        AddQuad(quadPoints, reverse);
        UVs.AddRange(uvs);
    }

    public void AddQuad(Vector3[] quadPoints, bool revese) {
        if (quadPoints.Length != 4) {
            Debug.LogError("Tried adding quad to mesh data, " +
                           "but quadPoints did not have 4 points.");
            return;
        }
        foreach (Vector3 point in quadPoints) {
            Vertices.Add(point);
        }
        AddTrianglesForQuad(revese);
    }

    public void AddTrianglesForQuad(bool reverse = false) {
        if (!reverse) {
            Triangles.Add(Vertices.Count - 4);
            Triangles.Add(Vertices.Count - 3);
            Triangles.Add(Vertices.Count - 2);

            Triangles.Add(Vertices.Count - 4);
            Triangles.Add(Vertices.Count - 2);
            Triangles.Add(Vertices.Count - 1);
        } else {
            Triangles.Add(Vertices.Count - 1);
            Triangles.Add(Vertices.Count - 2);
            Triangles.Add(Vertices.Count - 4);

            Triangles.Add(Vertices.Count - 2);
            Triangles.Add(Vertices.Count - 3);
            Triangles.Add(Vertices.Count - 4);
        }
    }
}

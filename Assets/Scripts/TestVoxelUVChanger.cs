using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestVoxelUVChanger : MonoBehaviour {

    private MeshFilter _meshFilter;

    private void Start() {
        _meshFilter = GetComponent<MeshFilter>();
        MeshData meshData = new MeshData();
        for (int i = 0; i < 6; i++) {
            AddFaceToMeshData(meshData, (Direction)i, new Voxel3(0, 0, 0));
        }

        _meshFilter.mesh.Clear();
        _meshFilter.mesh.vertices = meshData.Vertices.ToArray();
        _meshFilter.mesh.triangles = meshData.Triangles.ToArray();
        _meshFilter.mesh.uv = meshData.UVs.ToArray();
        _meshFilter.mesh.RecalculateNormals();
    }

    private void AddFaceToMeshData(MeshData meshData, Direction drawDirection, Voxel3 pos) {
        const int verticesPerFace = 4;
        Vector3[] newFaceVertices = new Vector3[verticesPerFace];
        Vector3[] faceVertexArray = Voxel.FaceVertexData[(int)drawDirection];
        for (int i = 0; i < faceVertexArray.Length; i++) {
            Vector3 vertex = faceVertexArray[i];
            Vector3 modifiedVertex = vertex * VoxelWorld.VoxelSize;

            modifiedVertex.x += pos.X * VoxelWorld.VoxelSize;
            modifiedVertex.y += pos.Y * VoxelWorld.VoxelSize;
            modifiedVertex.z += pos.Z * VoxelWorld.VoxelSize;

            newFaceVertices[i] = modifiedVertex;
        }

        meshData.AddQuadWithUVs(newFaceVertices, GetFaceUVs(drawDirection), false);
    }

    public virtual Vector2 GetTextureTilePosition(Direction direction) {
        return new Vector2(0, 2);
    }

    public virtual Vector2[] GetFaceUVs(Direction direction) {
        Vector2[] uvs = new Vector2[4];
        Vector2 tilePos = GetTextureTilePosition(direction);

        const float tileSize = 0.25f;
        // Bottom right
        uvs[0] = new Vector2(tileSize * tilePos.x + tileSize, tileSize * tilePos.y);
        // Top Right
        uvs[1] = new Vector2(tileSize * tilePos.x + tileSize, tileSize * tilePos.y + tileSize);
        // Top Left
        uvs[2] = new Vector2(tileSize * tilePos.x, tileSize * tilePos.y + tileSize);
        // Bottom Left
        uvs[3] = new Vector2(tileSize * tilePos.x, tileSize * tilePos.y);

        const float antiBleedOffset = 0.0025f;
        Vector2[] antiTextureBleedOffsets = {
            new Vector2(-antiBleedOffset, antiBleedOffset),
            new Vector2(-antiBleedOffset, -antiBleedOffset),
            new Vector2(antiBleedOffset, -antiBleedOffset),
            new Vector2(antiBleedOffset, antiBleedOffset)
        };

        for (int i = 0; i < uvs.Length; i++) {
            uvs[i] += antiTextureBleedOffsets[i];
        }

        return uvs;
    }
}

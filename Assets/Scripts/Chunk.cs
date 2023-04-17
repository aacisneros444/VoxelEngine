using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshCollider))]
public class Chunk : MonoBehaviour {
    private MeshFilter _meshFilter;
    private MeshCollider _meshCollider;

    private Voxel[,,] _voxels;

    [HideInInspector]
    public VoxelWorld World;
    [HideInInspector]
    public Voxel3 ChunkCoordinates;

    public void Init() {
        _meshFilter = GetComponent<MeshFilter>();
        _meshCollider = GetComponent<MeshCollider>();
        _voxels = new Voxel[VoxelWorld.ChunkVoxelSize, VoxelWorld.ChunkVoxelSize, VoxelWorld.ChunkVoxelSize];

        for (int x = 0; x < VoxelWorld.ChunkVoxelSize; x++) {
            for (int y = 0; y < VoxelWorld.ChunkVoxelSize; y++) {
                for (int z = 0; z < VoxelWorld.ChunkVoxelSize; z++) {
                    _voxels[x, y, z] = new RoomVoxel();
                }
            }
        }
    }

    /// <summary>
    /// Given voxel world coordinates, convert them to chunk local voxel coordinates.
    /// </summary>
    /// <param name="voxelPosition">The world voxel coordinates.</param>
    /// <returns>Voxel coordinates in the local chunk.</returns>
    public Voxel3 GetLocalVoxelPositionInChunk(Voxel3 voxelPosition) {
        int voxelLocalX = voxelPosition.X - ChunkCoordinates.X * VoxelWorld.ChunkVoxelSize;
        int voxelLocaly = voxelPosition.Y - ChunkCoordinates.Y * VoxelWorld.ChunkVoxelSize;
        int voxelLocalZ = voxelPosition.Z - ChunkCoordinates.Z * VoxelWorld.ChunkVoxelSize;
        return new Voxel3(voxelLocalX, voxelLocaly, voxelLocalZ);
    }

    // Get a voxel from the chunk. Expects chunk local coordinates.
    public Voxel GetVoxel(Voxel3 chunkLocalCoordinates) {
        if (InRange(chunkLocalCoordinates)) {
            return _voxels[chunkLocalCoordinates.X, chunkLocalCoordinates.Y, chunkLocalCoordinates.Z];
        }
        // Did not find requested voxel in chunk. Convert given voxel coordinates to world voxel coordinates
        // and search world.
        return World.GetVoxel(new Voxel3(ChunkCoordinates.X * VoxelWorld.ChunkVoxelSize + chunkLocalCoordinates.X,
                                         ChunkCoordinates.Y * VoxelWorld.ChunkVoxelSize + chunkLocalCoordinates.Y,
                                         ChunkCoordinates.Z * VoxelWorld.ChunkVoxelSize + chunkLocalCoordinates.Z));
    }

    /// <summary>
    /// Evaluate whether given local chunk voxel coordinates are in range
    /// for this chunk.
    /// </summary>
    /// <param name="chunkLocalCoordinates">The given local chunk voxel coordinates.</param>
    /// <returns>True if in range, false otherwise.</returns>
    private bool InRange(Voxel3 chunkLocalCoordinates) {
        return (chunkLocalCoordinates.X >= 0 &&
                chunkLocalCoordinates.X < VoxelWorld.ChunkVoxelSize) &&
               (chunkLocalCoordinates.Y >= 0 &&
                chunkLocalCoordinates.Y < VoxelWorld.ChunkVoxelSize) &&
               (chunkLocalCoordinates.Z >= 0 &&
                chunkLocalCoordinates.Z < VoxelWorld.ChunkVoxelSize);
    }

    // Updates the chunk mesh based on contents of this chunk's voxel array.
    public void UpdateChunkMesh() {
        MeshData meshData = new MeshData();

        for (int x = 0; x < VoxelWorld.ChunkVoxelSize; x++) {
            for (int y = 0; y < VoxelWorld.ChunkVoxelSize; y++) {
                for (int z = 0; z < VoxelWorld.ChunkVoxelSize; z++) {
                    _voxels[x, y, z].AddSelfToChunkMeshData(meshData, this, new Voxel3(x, y, z));
                }
            }
        }

        _meshFilter.mesh.Clear();
        _meshFilter.mesh.vertices = meshData.Vertices.ToArray();
        _meshFilter.mesh.triangles = meshData.Triangles.ToArray();
        _meshFilter.mesh.uv = meshData.UVs.ToArray();
        _meshFilter.mesh.RecalculateNormals();

        _meshCollider.sharedMesh = null;
        Mesh mesh = new Mesh();
        mesh.vertices = meshData.Vertices.ToArray();
        mesh.triangles = meshData.Triangles.ToArray();
        mesh.RecalculateNormals();

        _meshCollider.sharedMesh = mesh;
    }
}

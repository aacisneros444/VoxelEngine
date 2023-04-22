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

    public void Init(Voxel[,,] voxels = null) {
        _meshFilter = GetComponent<MeshFilter>();
        _meshCollider = GetComponent<MeshCollider>();

        if (voxels == null) {
            _voxels = new Voxel[VoxelWorld.ChunkVoxelSize, VoxelWorld.ChunkVoxelSize, VoxelWorld.ChunkVoxelSize];
            for (int x = 0; x < VoxelWorld.ChunkVoxelSize; x++) {
                for (int y = 0; y < VoxelWorld.ChunkVoxelSize; y++) {
                    for (int z = 0; z < VoxelWorld.ChunkVoxelSize; z++) {
                        _voxels[x, y, z] = new AirVoxel();
                    }
                }
            }
        } else {
            _voxels = voxels;
        }

        // _voxels[2, 5, 6] = new RoomVoxel();
        // _voxels[2, 5, 6] = new RoomVoxel();
        // _voxels[2, 5, 6] = new RoomVoxel();

        // _voxels[3, 5, 6] = new RoomVoxel();
        // _voxels[3, 5, 6] = new RoomVoxel();
        // _voxels[3, 5, 6] = new RoomVoxel();

        // _voxels[4, 5, 6] = new RoomVoxel();
        // _voxels[4, 5, 6] = new RoomVoxel();
        // _voxels[4, 5, 6] = new RoomVoxel();

        // _voxels[2, 5, 5] = new RoomVoxel();
        // _voxels[2, 5, 5] = new RoomVoxel();
        // _voxels[2, 5, 5] = new RoomVoxel();

        // _voxels[3, 5, 5] = new RoomVoxel();
        // _voxels[3, 5, 5] = new RoomVoxel();
        // _voxels[3, 5, 5] = new RoomVoxel();

        // _voxels[4, 5, 5] = new RoomVoxel();
        // _voxels[4, 5, 5] = new RoomVoxel();
        // _voxels[4, 5, 5] = new RoomVoxel();

        // _voxels[3, 5, 7] = new RoomVoxel();
    }

    /// <summary>
    /// Given voxel world coordinates, convert them to chunk local voxel coordinates.
    /// </summary>
    /// <param name="voxelPosition">The world voxel coordinates.</param>
    /// <returns>Voxel coordinates in the local chunk.</returns>
    public Voxel3 GetLocalVoxelPositionInChunk(Voxel3 voxelPosition) {
        return voxelPosition - (ChunkCoordinates * VoxelWorld.ChunkVoxelSize);
    }

    private Voxel3 GetWorldVoxelPositionFromLocalCoordinates(Voxel3 chunkLocalCoordinates) {
        return (ChunkCoordinates * VoxelWorld.ChunkVoxelSize) + chunkLocalCoordinates;
    }

    // Get a voxel from the chunk. Expects chunk local coordinates.
    public Voxel GetVoxel(Voxel3 chunkLocalCoordinates) {
        if (InRange(chunkLocalCoordinates)) {
            return _voxels[chunkLocalCoordinates.X, chunkLocalCoordinates.Y, chunkLocalCoordinates.Z];
        }
        // Did not find requested voxel in chunk. Convert given voxel coordinates to world voxel coordinates
        // and search world.
        return World.GetVoxel(GetWorldVoxelPositionFromLocalCoordinates(chunkLocalCoordinates));
    }

    public void SetVoxel(Voxel3 chunkLocalCoordinates, Voxel voxel) {
        if (InRange(chunkLocalCoordinates)) {
            _voxels[chunkLocalCoordinates.X, chunkLocalCoordinates.Y, chunkLocalCoordinates.Z] = voxel;
        } else {
            World.SetVoxel(GetWorldVoxelPositionFromLocalCoordinates(chunkLocalCoordinates), voxel);
        }
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

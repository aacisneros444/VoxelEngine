using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VoxelWorld : MonoBehaviour {
    [SerializeField] private GameObject _chunkPrefab;

    [Tooltip("Then length of a chunk side in voxels. For example, for a ChunkSize of 8, chunks will have 8x8x8 voxels.")]
    public static int ChunkVoxelSize = 8;
    [Tooltip("The length of voxel side in world units.")]
    public static int VoxelSize = 4;
    [Tooltip("The length of a chunk side in world units.")]
    public static readonly int ChunkSize = ChunkVoxelSize * VoxelSize;

    private Dictionary<Voxel3, Chunk> _chunks;

    private void Start() {
        _chunks = new Dictionary<Voxel3, Chunk>();

        for (int x = 0; x < 2; x++) {
            for (int y = 0; y < 1; y++) {
                for (int z = 0; z < 1; z++) {
                    CreateChunk(new Voxel3(x, y, z));
                }
            }
        }
        // CreateChunk(new Voxel3(0, 0, 0));
        // CreateChunk(new Voxel3(1, 0, 0));
        // CreateChunk(new Voxel3(2, 0, 0));

        foreach (KeyValuePair<Voxel3, Chunk> kv in _chunks) {
            kv.Value.Init();
        }
        foreach (KeyValuePair<Voxel3, Chunk> kv in _chunks) {
            kv.Value.UpdateChunkMesh();
        }
    }

    private void CreateChunk(Voxel3 chunkCoordinates) {
        Vector3 worldPosition = new Vector3(chunkCoordinates.X * ChunkSize,
                                            chunkCoordinates.Y * ChunkSize,
                                            chunkCoordinates.Z * ChunkSize);

        GameObject chunkInstance = Instantiate(_chunkPrefab, worldPosition, Quaternion.identity);

        Chunk newChunk = chunkInstance.GetComponent<Chunk>();
        newChunk.ChunkCoordinates = chunkCoordinates;
        newChunk.World = this;

        _chunks.Add(chunkCoordinates, newChunk);
    }

    /// <summary>
    /// Given voxel coordinates, get the appropriate chunk.
    /// </summary>
    /// <param name="voxelCoordinates">The given voxel coordinates.</param>
    /// <returns>The chunk that should own the voxel for the given voxel coordinates, 
    /// if the chunk exists. Null if it does not.</returns>
    public Chunk GetChunk(Voxel3 voxelCoordinates) {
        Voxel3 chunkCoordinates = new Voxel3(Mathf.FloorToInt(voxelCoordinates.X / (float)ChunkVoxelSize),
                                             Mathf.FloorToInt(voxelCoordinates.Y / (float)ChunkVoxelSize),
                                             Mathf.FloorToInt(voxelCoordinates.Z / (float)ChunkVoxelSize));

        Chunk chunk = null;
        _chunks.TryGetValue(chunkCoordinates, out chunk);
        return chunk;
    }

    /// <summary>
    /// Given voxel coordinates, get the voxel at those coordinates.
    /// </summary>
    /// <param name="voxelPosition">The given voxel coordinates.</param>
    /// <returns>The voxel at the given coordinates. A new AirVoxel() if it does not exist.</returns>
    public Voxel GetVoxel(Voxel3 voxelCoordinates) {
        Chunk containingChunk = GetChunk(voxelCoordinates);

        if (containingChunk != null) {
            Voxel3 localVoxelPositionInChunk = containingChunk.GetLocalVoxelPositionInChunk(voxelCoordinates);
            Voxel voxel = containingChunk.GetVoxel(localVoxelPositionInChunk);
            return voxel;
        }
        return new AirVoxel();
    }

    public Voxel GetVoxel(Vector3 worldPos) {
        return GetVoxel(new Voxel3((int)worldPos.x, (int)worldPos.y, (int)worldPos.z));
    }
}

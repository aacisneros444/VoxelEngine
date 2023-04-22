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
    private Dictionary<Voxel3, Chunk> _chunksToUpdate;

    private void Start() {
        _chunks = new Dictionary<Voxel3, Chunk>();
        _chunksToUpdate = new Dictionary<Voxel3, Chunk>();

        // for (int x = -2; x < 2; x++) {
        //     for (int y = -2; y < 2; y++) {
        //         for (int z = -2; z < 2; z++) {
        //             CreateChunk(new Voxel3(x, y, z));
        //         }
        //     }
        // }

        CreateChunk(new Voxel3(0, 0, 0));
    }

    private void Update() {
        foreach (Chunk chunk in _chunksToUpdate.Values) {
            chunk.UpdateChunkMesh();
            // Debug.Log("Updating " + chunk.ChunkCoordinates);
        }
        _chunksToUpdate.Clear();
    }

    private Chunk CreateChunk(Voxel3 chunkCoordinates) {
        Vector3 worldPosition = new Vector3(chunkCoordinates.X * ChunkSize,
                                            chunkCoordinates.Y * ChunkSize,
                                            chunkCoordinates.Z * ChunkSize);

        GameObject chunkInstance = Instantiate(_chunkPrefab, worldPosition, Quaternion.identity);

        Chunk newChunk = chunkInstance.GetComponent<Chunk>();
        newChunk.ChunkCoordinates = chunkCoordinates;
        newChunk.World = this;
        if (chunkCoordinates.X == 0 && chunkCoordinates.Y == 0 && chunkCoordinates.Z == 0) {
            Voxel[,,] voxels = new Voxel[VoxelWorld.ChunkVoxelSize, VoxelWorld.ChunkVoxelSize, VoxelWorld.ChunkVoxelSize];
            for (int x = 0; x < VoxelWorld.ChunkVoxelSize; x++) {
                for (int y = 0; y < VoxelWorld.ChunkVoxelSize; y++) {
                    for (int z = 0; z < VoxelWorld.ChunkVoxelSize; z++) {
                        voxels[x, y, z] = new RoomVoxel();
                    }
                }
            }
            newChunk.Init(voxels);
        } else {
            newChunk.Init();
        }
        _chunksToUpdate.TryAdd(chunkCoordinates, newChunk);

        _chunks.Add(chunkCoordinates, newChunk);

        return newChunk;
    }

    private Voxel3 GetChunkCoordinatesFromVoxelWorldCoordinates(Voxel3 voxelCoordinates) {
        return new Voxel3(Mathf.FloorToInt(voxelCoordinates.X / (float)ChunkVoxelSize),
                          Mathf.FloorToInt(voxelCoordinates.Y / (float)ChunkVoxelSize),
                          Mathf.FloorToInt(voxelCoordinates.Z / (float)ChunkVoxelSize));
    }

    /// <summary>
    /// Given voxel coordinates, get the appropriate chunk.
    /// </summary>
    /// <param name="voxelCoordinates">The given voxel coordinates.</param>
    /// <returns>The chunk that should own the voxel for the given voxel coordinates, 
    /// if the chunk exists. Null if it does not.</returns>
    public Chunk GetChunkFromVoxelCoordinates(Voxel3 voxelCoordinates) {
        Voxel3 chunkCoordinates = GetChunkCoordinatesFromVoxelWorldCoordinates(voxelCoordinates);
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
        Chunk containingChunk = GetChunkFromVoxelCoordinates(voxelCoordinates);

        if (containingChunk != null) {
            Voxel3 localVoxelPositionInChunk = containingChunk.GetLocalVoxelPositionInChunk(voxelCoordinates);
            Voxel voxel = containingChunk.GetVoxel(localVoxelPositionInChunk);
            return voxel;
        }
        return new AirVoxel();
    }

    /// <summary>
    /// Set a voxel to a given voxel type at voxel world coordinates.
    /// </summary>
    /// <param name="voxelCoordinates">The coordinates for the voxel to set.</param>
    /// <param name="voxel">A voxel object, the type of voxel we want to set the voxel to.</param>
    public void SetVoxel(Voxel3 voxelCoordinates, Voxel voxel) {
        Voxel3 chunkCoordinates = GetChunkCoordinatesFromVoxelWorldCoordinates(voxelCoordinates);
        Chunk containingChunk = GetChunkFromVoxelCoordinates(voxelCoordinates);
        if (containingChunk == null) {
            // Chunk requested did not exist. Create a new chunk to house the voxel.
            containingChunk = CreateChunk(chunkCoordinates);
        }

        Voxel3 localVoxelPositionInChunk = containingChunk.GetLocalVoxelPositionInChunk(voxelCoordinates);
        containingChunk.SetVoxel(localVoxelPositionInChunk, voxel);

        // Check if voxel is on chunk boundary. If so, update boundary chunk neighbor.
        Voxel3 chunkLocalCoordinates = containingChunk.GetLocalVoxelPositionInChunk(voxelCoordinates);
        CheckToUpdateChunkNeighbors(chunkLocalCoordinates, chunkCoordinates);

        // Queue the owning chunk to be updated.
        _chunksToUpdate.TryAdd(chunkCoordinates, containingChunk);
    }

    /// <summary>
    /// Check if voxel is on a chunk boundary. If so, queue the chunk neighbor to update.
    /// </summary>
    /// <param name="voxelLocalCoordinatesInChunk">The voxel's chunk local coordinates.</param>
    /// <param name="chunkCoordinates">The chunk's world coordinates.</param>
    private void CheckToUpdateChunkNeighbors(Voxel3 voxelLocalCoordinatesInChunk, Voxel3 chunkCoordinates) {
        UpdateChunkIfEqual(voxelLocalCoordinatesInChunk.X, 0,
                           new Voxel3(chunkCoordinates.X - 1, chunkCoordinates.Y, chunkCoordinates.Z));

        UpdateChunkIfEqual(voxelLocalCoordinatesInChunk.X, ChunkVoxelSize - 1,
                           new Voxel3(chunkCoordinates.X + 1, chunkCoordinates.Y, chunkCoordinates.Z));

        UpdateChunkIfEqual(voxelLocalCoordinatesInChunk.Y, 0,
                           new Voxel3(chunkCoordinates.X, chunkCoordinates.Y - 1, chunkCoordinates.Z));

        UpdateChunkIfEqual(voxelLocalCoordinatesInChunk.Y, ChunkVoxelSize - 1,
                           new Voxel3(chunkCoordinates.X, chunkCoordinates.Y + 1, chunkCoordinates.Z));

        UpdateChunkIfEqual(voxelLocalCoordinatesInChunk.Z, 0,
                           new Voxel3(chunkCoordinates.X, chunkCoordinates.Y, chunkCoordinates.Z - 1));

        UpdateChunkIfEqual(voxelLocalCoordinatesInChunk.Z, ChunkVoxelSize - 1,
                           new Voxel3(chunkCoordinates.X, chunkCoordinates.Y, chunkCoordinates.Z + 1));
    }

    /// <summary>
    /// Helper method for CheckToUpdateChunkNeighbors.
    /// If val1 and val2 are equal (chunk boundary check), queue
    /// the chunk at chunkCoordinates to be updated.
    /// </summary>
    /// <param name="val1">A voxel local coordinate component.</param>
    /// <param name="val2">The boundary value.</param>
    /// <param name="chunkCoordinates">The chunk world coordinate to be updated if val1==val2.</param>
    private void UpdateChunkIfEqual(int val1, int val2, Voxel3 chunkCoordinates) {
        if (val1 != val2) {
            return;
        }
        Chunk chunk = null;
        _chunks.TryGetValue(chunkCoordinates, out chunk);
        if (chunk != null && !_chunksToUpdate.ContainsKey(chunkCoordinates)) {
            // Queue the neigboring chunk to be updated.
            _chunksToUpdate.Add(chunkCoordinates, chunk);
        }
    }
}

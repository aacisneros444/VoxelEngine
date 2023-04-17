using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AirVoxel : Voxel {
    public AirVoxel() : base() {

    }

    public override void AddSelfToChunkMeshData(MeshData chunkMeshData, Chunk chunk, Voxel3 localChunkCoordinates) {
        // Add nothing to chunkMeshData.
    }

    public override bool IsSolid() {
        return false;
    }
}

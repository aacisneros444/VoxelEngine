using UnityEngine;

public class Voxel {

    public static readonly Vector3[][] FaceVertexData = {
        new Vector3[] {
            // East face
            new Vector3(0.5f, -0.5f, -0.5f),
            new Vector3(0.5f, 0.5f, -0.5f),
            new Vector3(0.5f, 0.5f, 0.5f),
            new Vector3(0.5f, -0.5f, 0.5f)
        },
        new Vector3[] {
            // West Face
            new Vector3(-0.5f, -0.5f, 0.5f),
            new Vector3(-0.5f, 0.5f, 0.5f),
            new Vector3(-0.5f, 0.5f, -0.5f),
            new Vector3(-0.5f, -0.5f, -0.5f)
        },
        new Vector3[] {
            // Up Face
            new Vector3(-0.5f, 0.5f, 0.5f),
            new Vector3(0.5f, 0.5f, 0.5f),
            new Vector3(0.5f, 0.5f, -0.5f),
            new Vector3(-0.5f, 0.5f, -0.5f)
        },
        new Vector3[] {
            // Down Face
            new Vector3(-0.5f, -0.5f, -0.5f),
            new Vector3(0.5f, -0.5f, -0.5f),
            new Vector3(0.5f, -0.5f, 0.5f),
            new Vector3(-0.5f, -0.5f, 0.5f)
        },
        new Vector3[] {
            // North Face
            new Vector3(0.5f, -0.5f, 0.5f),
            new Vector3(0.5f, 0.5f, 0.5f),
            new Vector3(-0.5f, 0.5f, 0.5f),
            new Vector3(-0.5f, -0.5f, 0.5f)
        },
        new Vector3[] {
            // South Face
            new Vector3(-0.5f, -0.5f, -0.5f),
            new Vector3(-0.5f, 0.5f, -0.5f),
            new Vector3(0.5f, 0.5f, -0.5f),
            new Vector3(0.5f, -0.5f, -0.5f)
        }
    };

    public virtual void AddSelfToChunkMeshData(MeshData chunkMeshData, Chunk chunk, Voxel3 localChunkCoordinates) {
        foreach (Direction direction in System.Enum.GetValues(typeof(Direction))) {
            Voxel3 dirVector = direction.DirectionToVector();
            Voxel neighbor = chunk.GetVoxel(localChunkCoordinates + dirVector);
            if (ShouldDrawFaceInDirection(direction, neighbor)) {
                AddFaceToMeshData(chunkMeshData, direction, localChunkCoordinates);
            }
        }
    }

    protected virtual bool ShouldDrawFaceInDirection(Direction direction, Voxel neighbor) {
        return !neighbor.IsSolid();
    }

    protected void AddFaceToMeshData(MeshData meshData, Direction drawDirection, Voxel3 pos) {
        const int verticesPerFace = 4;
        Vector3[] newFaceVertices = new Vector3[verticesPerFace];
        Vector3[] faceVertexArray = FaceVertexData[(int)drawDirection];
        for (int i = 0; i < faceVertexArray.Length; i++) {
            Vector3 vertex = faceVertexArray[i];
            Vector3 modifiedVertex = vertex * VoxelWorld.VoxelSize;

            modifiedVertex.x += pos.X * VoxelWorld.VoxelSize;
            modifiedVertex.y += pos.Y * VoxelWorld.VoxelSize;
            modifiedVertex.z += pos.Z * VoxelWorld.VoxelSize;

            newFaceVertices[i] = modifiedVertex;
        }

        meshData.AddQuadWithUVs(newFaceVertices, GetFaceUVs(drawDirection), ShouldFlipNormals());
    }

    public virtual bool IsSolid() {
        return true;
    }

    public virtual bool ShouldFlipNormals() {
        return false;
    }

    public virtual Vector2 GetTextureTilePosition(Direction direction) {
        return new Vector2(0f, 0f);
    }

    public virtual Vector2[] GetFaceUVs(Direction direction) {
        Vector2[] uvs = new Vector2[4];
        Vector2 tilePos = GetTextureTilePosition(direction);

        const float tileSize = 0.25f;
        uvs[0] = new Vector2(tileSize * tilePos.x + tileSize, tileSize * tilePos.y);
        uvs[1] = new Vector2(tileSize * tilePos.x + tileSize, tileSize * tilePos.y + tileSize);
        uvs[2] = new Vector2(tileSize * tilePos.x, tileSize * tilePos.y + tileSize);
        uvs[3] = new Vector2(tileSize * tilePos.x, tileSize * tilePos.y);

        return uvs;
    }
}

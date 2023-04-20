using UnityEngine;

public static class VoxelUtils {
    /// <summary>
    /// Convert a Vector3 position, to voxel world coordinates. 
    /// </summary>
    /// <param name="pos">The given Vector3 position.</param>
    /// <returns>A Voxel3, the voxel world coordinates for the given position.</returns>
    public static Voxel3 GetVoxelCoordinates(Vector3 pos) {
        Voxel3 voxelCoordinates = new Voxel3(Mathf.RoundToInt(pos.x / (float)VoxelWorld.VoxelSize),
                                             Mathf.RoundToInt(pos.y / (float)VoxelWorld.VoxelSize),
                                             Mathf.RoundToInt(pos.z / (float)VoxelWorld.VoxelSize));
        return voxelCoordinates;
    }

    /// <summary>
    /// Given a raycast hit, return voxel world coordiantes.
    /// </summary>
    /// <param name="hit">The given raycast hit.</param>
    /// <param name="adjacent">Determines whether to get the voxel coordinates for the position 
    /// along the hit normal, or opposite to it.</param>
    /// <returns>A Voxel3, voxel world coordinates.</returns>
    public static Voxel3 GetVoxelCoordinates(RaycastHit hit, bool adjacent = false) {
        Vector3 voxelPosition = new Vector3(MoveWithinVoxel(hit.point.x, hit.normal.x, adjacent),
                                            MoveWithinVoxel(hit.point.y, hit.normal.y, adjacent),
                                            MoveWithinVoxel(hit.point.z, hit.normal.z, adjacent));
        return GetVoxelCoordinates(voxelPosition);
    }

    /// <summary>
    /// Move the given position component with or opposite the hit normal.
    /// </summary>
    /// <param name="pos">A position component.</param>
    /// <param name="normal">A normal component.</param>
    /// <param name="adjacent">Determines whether or not to move the position component
    /// with or opposite the hit normal.</param>
    /// <returns>The modified component moved along or opposite the hit normal component.</returns>
    private static float MoveWithinVoxel(float pos, float normal, bool adjacent = false) {
        if (adjacent) {
            pos += (normal * 0.5f * VoxelWorld.VoxelSize);
        } else {
            pos -= (normal * 0.5f * VoxelWorld.VoxelSize);
        }
        return pos;
    }
}
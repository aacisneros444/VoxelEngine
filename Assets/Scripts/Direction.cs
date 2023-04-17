using UnityEngine;

public enum Direction {
    East,
    West,
    Up,
    Down,
    North,
    South
}

public static class DirectionExtensions {
    public static readonly Voxel3[] s_dirVectors = {
        new Voxel3(1, 0, 0),
        new Voxel3(-1, 0, 0),
        new Voxel3(0, 1, 0),
        new Voxel3(0, -1, 0),
        new Voxel3(0, 0, 1),
        new Voxel3(0, 0, -1)
    };

    public static Voxel3 DirectionToVector(this Direction direction) {
        return s_dirVectors[(int)direction];
    }
}
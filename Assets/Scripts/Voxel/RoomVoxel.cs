using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomVoxel : Voxel {
    protected override bool ShouldDrawFaceInDirection(Direction direction, Voxel neighborInDirection) {
        return neighborInDirection.GetType() != typeof(RoomVoxel); ;
    }

    public override bool IsSolid() {
        return true;
    }

    public override bool ShouldFlipNormals() {
        return true;
    }

    public override Vector2 GetTextureTilePosition(Direction direction) {
        return new Vector2(0, 1);
    }
}

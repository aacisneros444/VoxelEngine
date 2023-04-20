public class Voxel3 {
    public int X;
    public int Y;
    public int Z;

    public Voxel3(int x, int y, int z) {
        X = x;
        Y = y;
        Z = z;
    }

    public static Voxel3 operator +(Voxel3 c1, Voxel3 c2) {
        return new Voxel3(c1.X + c2.X, c1.Y + c2.Y, c1.Z + c2.Z);
    }

    public static Voxel3 operator -(Voxel3 c1, Voxel3 c2) {
        return new Voxel3(c1.X - c2.X, c1.Y - c2.Y, c1.Z - c2.Z);
    }

    public static Voxel3 operator *(Voxel3 c1, int scalar) {
        return new Voxel3(c1.X * scalar, c1.Y * scalar, c1.Z * scalar);
    }

    // public static int Get1DimensionalIndex(Voxel3 position, int voxelSize) {
    //     return position.X + position.Y * voxelSize + position.Z * voxelSize * voxelSize;
    // }

    public override bool Equals(object obj) {
        if (!(obj is Voxel3)) {
            return false;
        }

        Voxel3 pos = (Voxel3)obj;
        return pos.X == X && pos.Y == Y && pos.Z == Z;
    }

    public override int GetHashCode() {
        int hash = 17;
        hash = hash * 31 + X.GetHashCode();
        hash = hash * 31 + Y.GetHashCode();
        hash = hash * 31 + Z.GetHashCode();
        return hash;
    }

    public override string ToString() {
        return $"({X}, {Y}, {Z})";
    }
}

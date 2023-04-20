using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VoxelEditor : MonoBehaviour {
    [SerializeField] private VoxelWorld _voxelWorld;
    [SerializeField] private GameObject _hitMarker;


    void Update() {
        if (Input.GetMouseButtonDown(0)) {
            TryEditVoxel();
        }
    }

    private void TryEditVoxel() {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit)) {
            _hitMarker.transform.position = hit.point;
            Voxel3 voxelCoordinates = VoxelUtils.GetVoxelCoordinates(hit, false);
            List<Voxel3> coords = new List<Voxel3>();
            for (int x = -1; x <= 1; x++) {
                for (int y = -1; y <= 1; y++) {
                    for (int z = 0; z <= 2; z++) {
                        coords.Add(new Voxel3(voxelCoordinates.X, voxelCoordinates.Y, voxelCoordinates.Z) + (new Voxel3(x, y, z)));
                    }
                }
            }
            // Debug.Log("hit, world pos: " + hit.point + "voxel coordinates: " + voxelCoordinates);
            float start = Time.realtimeSinceStartup;
            for (int i = 0; i < coords.Count; i++) {
                _voxelWorld.SetVoxel(coords[i], new RoomVoxel());
            }
            Debug.Log("Took " + ((Time.realtimeSinceStartup - start) * 1000) + " ms");
        }
    }
}

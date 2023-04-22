using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct VoxelSelectionEditData {
    public Voxel3 SelectedVoxelCoords;
    public Voxel SelectedVoxel;
    public Voxel3 SurfaceVoxelCoords;
    public Voxel SurfaceVoxel;

    public VoxelSelectionEditData(Voxel3 selectedVoxelCoords, Voxel selectedVoxel, Voxel3 surfaceVoxelCoords, Voxel surfaceVoxel) {
        this.SelectedVoxelCoords = selectedVoxelCoords;
        this.SelectedVoxel = selectedVoxel;
        this.SurfaceVoxelCoords = surfaceVoxelCoords;
        this.SurfaceVoxel = surfaceVoxel;
    }
}

public class VoxelEditor : MonoBehaviour {
    [SerializeField] private VoxelWorld _voxelWorld;
    [SerializeField] private GameObject _hitMarker;
    [SerializeField] private GameObject _selectionHighlight;

    private Voxel3 _startDragVoxelCoords;
    private List<VoxelSelectionEditData> _currentSelection;
    private Vector3 _dragHitNormal;

    private Bounds _selectionBounds;

    void Start() {
        _currentSelection = new List<VoxelSelectionEditData>();
        _selectionBounds = new Bounds();
    }

    void Update() {
        if (Input.GetMouseButtonDown(0)) {
            StartDragSelect();
        }

        if (Input.GetMouseButton(0)) {
            DragSelect();
        }

        if (Input.GetMouseButtonDown(1)) {
            _currentSelection.Clear();
            _selectionBounds = new Bounds(Vector3.zero, Vector3.zero);
        }

        UpdateSelectionHighlight();

        if (Input.GetKeyDown(KeyCode.Minus)) {
            EditSelectedVoxels(KeyCode.Minus);
        }

        if (Input.GetKeyDown(KeyCode.Equals)) {
            EditSelectedVoxels(KeyCode.Equals);
        }

        // if (Input.GetKeyDown(KeyCode.Equals)) {
        //     for (int i = 0; i < _currentSelection.Count; i++) {
        //         Voxel3 coords = _currentSelection[i].Item1;
        //         if (_currentSelection[i].Item2.GetType() == typeof(AirVoxel)) {
        //             coords += _dragHitNormal;
        //         }

        //         _voxelWorld.SetVoxel(coords, new AirVoxel());

        //         Voxel3 newCoords = coords + _dragHitNormal;
        //         _currentSelection[i] = (newCoords, _voxelWorld.GetVoxel(newCoords));

        //         _selectionBounds.center += new Vector3(_dragHitNormal.X, _dragHitNormal.Y, _dragHitNormal.Z) * VoxelWorld.VoxelSize;
        //     }
        // }
    }

    private void StartDragSelect() {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit)) {
            _startDragVoxelCoords = VoxelUtils.GetVoxelCoordinates(hit, false);
            _dragHitNormal = hit.normal;
        }
    }

    private void DragSelect() {
        _currentSelection.Clear();

        Voxel3 roundedNormal = new Voxel3(Mathf.RoundToInt(_dragHitNormal.x),
                                          Mathf.RoundToInt(_dragHitNormal.y),
                                          Mathf.RoundToInt(_dragHitNormal.z));

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit)) {
            Voxel3 endDragVoxelCoords = VoxelUtils.GetVoxelCoordinates(hit, false);
            Voxel3 dimensions = new Voxel3(Mathf.Abs(_startDragVoxelCoords.X - endDragVoxelCoords.X),
                                           Mathf.Abs(_startDragVoxelCoords.Y - endDragVoxelCoords.Y),
                                           Mathf.Abs(_startDragVoxelCoords.Z - endDragVoxelCoords.Z));
            Voxel3 start = new Voxel3(Mathf.Min(_startDragVoxelCoords.X, endDragVoxelCoords.X),
                                      Mathf.Min(_startDragVoxelCoords.Y, endDragVoxelCoords.Y),
                                      Mathf.Min(_startDragVoxelCoords.Z, endDragVoxelCoords.Z));
            for (int x = start.X; x <= start.X + dimensions.X; x++) {
                for (int y = start.Y; y <= start.Y + dimensions.Y; y++) {
                    for (int z = start.Z; z <= start.Z + dimensions.Z; z++) {
                        Voxel3 voxelCoords = new Voxel3(x, y, z);
                        Voxel3 surfaceVoxelCoords = voxelCoords + roundedNormal;
                        //_currentSelection.Add((voxelCoords, _voxelWorld.GetVoxel(voxelCoords)));
                        _currentSelection.Add(new VoxelSelectionEditData(voxelCoords,
                                                                         _voxelWorld.GetVoxel(voxelCoords),
                                                                         surfaceVoxelCoords,
                                                                         _voxelWorld.GetVoxel(surfaceVoxelCoords)));
                    }
                }
            }
            RecalculateBoundsForCurrentSelection(start, dimensions);
        }
    }

    private void RecalculateBoundsForCurrentSelection(Voxel3 dragStart, Voxel3 dimensions) {
        Vector3 boundsSize = new Vector3(dimensions.X, dimensions.Y, dimensions.Z) * VoxelWorld.VoxelSize;

        Vector3 voxelSize = new Vector3(VoxelWorld.VoxelSize, VoxelWorld.VoxelSize, VoxelWorld.VoxelSize);
        // Add a whole voxel worth of volume to size, as voxel positions start in the center of the voxel.
        boundsSize += voxelSize;

        // Vector3 dragAxis = GetDragAxisFromNormal(_dragHitNormal);
        // Vector3 otherDirection = Vector3.Cross(dragAxis, _dragHitNormal);
        // dragAxis = new Vector3(Mathf.Abs(dragAxis.x), Mathf.Abs(dragAxis.y), Mathf.Abs(dragAxis.z));
        // otherDirection = new Vector3(Mathf.Abs(otherDirection.x), Mathf.Abs(otherDirection.y), Mathf.Abs(otherDirection.z));
        // Debug.Log("Drag axis: " + dragAxis);
        // Debug.Log("Other dir: " + otherDirection);
        // boundsSize += dragAxis * VoxelWorld.VoxelSize;
        // boundsSize += otherDirection * VoxelWorld.VoxelSize;
        // boundsSize += _dragHitNormal * VoxelWorld.VoxelSize * 0.05f;

        Vector3 localCenter = boundsSize / 2f;
        // Get the center in world space by adding dragStart * VoxelSize and localCenter.
        Vector3 center = (new Vector3(dragStart.X, dragStart.Y, dragStart.Z) * VoxelWorld.VoxelSize + localCenter);
        // Subtract half of voxel size to correctly shift bounds to position.
        center -= voxelSize * 0.5f;

        // Move the center a bit to overcome clipping with the normal surface.
        Vector3 overcomeClippingConstant = _dragHitNormal * 0.05f;
        center += overcomeClippingConstant;

        _selectionBounds.size = boundsSize * 1.025f;
        _selectionBounds.center = center;
    }

    private Vector3 GetDragAxisFromNormal(Vector3 normal) {
        Vector3 referenceVector = Vector3.forward;
        if (normal == Vector3.forward || normal == -Vector3.forward || normal == Vector3.right || normal == -Vector3.right) {
            referenceVector = Vector3.up;
        }
        return Vector3.Cross(normal, referenceVector).normalized;
    }


    private void EditSelectedVoxels(KeyCode keyPressed) {
        Voxel3 roundedNormal = new Voxel3(Mathf.RoundToInt(_dragHitNormal.x),
                                          Mathf.RoundToInt(_dragHitNormal.y),
                                          Mathf.RoundToInt(_dragHitNormal.z));

        Voxel3 editDirection = keyPressed == KeyCode.Equals ? roundedNormal * -1 : roundedNormal;
        for (int i = 0; i < _currentSelection.Count; i++) {
            Voxel3 selectedCoords = _currentSelection[i].SelectedVoxelCoords;
            Voxel selectedVoxel = _currentSelection[i].SelectedVoxel;
            Voxel3 surfaceCoords = selectedCoords + roundedNormal;
            Voxel surfaceVoxel = _voxelWorld.GetVoxel(surfaceCoords);

            Voxel3 coordsToSet = null;
            System.Type voxelTypeToSetTo;
            if (keyPressed == KeyCode.Equals) {
                coordsToSet = selectedCoords;
                voxelTypeToSetTo = surfaceVoxel.GetType();
            } else {
                coordsToSet = surfaceCoords;
                voxelTypeToSetTo = selectedVoxel.GetType();
            }

            _voxelWorld.SetVoxel(coordsToSet, (Voxel)System.Activator.CreateInstance(voxelTypeToSetTo));

            selectedCoords = selectedCoords + editDirection;
            selectedVoxel = _voxelWorld.GetVoxel(selectedCoords);
            surfaceCoords = surfaceCoords + editDirection;
            surfaceVoxel = _voxelWorld.GetVoxel(surfaceCoords);

            _currentSelection[i] = new VoxelSelectionEditData(selectedCoords, selectedVoxel, surfaceCoords, surfaceVoxel);
        }
        _selectionBounds.center += new Vector3(editDirection.X, editDirection.Y, editDirection.Z) * VoxelWorld.VoxelSize;
    }

    void OnDrawGizmos() {
        if (_currentSelection == null) {
            return;
        }
        Gizmos.color = Color.white;
        Gizmos.DrawCube(_selectionBounds.center, _selectionBounds.size);
        foreach (VoxelSelectionEditData editData in _currentSelection) {
            Gizmos.color = Color.red;
            Vector3 center = new Vector3(editData.SelectedVoxelCoords.X, editData.SelectedVoxelCoords.Y, editData.SelectedVoxelCoords.Z) * VoxelWorld.VoxelSize;
            Vector3 size = new Vector3(VoxelWorld.VoxelSize, VoxelWorld.VoxelSize, VoxelWorld.VoxelSize);
            Gizmos.DrawWireCube(center, size);


            Gizmos.color = Color.white;
            center = new Vector3(editData.SurfaceVoxelCoords.X, editData.SurfaceVoxelCoords.Y, editData.SurfaceVoxelCoords.Z) * VoxelWorld.VoxelSize;
            size = new Vector3(VoxelWorld.VoxelSize, VoxelWorld.VoxelSize, VoxelWorld.VoxelSize);
            Gizmos.DrawWireCube(center, size);
        }
    }

    private void UpdateSelectionHighlight() {
        _selectionHighlight.transform.position = _selectionBounds.center;
        _selectionHighlight.transform.localScale = _selectionBounds.size;
    }
}

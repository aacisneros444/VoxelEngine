using System.Collections.Generic;
using UnityEngine;
using System;

public struct VoxelSelectionEditData {
    public Voxel3 SelectedVoxelCoords;
    public Voxel SelectedVoxel;
    public Voxel3 AdjacentVoxelCoords;
    public Voxel AdjacentVoxel;

    public VoxelSelectionEditData(Voxel3 selectedVoxelCoords, Voxel selectedVoxel, Voxel3 adjacentVoxelCoords, Voxel adjacentVoxel) {
        this.SelectedVoxelCoords = selectedVoxelCoords;
        this.SelectedVoxel = selectedVoxel;
        this.AdjacentVoxelCoords = adjacentVoxelCoords;
        this.AdjacentVoxel = adjacentVoxel;
    }
}

public class VoxelEditor : MonoBehaviour {
    [SerializeField] private VoxelWorld _voxelWorld;
    [SerializeField] private GameObject _hitMarker;
    [SerializeField] private GameObject _selectionHighlight;
    [SerializeField] private bool _instant;
    [SerializeField] private VoxelEditAnimationManager _voxelEditAnimationManager;

    private Voxel3 _startDragVoxelCoords;
    private List<VoxelSelectionEditData> _currentSelection;
    private Vector3 _dragHitNormal;

    private Bounds _selectionHighlightBounds;

    public event Action MadeNewSelection;
    public event Action<List<VoxelSelectionEditData>, Vector3, bool> EditedSelection;

    void Start() {
        _currentSelection = new List<VoxelSelectionEditData>();
        _selectionHighlightBounds = new Bounds();
    }

    void Update() {
        if (Input.GetMouseButtonDown(0)) {
            StartDragSelect();
        }

        if (Input.GetMouseButton(0)) {
            DragSelect();
        }

        if (Input.GetMouseButtonUp(0)) {
            MadeNewSelection?.Invoke();
        }

        if (Input.GetMouseButtonDown(1)) {
            _currentSelection.Clear();
            _selectionHighlightBounds = new Bounds(Vector3.zero, Vector3.zero);
        }

        UpdateSelectionHighlight();

        if (Input.GetKeyDown(KeyCode.Minus) || Input.GetKeyDown(KeyCode.Equals)) {
            bool extrusion = Input.GetKeyDown(KeyCode.Equals);
            if (_instant) {
                EditSelectedVoxels(_currentSelection, _dragHitNormal, extrusion);
            } else {
                EditedSelection?.Invoke(new List<VoxelSelectionEditData>(_currentSelection), _dragHitNormal, extrusion);
            }
        }
    }

    private void StartDragSelect() {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit)) {
            _startDragVoxelCoords = VoxelUtils.GetVoxelCoordinates(hit, true);
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
            Voxel3 endDragVoxelCoords = VoxelUtils.GetVoxelCoordinates(hit, true);
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
                        Voxel3 adjacentVoxelCoords = voxelCoords - roundedNormal;
                        //_currentSelection.Add((voxelCoords, _voxelWorld.GetVoxel(voxelCoords)));
                        _currentSelection.Add(new VoxelSelectionEditData(voxelCoords,
                                                                         _voxelWorld.GetVoxel(voxelCoords),
                                                                         adjacentVoxelCoords,
                                                                         _voxelWorld.GetVoxel(adjacentVoxelCoords)));
                    }
                }
            }
            RecalculateHighlightBoundsForCurrentSelection(start, dimensions);
        }
    }

    // Vector3 dragAxis = GetDragAxisFromNormal(_dragHitNormal);
    // Vector3 otherDirection = Vector3.Cross(dragAxis, _dragHitNormal);
    // Vector3 absNormal = new Vector3(Mathf.Abs(_dragHitNormal.x), Mathf.Abs(_dragHitNormal.y), Mathf.Abs(_dragHitNormal.z));
    // dragAxis = new Vector3(Mathf.Abs(dragAxis.x), Mathf.Abs(dragAxis.y), Mathf.Abs(dragAxis.z));
    // otherDirection = new Vector3(Mathf.Abs(otherDirection.x), Mathf.Abs(otherDirection.y), Mathf.Abs(otherDirection.z));
    // Debug.Log("Dimensions: " + dimensions);
    // Debug.Log("Drag axis: " + dragAxis);
    // Debug.Log("Other dir: " + otherDirection);
    // Debug.Log("Drag hit normal: " + _dragHitNormal);
    // boundsSize += dragAxis * VoxelWorld.VoxelSize;
    // boundsSize += otherDirection * VoxelWorld.VoxelSize;
    // boundsSize += absNormal * VoxelWorld.VoxelSize * 0.02f;

    // Add a whole voxel worth of volume to size, as voxel positions start in the center of the voxel.
    // Dimensions of (0, 0, 0) indicate a single voxel, must account for this as well.
    // boundsSize += voxelSize;

    /// <summary>
    /// Recalcuate the bounds used to highlight the current selection.
    /// </summary>
    /// <param name="dragStart">The voxel coordinates for the drag selection start.</param>
    /// <param name="dimensions">The voxel dimensions for the drag selection.</param>
    private void RecalculateHighlightBoundsForCurrentSelection(Voxel3 dragStart, Voxel3 dimensions) {
        Vector3 boundsSize = new Vector3(dimensions.X, dimensions.Y, dimensions.Z) * VoxelWorld.VoxelSize;
        Vector3 voxelSize = new Vector3(VoxelWorld.VoxelSize, VoxelWorld.VoxelSize, VoxelWorld.VoxelSize);

        Vector3 dragAxis = GetDragAxisFromNormal(_dragHitNormal);
        Vector3 otherDirection = Vector3.Cross(dragAxis, _dragHitNormal);

        // Absolute normal
        Vector3 absNormal = new Vector3(Mathf.Abs(_dragHitNormal.x), Mathf.Abs(_dragHitNormal.y), Mathf.Abs(_dragHitNormal.z));
        // The absolute axis of the drag selection
        dragAxis = new Vector3(Mathf.Abs(dragAxis.x), Mathf.Abs(dragAxis.y), Mathf.Abs(dragAxis.z));
        // The absolute axis of the direction perpendicular to the drag axis/ normal
        // For example, if we are dragging along the x axis, and the normal is along the z axis, this will be y axis (0, 1, 0)
        otherDirection = new Vector3(Mathf.Abs(otherDirection.x), Mathf.Abs(otherDirection.y), Mathf.Abs(otherDirection.z));

        // Add some size in every direction, as dimensions can be (0, 0, 0), indicative
        // of a selection of 1 voxel.
        boundsSize += dragAxis * VoxelWorld.VoxelSize;
        boundsSize += otherDirection * VoxelWorld.VoxelSize;
        // This will be the thickness of the bounds in the direction of the normal.
        const float percentThickness = 0.02f;
        boundsSize += absNormal * VoxelWorld.VoxelSize * percentThickness;

        Vector3 localCenter = boundsSize * 0.5f;
        // Get the center in world space by adding dragStart * VoxelSize and localCenter.
        Vector3 center = (new Vector3(dragStart.X, dragStart.Y, dragStart.Z) * VoxelWorld.VoxelSize + localCenter);

        // Subtract half of voxel size to correctly shift bounds to position.
        center -= VoxelWorld.VoxelSize * 0.5f * dragAxis;
        center -= VoxelWorld.VoxelSize * 0.5f * otherDirection;
        // Subtract half of voxel size * normal to shift bounds to voxel face.
        const float percentToVoxelFace = 0.95f;
        center -= VoxelWorld.VoxelSize * 0.5f * _dragHitNormal * percentToVoxelFace;

        _selectionHighlightBounds.size = boundsSize;
        _selectionHighlightBounds.center = center;
    }

    /// <summary>
    /// Given a normal, calculate the selection drag axis.
    /// </summary>
    /// <param name="normal">The given normal.</param>
    /// <returns>The axis for the selection drag.</returns>
    private Vector3 GetDragAxisFromNormal(Vector3 normal) {
        Vector3 referenceVector = Vector3.up;
        if (normal == Vector3.up || normal == -Vector3.up) {
            referenceVector = Vector3.forward;
        }
        return Vector3.Cross(normal, referenceVector).normalized;
    }


    public void EditSelectedVoxels(List<VoxelSelectionEditData> selectionEditData,
                                   Vector3 editNormal,
                                   bool extrusion) {
        Voxel3 roundedNormal = new Voxel3(Mathf.RoundToInt(editNormal.x),
                                          Mathf.RoundToInt(editNormal.y),
                                          Mathf.RoundToInt(editNormal.z));

        Voxel3 editDirection = !extrusion ? roundedNormal * -1 : roundedNormal;
        for (int i = 0; i < selectionEditData.Count; i++) {
            // The selected voxel.
            Voxel3 selectedCoords = selectionEditData[i].SelectedVoxelCoords;
            Voxel selectedVoxel = selectionEditData[i].SelectedVoxel;
            // The voxel adjacent to the selected voxel in the opposite normal direction.
            Voxel3 adjacentCoords = selectedCoords - roundedNormal;
            Voxel adjacentVoxel = _voxelWorld.GetVoxel(adjacentCoords);

            Voxel3 coordsToSet = null;
            System.Type voxelTypeToSetTo;
            if (extrusion) {
                coordsToSet = selectedCoords;
                voxelTypeToSetTo = adjacentVoxel.GetType();
            } else {
                coordsToSet = adjacentCoords;
                voxelTypeToSetTo = selectedVoxel.GetType();
            }

            _voxelWorld.SetVoxel(coordsToSet, (Voxel)System.Activator.CreateInstance(voxelTypeToSetTo));

            selectedCoords = selectedCoords + editDirection;
            selectedVoxel = _voxelWorld.GetVoxel(selectedCoords);
            adjacentCoords = adjacentCoords + editDirection;
            adjacentVoxel = _voxelWorld.GetVoxel(adjacentCoords);

            selectionEditData[i] = new VoxelSelectionEditData(selectedCoords, selectedVoxel, adjacentCoords, adjacentVoxel);
        }
        _selectionHighlightBounds.center += new Vector3(editDirection.X, editDirection.Y, editDirection.Z) * VoxelWorld.VoxelSize;
    }

    void OnDrawGizmos() {
        if (_currentSelection == null) {
            return;
        }
        Gizmos.color = Color.white;
        Gizmos.DrawCube(_selectionHighlightBounds.center, _selectionHighlightBounds.size);
        foreach (VoxelSelectionEditData editData in _currentSelection) {
            Gizmos.color = Color.red;
            Vector3 center = new Vector3(editData.SelectedVoxelCoords.X, editData.SelectedVoxelCoords.Y, editData.SelectedVoxelCoords.Z) * VoxelWorld.VoxelSize;
            Vector3 size = new Vector3(VoxelWorld.VoxelSize, VoxelWorld.VoxelSize, VoxelWorld.VoxelSize);
            Gizmos.DrawWireCube(center, size);


            Gizmos.color = Color.white;
            center = new Vector3(editData.AdjacentVoxelCoords.X, editData.AdjacentVoxelCoords.Y, editData.AdjacentVoxelCoords.Z) * VoxelWorld.VoxelSize;
            size = new Vector3(VoxelWorld.VoxelSize, VoxelWorld.VoxelSize, VoxelWorld.VoxelSize);
            Gizmos.DrawWireCube(center, size);
        }
    }

    private void UpdateSelectionHighlight() {
        _selectionHighlight.transform.position = _selectionHighlightBounds.center;
        _selectionHighlight.transform.localScale = _selectionHighlightBounds.size;
    }
}

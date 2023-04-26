using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VoxelEditAnimationData {
    public VoxelEditAnimationController AnimController;
    public VoxelEditor VoxelEditor;
    public List<VoxelSelectionEditData> VoxelEditSelectionData;
    public GameObject VoxelVisualPrefab;
    public Vector3 EditNormal;
    public int LayersToAnimate;
    public bool IsExtrusion;
    public float AnimationDuration;

    public List<StartEndKeyframes> VoxelStartEndPositions;
    public List<GameObject> VoxelVisualPrefabInstances;

    public float StartTime;
    public int LayersAnimated;
    public bool LayersUpdated;
    public bool SelectionForAnimationActive;

    public VoxelEditAnimationData(
        VoxelEditAnimationController animationController,
        VoxelEditor voxelEditor,
        List<VoxelSelectionEditData> editSelectionData,
        GameObject voxelVisuabPrefab,
        Vector3 editNormal,
        int layersToAnimate,
        bool isExtrusion,
        float animationDuration
        ) {
        AnimController = animationController;
        VoxelEditor = voxelEditor;
        VoxelEditSelectionData = editSelectionData;
        VoxelVisualPrefab = voxelVisuabPrefab;
        EditNormal = editNormal;
        LayersToAnimate = layersToAnimate;
        IsExtrusion = isExtrusion;
        AnimationDuration = animationDuration;
        SelectionForAnimationActive = true;
    }
}


public class StartEndKeyframes {
    public Vector3 StartPosition;
    public Vector3 EndPosition;

    public StartEndKeyframes(Vector3 startPosition, Vector3 endPosition) {
        StartPosition = startPosition;
        EndPosition = endPosition;
    }
}
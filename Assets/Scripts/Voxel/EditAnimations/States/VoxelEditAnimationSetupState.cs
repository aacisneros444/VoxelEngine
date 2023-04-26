using System.Collections.Generic;
using UnityEngine;

public class VoxelEditAnimationSetupState : IState {

    private StateMachine _parentFsm;
    private VoxelEditAnimationData _animData;

    public VoxelEditAnimationSetupState(StateMachine parentFsm, VoxelEditAnimationData animData) {
        _animData = animData;
        _parentFsm = parentFsm;
    }

    public void Enter() {
        Debug.Log("Enter:" + this.GetType().Name);
        Setup();
        _parentFsm.PopState();
        _parentFsm.PushState(new VoxelEditAnimationPreAnimState(_parentFsm, _animData));
    }

    public void Exit() {

    }

    public void FixedUpdate() {

    }

    public void Update() {

    }

    private void Setup() {
        _animData.VoxelStartEndPositions = new List<StartEndKeyframes>();
        for (int i = 0; i < _animData.VoxelEditSelectionData.Count; i++) {
            Vector3 selectedPosition = _animData.VoxelEditSelectionData[i].SelectedVoxelCoords.ToVector3() * VoxelWorld.VoxelSize;
            Vector3 adjacentPosition = _animData.VoxelEditSelectionData[i].AdjacentVoxelCoords.ToVector3() * VoxelWorld.VoxelSize;
            Vector3 behindAdjacentPosition = adjacentPosition - _animData.EditNormal * VoxelWorld.VoxelSize;

            Vector3 closestAirVoxelPostionToAdjacent = Vector3.zero;

            (Vector3, Vector3) startEndPos = _animData.IsExtrusion ?
                                             (adjacentPosition, selectedPosition) :
                                             (adjacentPosition, behindAdjacentPosition);
            StartEndKeyframes startEnd = new StartEndKeyframes(startEndPos.Item1, startEndPos.Item2);
            _animData.VoxelStartEndPositions.Add(startEnd);
        }

        if (_animData.VoxelVisualPrefabInstances == null) {
            _animData.VoxelVisualPrefabInstances = new List<GameObject>();
            for (int i = 0; i < _animData.VoxelEditSelectionData.Count; i++) {
                GameObject voxelVisualPrefabInstance = Object.Instantiate(_animData.VoxelVisualPrefab,
                                                                          _animData.VoxelStartEndPositions[i].StartPosition,
                                                                          Quaternion.identity);
                _animData.VoxelVisualPrefabInstances.Add(voxelVisualPrefabInstance);
            }
        }
    }
}

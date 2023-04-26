using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VoxelEditAnimationPostAnimState : IState {
    private StateMachine _parentFsm;
    private VoxelEditAnimationData _animData;

    public VoxelEditAnimationPostAnimState(StateMachine parentFsm, VoxelEditAnimationData animData) {
        _animData = animData;
        _parentFsm = parentFsm;
    }

    public void Enter() {
        Debug.Log("Enter:" + this.GetType().Name);
        if (_animData.IsExtrusion) {
            _animData.LayersAnimated += 1;
        } else {
            _animData.LayersAnimated -= 1;
        }

        if (_animData.IsExtrusion) {
            // This is an extrusion. We must update the real voxel geometry post animating.
            _animData.VoxelEditor.EditSelectedVoxels(_animData.VoxelEditSelectionData, _animData.EditNormal, true);
        }

        if (_animData.LayersAnimated != _animData.LayersToAnimate) {
            bool changedDirection = _animData.LayersUpdated &&
                                    (_animData.IsExtrusion && _animData.LayersToAnimate < _animData.LayersAnimated) ||
                                    (!_animData.IsExtrusion && _animData.LayersToAnimate > _animData.LayersAnimated);
            if (!changedDirection) {
                UpdateVoxelStartEndPositions();
                _parentFsm.PopState();
                _parentFsm.PushState(new VoxelEditAnimationPreAnimState(_parentFsm, _animData));
            } else {
                _animData.LayersUpdated = false;
                _animData.IsExtrusion = !_animData.IsExtrusion;
                // Changed edit direction, must setup state again for new edit direction.
                _parentFsm.PopState();
                _parentFsm.PushState(new VoxelEditAnimationSetupState(_parentFsm, _animData));
            }
        }
    }

    public void Exit() {

    }

    public void FixedUpdate() {

    }

    public void Update() {
        if (_animData.LayersAnimated == _animData.LayersToAnimate && !_animData.SelectionForAnimationActive) {
            _animData.AnimController.StartDestroy();
        }
    }

    public void UpdateVoxelStartEndPositions() {
        for (int i = 0; i < _animData.VoxelStartEndPositions.Count; i++) {
            StartEndKeyframes animData = _animData.VoxelStartEndPositions[i];
            Vector3 displacement = _animData.EditNormal * VoxelWorld.VoxelSize;
            displacement = !_animData.IsExtrusion ? displacement * -1 : displacement;
            animData.StartPosition += displacement;
            animData.EndPosition += displacement;
        }
    }
}

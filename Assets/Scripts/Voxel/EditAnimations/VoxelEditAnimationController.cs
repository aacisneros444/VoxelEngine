using System;
using System.Collections.Generic;
using UnityEngine;

public class VoxelEditAnimationController {

    private VoxelEditAnimationData _animData;
    private StateMachine _stateMachine;
    public bool MarkedForDestroy;

    public VoxelEditAnimationController(VoxelEditor voxelEditor,
                              List<VoxelSelectionEditData> editSelectionData,
                              GameObject voxelVisuabPrefab,
                              Vector3 editNormal,
                              int layersToAnimate,
                              bool extrusion,
                              float animationDuration) {
        _animData = new VoxelEditAnimationData(this,
                                               voxelEditor,
                                               editSelectionData,
                                               voxelVisuabPrefab,
                                               editNormal,
                                               layersToAnimate,
                                               extrusion,
                                               animationDuration);
        _animData.LayersToAnimate = _animData.IsExtrusion ? _animData.LayersToAnimate : -_animData.LayersToAnimate;
        _stateMachine = new StateMachine();
        _stateMachine.PushState(new VoxelEditAnimationSetupState(_stateMachine, _animData));
    }

    public void Update() {
        _stateMachine.Update();
    }

    public void AddOrRemoveLayer(bool extrusion) {
        if (_animData.LayersAnimated == _animData.LayersToAnimate) {
            // Immediately starting an extrusion or retraction.
            _animData.IsExtrusion = extrusion;
        }

        if (extrusion) {
            _animData.LayersToAnimate += 1;
        } else {
            _animData.LayersToAnimate -= 1;
        }

        _animData.LayersUpdated = true;

        if (_stateMachine.GetCurrentState().GetType() == typeof(VoxelEditAnimationPostAnimState)) {
            _stateMachine.PopState();
            _stateMachine.PushState(new VoxelEditAnimationSetupState(_stateMachine, _animData));
        }
    }

    public void SetSelectionForAnimationNotActive() {
        _animData.SelectionForAnimationActive = false;
    }

    public void StartDestroy() {
        for (int i = 0; i < _animData.VoxelVisualPrefabInstances.Count; i++) {
            UnityEngine.Object.Destroy(_animData.VoxelVisualPrefabInstances[i]);
        }
        MarkedForDestroy = true;
    }
}
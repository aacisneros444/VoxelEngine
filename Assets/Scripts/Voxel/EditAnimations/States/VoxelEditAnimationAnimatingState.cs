using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VoxelEditAnimationAnimatingState : IState {
    private StateMachine _parentFsm;
    private VoxelEditAnimationData _animData;

    public VoxelEditAnimationAnimatingState(StateMachine parentFsm, VoxelEditAnimationData animData) {
        _animData = animData;
        _parentFsm = parentFsm;
    }

    public void Enter() {
        Debug.Log("Enter:" + this.GetType().Name);
    }

    public void Exit() {

    }

    public void FixedUpdate() {

    }

    public void Update() {
        float timeSinceStart = Time.time - _animData.StartTime;
        float t = Mathf.Clamp01(timeSinceStart / _animData.AnimationDuration);
        for (int i = 0; i < _animData.VoxelVisualPrefabInstances.Count; i++) {
            StartEndKeyframes startEnd = _animData.VoxelStartEndPositions[i];
            _animData.VoxelVisualPrefabInstances[i].transform.position =
                Vector3.Lerp(startEnd.StartPosition, startEnd.EndPosition, t);
        }
        if (t >= 1) {
            _parentFsm.PopState();
            _parentFsm.PushState(new VoxelEditAnimationPostAnimState(_parentFsm, _animData));
        }
    }
}

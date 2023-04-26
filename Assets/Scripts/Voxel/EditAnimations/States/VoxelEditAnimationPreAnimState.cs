using UnityEngine;

public class VoxelEditAnimationPreAnimState : IState {

    private StateMachine _parentFsm;
    private VoxelEditAnimationData _animData;

    public VoxelEditAnimationPreAnimState(StateMachine parentFsm, VoxelEditAnimationData animData) {
        _animData = animData;
        _parentFsm = parentFsm;
    }

    public void Enter() {
        Debug.Log("Enter:" + this.GetType().Name);
        Debug.Log("PreAnim for layer " + (_animData.LayersAnimated + (_animData.IsExtrusion ? 1 : -1)));
        _animData.StartTime = Time.time;
        if (!_animData.IsExtrusion) {
            // This is a retraction. We must update the real voxel geometry before animating.
            _animData.VoxelEditor.EditSelectedVoxels(_animData.VoxelEditSelectionData, _animData.EditNormal, false);
        }
        _parentFsm.PopState();
        _parentFsm.PushState(new VoxelEditAnimationAnimatingState(_parentFsm, _animData));
    }

    public void Exit() {

    }

    public void FixedUpdate() {

    }

    public void Update() {

    }
}

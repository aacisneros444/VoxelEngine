using System.Collections.Generic;
using UnityEngine;

public class VoxelEditAnimationManager : MonoBehaviour {

    [SerializeField] private VoxelEditor _voxelEditor;
    [SerializeField] private GameObject _voxelVisualPrefab;
    [SerializeField] private float _animationDuration = 1f;
    private Dictionary<int, VoxelEditAnimationController> _runningAnimations;
    private VoxelEditAnimationController _currentAnimation;
    private bool _called;
    private int _animIdCounter;

    private void Awake() {
        _runningAnimations = new Dictionary<int, VoxelEditAnimationController>();
    }

    private void Start() {
        _voxelEditor.MadeNewSelection += ClearCurrentAnimation;
        _voxelEditor.EditedSelection += UpdateOrCreateAnimForEdit;
    }

    private void OnDestroy() {
        _voxelEditor.MadeNewSelection -= ClearCurrentAnimation;
        _voxelEditor.EditedSelection -= UpdateOrCreateAnimForEdit;
    }

    private void ClearCurrentAnimation() {
        if (_currentAnimation != null) {
            _currentAnimation.SetSelectionForAnimationNotActive();
            _currentAnimation = null;
        }
    }

    private void UpdateOrCreateAnimForEdit(List<VoxelSelectionEditData> editSelectionData, Vector3 editNormal, bool extrude) {
        if (_currentAnimation == null) {
            int animId = ++_animIdCounter;
            VoxelEditAnimationController voxelEditAnimationController = new VoxelEditAnimationController(
                _voxelEditor,
                editSelectionData,
                _voxelVisualPrefab,
                editNormal,
                1,
                extrude,
                _animationDuration);
            _runningAnimations.Add(animId, voxelEditAnimationController);
            _currentAnimation = voxelEditAnimationController;
        } else {
            _currentAnimation.AddOrRemoveLayer(extrude);
        }
    }

    private void Update() {
        List<int> toRemove = new List<int>();
        foreach (KeyValuePair<int, VoxelEditAnimationController> animKv in _runningAnimations) {
            if (!animKv.Value.MarkedForDestroy) {
                animKv.Value.Update();
            } else {
                toRemove.Add(animKv.Key);
            }
        }

        foreach (int animId in toRemove) {
            _runningAnimations.Remove(animId);
        }
    }
}

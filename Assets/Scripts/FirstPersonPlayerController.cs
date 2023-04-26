using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirstPersonPlayerController : MonoBehaviour {

    [SerializeField] private Camera _camera;
    [SerializeField] private float _cameraSensitivity = 2.5f;
    [SerializeField] private float _moveSpeed = 1f;
    private Rigidbody _rb;

    private void Start() {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        _rb = GetComponent<Rigidbody>();
    }


    private void Update() {
        _camera.transform.position = transform.position + new Vector3(0f, 0.5f, 0f);
        RotateCameraForMouseInput();
        MovePlayer();
    }

    private void MovePlayer() {
        Vector3 input = new Vector3(Input.GetAxisRaw("Horizontal"), 0f, Input.GetAxisRaw("Vertical"));
        Vector3 moveDir = Quaternion.AngleAxis(_camera.transform.eulerAngles.y, Vector3.up) * input;
        moveDir.Normalize();
        _rb.position = _rb.position + moveDir * _moveSpeed * Time.deltaTime;
    }

    private void RotateCameraForMouseInput() {
        Vector3 mouseInput = new Vector3(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"), 0f);
        mouseInput *= _cameraSensitivity;

        Vector3 currEulerAngles = _camera.transform.eulerAngles;
        float newRotX = currEulerAngles.x - mouseInput.y;
        float newRotY = currEulerAngles.y + mouseInput.x;
        if (newRotX > 90f && newRotX < 180f) {
            // Clamp to quadrant 1
            newRotX = 90f;
        } else if (newRotX > 180f && newRotX < 270f) {
            // Clamp to quadrant 4
            newRotX = 270f;
        }

        Quaternion newRotation = Quaternion.Euler(newRotX, newRotY, currEulerAngles.z);
        _camera.transform.rotation = newRotation;
    }
}

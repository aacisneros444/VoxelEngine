using UnityEngine;

public class CameraController : MonoBehaviour {

    [SerializeField] private float _sensitivity = 2.5f;
    [SerializeField] private float _moveSpeed = 40f;
    private bool _inFreeRoamState = true;

    void Start() {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update() {
        if (Input.GetKeyDown(KeyCode.Space)) {
            if (_inFreeRoamState) {
                _inFreeRoamState = false;
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            } else {
                _inFreeRoamState = true;
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
        }

        Vector3 input = new Vector3(Input.GetAxisRaw("Horizontal"), 0f, Input.GetAxisRaw("Vertical"));
        if (_inFreeRoamState) {
            FreeRoamState(input);
        } else {
            SelectModeState(input);
        }
    }

    private void RotateCameraForMouseInput() {
        Vector3 mouseInput = new Vector3(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"), 0f);
        mouseInput *= _sensitivity;

        Vector3 currEulerAngles = transform.eulerAngles;
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
        transform.rotation = newRotation;
    }

    private void FreeRoamMoveCameraForInput(Vector3 input) {
        Quaternion cameraRotation = Quaternion.Euler(transform.eulerAngles.x, transform.eulerAngles.y, 0f);
        Vector3 moveDir = cameraRotation * input;
        if (Input.GetKey(KeyCode.Q)) {
            moveDir -= Vector3.up;
        }
        if (Input.GetKey(KeyCode.E)) {
            moveDir += Vector3.up;
        }
        moveDir.Normalize();
        transform.position += moveDir * _moveSpeed * Time.deltaTime;
    }

    private void SelectModeMoveCameraForInput(Vector3 input) {
        Vector3 moveDir = input.x * transform.right +
                          input.z * Vector3.ProjectOnPlane(transform.forward, Vector3.up).normalized;
        if (Input.GetKey(KeyCode.Q)) {
            moveDir -= Vector3.up;
        }
        if (Input.GetKey(KeyCode.E)) {
            moveDir += Vector3.up;
        }
        moveDir.Normalize();
        transform.position += moveDir * _moveSpeed * Time.deltaTime;
    }

    private void FreeRoamState(Vector3 input) {
        RotateCameraForMouseInput();
        FreeRoamMoveCameraForInput(input);
    }

    private void SelectModeState(Vector3 input) {
        SelectModeMoveCameraForInput(input);
    }
}

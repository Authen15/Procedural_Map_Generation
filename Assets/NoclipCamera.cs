using UnityEngine;
using UnityEngine.InputSystem;

public class NoclipCamera : MonoBehaviour
{
    private Vector2 _move;
    private Vector2 _look;
    private float _jump;
    private float _sprint;

    [Header("Settings")]
    public float moveSpeed = 15f;
    public float lookSensitivity = 1f;

    private float _pitch = 0f; // Vertical rotation

    public void OnMove(InputValue value)
    {
        _move = value.Get<Vector2>();
    }

    public void OnJump(InputValue value)
    {
        _jump = value.Get<float>();
    }

    public void OnSprint(InputValue value)
    {
        _sprint = value.Get<float>();
    }

    public void OnLook(InputValue value)
    {
        _look = value.Get<Vector2>();
    }

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void Update()
    {
        HandleMovement();
        HandleLook();
    }

    private void HandleMovement()
    {
        Vector3 forward = transform.forward * _move.y;
        Vector3 right = transform.right * _move.x;
        Vector3 up = transform.up * _jump;
        Vector3 moveDirection = (forward + right + up).normalized;

        Vector3 sprintSpeed = moveDirection * moveSpeed * _sprint * 3f;
        transform.position += (moveDirection * moveSpeed + sprintSpeed) * Time.deltaTime ;
    }

    private void HandleLook()
    {
        float yaw = _look.x * lookSensitivity;
        float pitch = -_look.y * lookSensitivity; // Inverted for natural camera control

        _pitch = Mathf.Clamp(_pitch + pitch, -90f, 90f); // Limit vertical rotation

        transform.rotation = Quaternion.Euler(_pitch, transform.eulerAngles.y + yaw, 0f);
    }
}

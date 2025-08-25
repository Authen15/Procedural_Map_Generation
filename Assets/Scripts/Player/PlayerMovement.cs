using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{

    [Header("Movement")]
    public float BaseMoveSpeed;
    public float SprintSpeedMultiplier;
    public float jumpHeight;
    public float AirMultiplier;
    private float _gravityValue = -9.81f;

    [Header("view")]
    [Range(0.0f, 1f)] public float rotationPower = 0.8f;
    public float rotationLerp = 0.5f;


    private Vector2 _move;
    private Vector2 _look;
    private bool _isJumping;
    private bool _isSprinting;
    private bool _isGrounded;

    private Vector3 _playerVelocity;
    private Quaternion _nextRotation;
    private CharacterController _characterController;

    public void OnMove(InputAction.CallbackContext context)
    {
        _move = context.ReadValue<Vector2>();
    }

    public void OnLook(InputAction.CallbackContext context)
    {
        _look = context.ReadValue<Vector2>();
    }

    public void OnSprint(InputAction.CallbackContext context)
    {
        _isSprinting = context.ReadValue<float>() > 0;
    }


    public void OnJump(InputAction.CallbackContext context)
    {
        _isJumping = context.ReadValue<float>() > 0 && _isGrounded;
    }

    private void Start()
    {
        _characterController = GetComponent<CharacterController>();
    }

    private void Update()
    {
        // using a raycast to check if the player is grounded because the character controller's isGrounded property isn't working properly
        _isGrounded = Physics.Raycast(transform.position, Vector3.down, _characterController.height / 2 + 0.1f);
        if (_isGrounded && _playerVelocity.y < 0)
        {
            _playerVelocity.y = 0f;
        }

        MovePlayer();
        RotatePlayer();
        ApplyJump();

        _playerVelocity.y += _gravityValue * Time.deltaTime;
        _characterController.Move(_playerVelocity * Time.deltaTime);
    }

    private void MovePlayer()
    {
        Vector3 moveDirection = transform.forward * _move.y + transform.right * _move.x;

        float moveSpeed = BaseMoveSpeed;
        if (_isSprinting)
            moveSpeed *= SprintSpeedMultiplier;

        if (_isGrounded)
            _characterController.Move(moveDirection.normalized * moveSpeed * Time.deltaTime);
        else
            _characterController.Move(moveDirection.normalized * moveSpeed * AirMultiplier * Time.deltaTime);

    }

    private void RotatePlayer()
    {
        transform.rotation *= Quaternion.AngleAxis(_look.x * rotationPower, Vector3.up);
        _nextRotation = Quaternion.Lerp(transform.rotation, _nextRotation, Time.deltaTime * rotationLerp);
    }

    private void ApplyJump()
    {
        // reset y velocity
        if (_isJumping && _isGrounded){
            _playerVelocity.y += Mathf.Sqrt(jumpHeight * -2.0f * _gravityValue);
            _isJumping = false;
        }
    }
}
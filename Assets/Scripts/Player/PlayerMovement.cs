using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{

    private CreatureStats _playerStats;

    [Header("Movement")]
    public float SprintSpeedMultiplier;
    public float jumpHeight;
    public float AirMultiplier;
    private float _gravityValue = -9.81f * 4f;

    [Header("view")]
    [SerializeField] private GameObject _cameraTarget;
    [Range(0.0f, 1f)] public float rotationPower = 0.8f;
    public float rotationLerp = 0.5f;

    [Header("Animation")]
    public Animator animator;

    private Vector2 _move;
    private Vector2 _look;
    private bool _isJumping;
    private bool _isSprinting;
    private bool _isGrounded;

    private Vector3 _playerVelocity;
    // private Quaternion _nextRotation;
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
        _playerStats = GetComponent<CreatureStats>();

        Cursor.lockState = CursorLockMode.Locked;
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
        Animation();

        _playerVelocity.y += _gravityValue * Time.deltaTime;
        _characterController.Move(_playerVelocity * Time.deltaTime);
    }

    private void Animation()
    {
        if (_move.magnitude > 0.1f)
            animator.SetInteger("Speed", 1);
        else
            animator.SetInteger("Speed", 0);
    }

    private void MovePlayer()
    {
        Vector3 moveDirection = transform.forward * _move.y + transform.right * _move.x;

        float moveSpeed = _playerStats.MovementSpeed.Value;
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
        // _nextRotation = Quaternion.Lerp(transform.rotation, _nextRotation, Time.deltaTime * rotationLerp);

        #region Vertical Rotation
        _cameraTarget.transform.rotation *= Quaternion.AngleAxis(-_look.y * rotationPower, Vector3.right);

        var angles = _cameraTarget.transform.localEulerAngles;
        angles.z = 0;

        var angle = _cameraTarget.transform.localEulerAngles.x;

        //Clamp the Up/Down rotation
        if (angle > 180 && angle < 340)
        {
            angles.x = 340;
        }
        else if(angle < 180 && angle > 40)
        {
            angles.x = 40;
        }


        _cameraTarget.transform.localEulerAngles = angles;
        #endregion
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
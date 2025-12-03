using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    public float SprintSpeedMultiplier;
    private float _gravityValue = -9.81f * 4f;

    [Header("view")]
    [SerializeField] private GameObject _cameraTarget;
    [Range(0.0f, 1f)] public float rotationPower = 0.8f;
    public float rotationLerp = 0.5f;

    [Header("Animation")]
    public Animator animator;

    private Vector2 _move;
    private bool _isSprinting;

    private Vector3 _playerVelocity;
    private Camera _mainCam;

    private CreatureStats _playerStats;
    private PlayerAim _playerAim;
    private CharacterController _characterController;

    public void OnMove(InputAction.CallbackContext context)
    {
        _move = context.ReadValue<Vector2>();
    }

    public void OnSprint(InputAction.CallbackContext context)
    {
        _isSprinting = context.ReadValue<float>() > 0;
    }

    private void Start()
    {
        _playerStats = GetComponent<CreatureStats>();
        _playerAim = GetComponent<PlayerAim>();
        _characterController = GetComponent<CharacterController>();
        _mainCam = Camera.main;

        // Cursor.lockState = CursorLockMode.Locked;
    }

    private void Update()
    {
        MovePlayer();
        RotatePlayer();
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

        _characterController.Move(moveDirection.normalized * moveSpeed * Time.deltaTime);
    }

    private void RotatePlayer()
    {
        Vector3 aimingPoint = _playerAim.GetAimingPoint();
        Vector3 direction = aimingPoint - transform.position;
        direction.y = 0f; // keep upright rotation

        if (direction.sqrMagnitude > 0.0001f)
        {
            transform.rotation = Quaternion.LookRotation(direction);
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed;

    public float groundDrag;

    public float jumpForce;
    public float jumpCooldown;
    public float airMultiplier;
    bool readyToJump;

    private float walkSpeed;
    private float sprintSpeed;

    [Header("Keybinds")]
    public KeyCode jumpKey = KeyCode.Space;

    [Header("Ground Check")]
    public float playerHeight;
    public LayerMask whatIsGround;
    bool grounded;

    public Transform orientation;

    float horizontalInput;
    float verticalInput;

    Vector3 moveDirection;

    CharacterController characterController;

    private void Start()
    {
        walkSpeed = moveSpeed;
        sprintSpeed = moveSpeed * 2;

        characterController = GetComponent<CharacterController>();

        readyToJump = true;
    }

    private void Update()
    {
        // ground check
        grounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.3f, whatIsGround);

        MyInput();
        SpeedControl();

        // handle drag
        // if (grounded)
        //     rb.drag = groundDrag;
        // else
        //     rb.drag = 0;
    }

    private void FixedUpdate()
    {
        MovePlayer();
    }

    private void MyInput()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");

        // when to jump
        if(Input.GetKey(jumpKey) && readyToJump && grounded)
        {
            readyToJump = false;

            Jump();

            Invoke(nameof(ResetJump), jumpCooldown);
        }

        if(Input.GetKey(KeyCode.LeftShift))
            moveSpeed = sprintSpeed;
        else
            moveSpeed = walkSpeed;
    }

    private void MovePlayer()
    {
        // calculate movement direction
        // moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput; //for movement in player orientation
        moveDirection = transform.forward * verticalInput + transform.right * horizontalInput; // for movement in camera orientation


        // on ground
        if(grounded)
            // rb.AddForce(moveDirection.normalized * moveSpeed * 10f, ForceMode.Force);
            characterController.SimpleMove(moveDirection.normalized * moveSpeed * Time.fixedDeltaTime);

        // in air
        else if(!grounded)
            // rb.AddForce(moveDirection.normalized * moveSpeed * 10f * airMultiplier, ForceMode.Force);
            characterController.SimpleMove(moveDirection.normalized * moveSpeed * airMultiplier * Time.fixedDeltaTime);

    }

    private void SpeedControl()
    {
        // // Vector3 flatVel = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
        // Vector3 flatVel = new Vector3(characterController.velocity.x, 0f, characterController.velocity.z);

        // if(flatVel.magnitude > moveSpeed)
        // {
        //     Vector3 limitedVel = flatVel.normalized * moveSpeed;
        //     // rb.velocity = new Vector3(limitedVel.x, rb.velocity.y, limitedVel.z);
        //     characterController.Move(new Vector3(limitedVel.x, characterController.velocity.y, limitedVel.z));
        // }
    }

    private void Jump()
    {
        // reset y velocity
        // rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
        characterController.Move(new Vector3(characterController.velocity.x, 0f, characterController.velocity.z));

        // rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
        characterController.Move(transform.up * jumpForce * Time.fixedDeltaTime);
    }
    private void ResetJump()
    {
        readyToJump = true;
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoClip : MonoBehaviour
{

    public float moveSpeed = 20f;
    public float rotationSpeed = 100f;

    public Rigidbody rb;
    public Camera cam;

    Vector3 movement;
    Vector3 mousePos;

    private float rotationX = 0f;
    private float rotationY = 0f;

    private void Start() {
        rb = GetComponent<Rigidbody>();
        cam = GetComponentInChildren<Camera>();

        Cursor.lockState = CursorLockMode.Locked;
        rb.useGravity = false;
        
    }

    private void Update() {
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.z = Input.GetAxisRaw("Vertical");

        mousePos = cam.ScreenToWorldPoint(Input.mousePosition);
    }

    private void FixedUpdate() {
        rb.MovePosition(rb.position + movement * moveSpeed * Time.fixedDeltaTime);

        rotationX += Input.GetAxis("Mouse X") * rotationSpeed;
        rotationY += Input.GetAxis("Mouse Y") * rotationSpeed;
        rb.rotation = Quaternion.Euler(rotationY, rotationX, 0);
    }



}

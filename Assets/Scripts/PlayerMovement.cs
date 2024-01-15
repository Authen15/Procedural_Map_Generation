using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{

    public float moveSpeed = 5f;
    public float rotationSpeed = 5f;

    public Rigidbody rb;
    public Camera cam;

    Vector3 movement;
    Vector3 mousePos;

    private float rotation = 0f;

    private void Update() {
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.z = Input.GetAxisRaw("Vertical");

        mousePos = cam.ScreenToWorldPoint(Input.mousePosition);
    }

    private void FixedUpdate() {
        rb.MovePosition(rb.position + movement * moveSpeed * Time.fixedDeltaTime);

        rotation += Input.GetAxis("Mouse X") * rotationSpeed;
        rb.rotation = Quaternion.Euler(0, rotation, 0);
    }



}
